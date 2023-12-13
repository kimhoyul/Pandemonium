using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TOONIPLAY
{
    public class UIContextManager : MonoBehaviour
    {
        public struct StackedUIContextComponent
        {
            public UIContextComponent contextComponent;
            public bool needBlockingBoard;
            public BlockingBackground blockingBackground;

            public bool Cancelable => contextComponent.Cancelable;
            public bool Stackable => contextComponent.Stackable;

            public bool NeedBlockingBackground => needBlockingBoard;
        };

        [SerializeField]
        private UIComponent contextPanelUIComponent;

        [SerializeField]
        private BlockingBackground blockingBackground;

        private GameObject lastSelectedObject;
        private readonly Stack<StackedUIContextComponent> openedContextComponentStack = new();

        private BlockingBackground activeBackground;

        private void Awake()
        {
            lastSelectedObject = null;
            openedContextComponentStack.Clear();

            activeBackground = blockingBackground;
            if (activeBackground != null)
                activeBackground.SetActive(false);
        }

        private void Start()
        {
            if (contextPanelUIComponent != null)
                contextPanelUIComponent.HideAllUIComponent();
        }

        private void Update()
        {
            if (openedContextComponentStack.Count == 0)
                return;

            var peekContext = openedContextComponentStack.Peek();
            if (peekContext.Cancelable)
            {
                var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
                if (Input.GetMouseButtonDown(0) && (lastSelectedObject == null || lastSelectedObject != currentSelectedGameObject))
                {
                    lastSelectedObject = currentSelectedGameObject;

                    var currentSelectedContextComponent = FindParentPanelUIComponent(EventSystem.current.currentSelectedGameObject);
                    if (openedContextComponentStack.Peek().contextComponent != currentSelectedContextComponent)
                    {
                        openedContextComponentStack.Peek().contextComponent.OnCanceled();
                        CloseContextComponent();
                    }
                }
            }
        }

        public bool IsOpenedUIContextComponent<T>() where T : UIContextComponent
        {
            foreach (var element in openedContextComponentStack)
            {
                if (element.contextComponent is T)
                    return true;
            }

            return false;
        }

        public bool IsAnyOpenedUIContextComponent() { return openedContextComponentStack.Count > 0; }

        public async Task<T> OpenContextComponent<T>(BlockingBackground blockingBackground = null, float x = float.MinValue, float y = float.MinValue) where T : UIContextComponent
        {
            while (openedContextComponentStack.Count > 0)
            {
                var peekComponent = openedContextComponentStack.Peek();
                if (peekComponent.Stackable)
                    break;

                CloseContextComponent();
            }

            var contextComponent = await contextPanelUIComponent.GetUIComponent<T>() as UIContextComponent;
            if (contextComponent == null)
                return null;

            var contextComponentTrans = contextComponent.gameObject.transform as RectTransform;

            if (contextComponent.NeedBlockingBackground)
            {
                activeBackground = (blockingBackground != null) ? blockingBackground : this.blockingBackground;
                if (activeBackground != null)
                {
                    activeBackground.SetActive(true);
                    contextComponentTrans.SetParent(activeBackground.ContentTrans);
                    contextComponentTrans.localPosition = Vector3.zero;
                }
            }

            if (x > float.MinValue || y > float.MinValue)
            {
                contextComponentTrans.localPosition = Vector3.zero;
                contextComponentTrans.anchoredPosition = new Vector3(x, y, 0.0f);
            }

            contextComponentTrans.SetAsLastSibling();

            contextPanelUIComponent.ShowUIComponent(contextComponent, true, false);

            AddOpenedContenxtQueue(contextComponent);
            return contextComponent as T;
        }

        public void CloseContextComponent<T>() where T : UIContextComponent
        {
            while (openedContextComponentStack.Count > 0)
            {
                bool isLastContextComponent = openedContextComponentStack.Peek().contextComponent is T;
                CloseContextComponent(false);

                if (isLastContextComponent)
                    break;
            }

            if (activeBackground != null)
                OnChooseNeedBlockingBackground();
        }

        public void CloseContextComponent(bool controlBlockingBackground = true)
        {
            if (openedContextComponentStack.Count == 0)
                return;

            var closedContextComponent = openedContextComponentStack.Pop();
            if (closedContextComponent.contextComponent != null)
            {
                Debug.Log(string.Format("Close Context Component : {0}, Frame Count = {1}", closedContextComponent.contextComponent.name, Time.frameCount));

                contextPanelUIComponent.ShowUIComponent(closedContextComponent.contextComponent, false, false);

                if (closedContextComponent.NeedBlockingBackground)
                    closedContextComponent.contextComponent.gameObject.transform.SetParent(contextPanelUIComponent.gameObject.transform);

                lastSelectedObject = null;
                EventSystem.current.SetSelectedGameObject(null);

                if (controlBlockingBackground && activeBackground != null)
                    OnChooseNeedBlockingBackground();
            }
        }

        public void CloseAllContextComponent()
        {
            while (openedContextComponentStack.Count > 0)
            {
                CloseContextComponent(false);
            }

            if (activeBackground != null)
            {
                activeBackground.SetActive(false);
                activeBackground = blockingBackground;
            }
        }

        public void ClearAllContextComponent()
        {
            CloseAllContextComponent();

            var trans = contextPanelUIComponent.transform;
            while (trans.childCount != 0)
            {
                var child = trans.GetChild(0);
                child.SetParent(null);
                
                if (contextPanelUIComponent.UnregisterUIComponent(child.gameObject.GetComponent<UIContextComponent>()))
                    GameObject.Destroy(child.gameObject);
            }
        }

        public async Task<T> GetUIContextComponent<T>() where T : UIContextComponent => await contextPanelUIComponent.GetUIComponent<T>();

        private void OnChooseNeedBlockingBackground()
        {            
            if (openedContextComponentStack.TryPeek(out var previousContextComponent) && previousContextComponent.needBlockingBoard)
            {
                activeBackground.SetActive(false);
                activeBackground = previousContextComponent.blockingBackground;
                activeBackground.SetActive(true);
            }
            else
            {
                activeBackground.SetActive(false);
                activeBackground = blockingBackground;
            }
        }

        private void AddOpenedContenxtQueue(UIContextComponent openedContextPanel)
        {
            openedContextComponentStack.Push(new StackedUIContextComponent()
            {
                contextComponent = openedContextPanel,
                needBlockingBoard = openedContextPanel.NeedBlockingBackground,
                blockingBackground = activeBackground
            });

            EventSystem.current.SetSelectedGameObject(openedContextPanel.gameObject);
            lastSelectedObject = openedContextPanel.gameObject;
        }

        private UIContextComponent FindParentPanelUIComponent(GameObject selectedUIObject)
        {
            if (selectedUIObject == null)
                return null;

            var searchObjectTrans = selectedUIObject.transform;
            var lastPopupPanel = openedContextComponentStack.Peek().contextComponent.gameObject.transform;

            while (searchObjectTrans != null)
            {
                if (searchObjectTrans == lastPopupPanel)
                    return searchObjectTrans.gameObject.GetComponent<UIContextComponent>();

                searchObjectTrans = searchObjectTrans.parent;
            }

            return null;
        }
    }
}

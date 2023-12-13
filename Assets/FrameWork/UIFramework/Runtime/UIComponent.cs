using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using AYellowpaper.SerializedCollections;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif
#endif

namespace TOONIPLAY
{
#if ODIN_INSPECTOR
    public class UIComponent : SerializedMonoBehaviour
#else
    public class UIComponent : MonoBehaviour, IViewModelAccessor
#endif
    {
        protected UIComponent parentUIComponent;

        [SerializeField]
        private bool alwaysShow;

        private bool IsAlwaysShow
        {
            get => alwaysShow;
            set => alwaysShow = value;
        }

        public ObservableObject ViewModelInstance { get; set; }

        private bool _isInitialized;

        public Transform componentRoot;

        #region Component Map
#if UNITY_EDITOR
        [SerializeField]
        private List<UIComponent> componentList = new();
#endif

        [SerializeField]
#if ODIN_INSPECTOR
        [DictionaryDrawerSettings(IsReadOnly = true)]
        protected Dictionary<string, UIComponent> componentMap = new();
#else
        protected SerializedDictionary<string, UIComponent> componentMap = new();
#endif

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (componentList == null)
                return;

#if ODIN_INSPECTOR
            var newComponentMap = new Dictionary<string, UIComponent>();
#else
            var newComponentMap = new SerializedDictionary<string, UIComponent>();
#endif

            foreach (var element in componentList)
            {
                if (element != null)
                    newComponentMap.Add(element.GetType().Name, element);
            }

            if (!componentMap.Equals(newComponentMap))
                componentMap = newComponentMap;
        }
#endif

        private void RegisterParentUIComponentAll()
        {
            _isInitialized = true;

#if UNITY_EDITOR
            if (componentList.Count != componentMap.Count)
                Debug.Assert(componentMap.Count == componentList.Count, gameObject.name + " : UI Component의 갯수가 맞지 않습니다.");
#endif

            foreach (var element in componentMap)
            {
                if (element.Value != null)
                    element.Value.parentUIComponent = this;
            }
        }
        #endregion

        private void Awake()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                RegisterParentUIComponentAll();
            }

            OnAwake();
        }

        protected virtual void OnAwake() { }

        public void Show(bool isShow, UIComponent notifyTarget = null)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                RegisterParentUIComponentAll();
            }

            if (!isShow && IsAlwaysShow)
                return;
            
            OnShow(isShow);

            if (notifyTarget != null)
            {
                notifyTarget.NotifyShow(this, isShow);
            }
            else
            {
                if (parentUIComponent != null)
                    parentUIComponent.NotifyShow(this, isShow);
            }
        }

        public virtual void NotifyShow(UIComponent sender, bool isShow) { }

        protected virtual void OnShow(bool isShow) => gameObject.SetActive(isShow);

        public async Task<T> ShowUIComponent<T>(bool isShow = true, bool showOnly = true) where T : UIComponent
        {
            var foundComponent = await GetUIComponent<T>();
            if (!showOnly)
            {
                if (foundComponent != null)
                    foundComponent.Show(isShow);
            }
            else
            {
                foreach (var element in componentMap.Where(element => element.Value != null))
                {
                    element.Value.Show((element.Value == foundComponent) && isShow);
                }
            }
            return foundComponent;
        }

        public async Task<UIComponent> ShowUIComponent(string panelTypeName, bool isShow = true, bool showOnly = true)
        {
            var foundComponent = await GetUIComponent(panelTypeName);
            if (!showOnly)
            {
                if (foundComponent != null)
                    foundComponent.Show(isShow);
            }
            else
            {
                foreach (var element in componentMap.Where(element => element.Value != null))
                {
                    element.Value.Show((element.Value == foundComponent) && isShow);
                }
            }
            return foundComponent;
        }

        public void ShowUIComponent(UIComponent target, bool isShow = true, bool showOnly = true)
        {
            Debug.Assert(componentMap.TryGetValue(target.GetType().Name, out _), "해당하는 target이 없습니다.");

            if (!showOnly)
            {
                if (target != null)
                    target.Show(isShow);

                return;
            }

            foreach (var element in componentMap.Where(element => element.Value != null))
            {
                element.Value.Show(element.Value == target && isShow);
            }
        }

        public void HideAllUIComponent(bool isForce = false)
        {
            foreach (var element in componentMap)
            {
                if (element.Value == null)
                    continue;

                if (isForce)
                {
                    var originTemp = element.Value.IsAlwaysShow;
                    element.Value.IsAlwaysShow = false;
                    element.Value.Show(false);
                    element.Value.IsAlwaysShow = originTemp;
                }
                else
                {
                    element.Value.Show(false);
                }
            }
        }

        public async Task<T> GetUIComponent<T>() where T : UIComponent => await GetUIComponent(typeof(T)) as T;

        public async Task<UIComponent> GetUIComponent(Type panelType)
        {
            var id = panelType.Name;

            if (componentMap.TryGetValue(id, out UIComponent outObject))
                return outObject;

            return await UIManager.Instance.LoadUIComponent(panelType, this);
        }
        
        public async Task<UIComponent> GetUIComponent(string panelTypeName)
        {
            if (componentMap.TryGetValue(panelTypeName, out var outObject))
                return outObject;

            return await UIManager.Instance.LoadUIComponent(panelTypeName, this);
        }

        public T GetUIComponentWithChildren<T>() where T : UIComponent
        {
            return GetUIComponentWithChildren(typeof(T).Name) as T;
        }

        public List<UIComponent> GetAllComponents() => componentMap.Values.ToList();

        // NOTE : Button listener용 유틸리티 함수
        public void OnClickButtonShowUIComponent(UIComponent component) => ShowUIComponent(component);

        private UIComponent GetUIComponentWithChildren(string id)
        {
            if (componentMap.TryGetValue(id, out var outObject))
                return outObject;

            foreach (var element in componentMap)
            {
                var child = element.Value.GetUIComponentWithChildren(id);
                if (child != null)
                    return child;
            }

            return null;
        }

        public bool RegisterUIComponent(UIComponent component)
        {
            component.parentUIComponent = this;
            return componentMap.TryAdd(component.GetType().Name, component);
        }

        public bool UnregisterUIComponent(UIComponent component) => componentMap.Remove(component.GetType().Name);
    }
}

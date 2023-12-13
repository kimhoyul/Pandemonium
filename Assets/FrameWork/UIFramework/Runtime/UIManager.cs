using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TOONIPLAY
{
    [RequireComponent(typeof(UIComponent), typeof(UIContextManager))]
    public class UIManager : TSingletonMonoBehaviour<UIManager>
    {
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private UIComponent loadingRootPanel;
        [SerializeField] private IndicatorScreen indicatorScreen;

        private UIComponent _globalUIComponent;
        private UIContextManager _contextManager;

        public float AspectRatio => rootCanvas.scaleFactor;

        private Dictionary<string, AssetReference> _uiComponentAssetMap;
        private Dictionary<string, AsyncOperationHandle<GameObject>> _loadingHandle;

        protected override void OnAwake()
        {
            _globalUIComponent = GetComponent<UIComponent>();
            _contextManager = GetComponent<UIContextManager>();

            loadingRootPanel.HideAllUIComponent();
            
            indicatorScreen.SetActive(false);
        }

        public async Task<T> OpenContextComponent<T>(BlockingBackground blockingBackground = null, float x = float.MinValue, float y = float.MinValue) where T : UIContextComponent => await _contextManager.OpenContextComponent<T>(blockingBackground, x, y);

        public void CloseContextComponent<T>() where T : UIContextComponent => _contextManager.CloseContextComponent<T>();

        public void CloseContextComponent() => _contextManager.CloseContextComponent();

        public void CloseAllContextComponent() => _contextManager.CloseAllContextComponent();

        public void ClearAllContextComponent() => _contextManager.ClearAllContextComponent();

        public async Task<T> GetUIContextComponent<T>() where T : UIContextComponent => await _contextManager.GetUIContextComponent<T>();

        public bool IsOpenedUIContextComponent<T>() where T : UIContextComponent => _contextManager.IsOpenedUIContextComponent<T>();

        public bool IsAnyOpenedUIContextComponent() => _contextManager.IsAnyOpenedUIContextComponent();

        public Rect CalcRealRect(RectTransform targetTrans)
        {
            var scaleFactor = rootCanvas.scaleFactor;
            
            var min = Screen.safeArea.position + Screen.safeArea.size * targetTrans.anchorMin + targetTrans.offsetMin * scaleFactor;

            var sizeDelta = targetTrans.sizeDelta;
            return new Rect(min.x, min.y, sizeDelta.x * scaleFactor, sizeDelta.y * scaleFactor);
        }

        public void ApplySetting(UISettingSO uiSetting)
        {
            _uiComponentAssetMap ??= new();
            
            for (var i = 0; i < uiSetting.uiComponentAssetKey.Count; ++i)
            {
                _uiComponentAssetMap.Add(uiSetting.uiComponentAssetKey[i], uiSetting.uiAssetReferences[i]);
            }

            _loadingHandle = new();
        }

        public async Task<T> LoadUIComponent<T>(UIComponent parentUIComponent) where T : UIComponent => await LoadUIComponent(typeof(T), parentUIComponent) as T;

        public async Task<UIComponent> LoadUIComponent(Type componentType, UIComponent parentUIComponent)
        {
            if (_uiComponentAssetMap == null)
                return null;

            if (!_uiComponentAssetMap.TryGetValue(componentType.Name, out var assetReference))
                return null;

            GameObject uiComponentGo;
            if (assetReference.Asset == null)
            {
                if (_loadingHandle.ContainsKey(componentType.Name))
                    return null;

                var handle = assetReference.LoadAssetAsync<GameObject>();
                {
                    _loadingHandle.Add(componentType.Name, handle);
                    await handle.Task;
                    _loadingHandle.Remove(componentType.Name);
                }

                if (handle.Status != AsyncOperationStatus.Succeeded)
                    return null;

                uiComponentGo = GameObject.Instantiate(handle.Result, parentUIComponent.componentRoot != null ? parentUIComponent.componentRoot : parentUIComponent.gameObject.transform);
            }
            else
            {
                uiComponentGo = GameObject.Instantiate(assetReference.Asset as GameObject, parentUIComponent.componentRoot != null ? parentUIComponent.componentRoot : parentUIComponent.gameObject.transform);
            }

            if (uiComponentGo == null)
                return null;

            var uiComponent = uiComponentGo.GetComponent(componentType) as UIComponent;
            if (uiComponent != null)
            {
                if (parentUIComponent.RegisterUIComponent(uiComponent))
                {
                    uiComponentGo.SetActive(false);

                    return uiComponent;
                }
            }

            GameObject.Destroy(uiComponentGo);

            return null;
        }

        public async Task<UIComponent> LoadUIComponent(string componentTypeName, UIComponent parentUIComponent)
        {
            if (_uiComponentAssetMap == null)
                return null;

            if (!_uiComponentAssetMap.TryGetValue(componentTypeName, out var assetReference))
                return null;

            GameObject uiComponentGo;
            if (assetReference.Asset == null)
            {
                if (_loadingHandle.ContainsKey(componentTypeName))
                    return null;

                var handle = assetReference.LoadAssetAsync<GameObject>();
                {
                    _loadingHandle.Add(componentTypeName, handle);
                    await handle.Task;
                    _loadingHandle.Remove(componentTypeName);
                }

                if (handle.Status != AsyncOperationStatus.Succeeded)
                    return null;

                uiComponentGo = GameObject.Instantiate(handle.Result, parentUIComponent.componentRoot != null ? parentUIComponent.componentRoot : parentUIComponent.gameObject.transform);
            }
            else
            {
                uiComponentGo = GameObject.Instantiate(assetReference.Asset as GameObject, parentUIComponent.componentRoot != null ? parentUIComponent.componentRoot : parentUIComponent.gameObject.transform);
            }

            if (uiComponentGo == null)
                return null;

            var uiComponent = uiComponentGo.GetComponent<UIComponent>();
            if (uiComponent != null)
            {
                if (parentUIComponent.RegisterUIComponent(uiComponent))
                {
                    uiComponentGo.SetActive(false);

                    return uiComponent;
                }
            }

            GameObject.Destroy(uiComponentGo);

            return null;
        }

        private Coroutine _coSetIndicator;
        public void SetIndicatorScreen(bool enable)
        {
            if (_coSetIndicator != null)
            {
                StopCoroutine(_coSetIndicator);
                _coSetIndicator = null;
                
                indicatorScreen.SetActive(false);
            }

            if (enable)
                _coSetIndicator = StartCoroutine(CoSetIndicator());
        }

        private IEnumerator CoSetIndicator()
        {
            indicatorScreen.SetActive(true);

            yield return new WaitForSeconds(0.3f);

            indicatorScreen.ActivateIndicator(true);
        }

        public async Task<LoadingPanel> ShowLoadingPanel(string panelClassType)
        {
            var loadingPanel = await loadingRootPanel.ShowUIComponent(panelClassType) as LoadingPanel;

            return loadingPanel;
        }

        public void HideLoadingPanel() => loadingRootPanel.HideAllUIComponent();
    }
}

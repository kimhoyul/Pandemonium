using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TOONIPLAY
{
    public class PhaseManager : TSingletonMonoBehaviour<PhaseManager>
    {
        private BasePhase _currentPhase;
        private ZoneInfoContainer _zoneInfoContainer;

        public T GetCurrentPhase<T>() where T : BasePhase => _currentPhase as T;

        public UIController GetCurrentUIController() => _currentPhase == null ? null : _currentPhase.GetUIController();

        public T GetCurrentUIController<T>() where T : UIController => _currentPhase == null ? null : _currentPhase.GetUIController<T>();

        public bool TryGetCurrentUIController<T>(out T result) where T : UIController
        {
            if (_currentPhase == null)
            {
                result = null;
                return false;
            }

            result = _currentPhase.GetUIController<T>();

            return result != null;
        }

        public void LoadPhaseAsync(string phaseName, object phaseParams, string blockingTypeName)
        {
            StartCoroutine(OnLoadPhaseAsync(phaseName, phaseParams, blockingTypeName));
        }

        private IEnumerator OnLoadPhaseAsync(string phaseName, object phaseParams, string blockingTypeName)
        {
            LoadingPanel loadingPanel = null;
            if (!string.IsNullOrEmpty(blockingTypeName))
            {
                var loadingPanelTask = UIManager.Instance.ShowLoadingPanel(blockingTypeName);
                yield return new WaitUntil(() => loadingPanelTask.IsCompleted);

                loadingPanel = loadingPanelTask.Result;
            }
            
            UIManager.Instance.ClearAllContextComponent();

            var uiController = GetCurrentUIController();
            if (uiController != null)
                uiController.HideAllUIComponent(true);

            var loadOperation = SceneManager.LoadSceneAsync("VoidLoadingPhase", LoadSceneMode.Single);
            while (!loadOperation.isDone)
            {
                yield return null;
            }

            loadOperation = SceneManager.LoadSceneAsync(phaseName, LoadSceneMode.Single);
            float progressMax = 0f;
            while (!loadOperation.isDone)
            {
                progressMax = Mathf.Max(progressMax, loadOperation.progress);

                if (loadingPanel != null)
                    loadingPanel.SetProgressValue(progressMax);

                yield return null;
            }

            if (loadingPanel != null)
                loadingPanel.SetProgressValue(progressMax);

            yield return new WaitForSeconds(0.5f);

            var basePhase = GameObject.FindObjectOfType<BasePhase>();
            if (basePhase == null)
                yield break;
            
            _currentPhase = basePhase;
            Debug.Log(_currentPhase.gameObject.name + " Scene OnLoadedPhase");
            _currentPhase.OnLoadedPhase(phaseParams);
            
            if (loadingPanel != null)
                UIManager.Instance.HideLoadingPanel();
        }

        public void UnloadPhase(string phaseName)
        {
            StartCoroutine(OnUnloadPhase(phaseName));
        }

        private IEnumerator OnUnloadPhase(string phaseName)
        {
            var loadOperation = SceneManager.UnloadSceneAsync(phaseName);
            while (!loadOperation.isDone)
            {
                yield return null;
            }
        }

        public void ApplySetting(PhaseSettingSO phaseSetting)
        {
            _zoneInfoContainer = phaseSetting.zoneInfoContainer;

            LoadPhaseAsync(phaseSetting.initialPhaseScene, null, phaseSetting.initialBlockingTypeName);
        }

        // TODO(jaeil): 존 관련 기능은 따로 분리하는게 어떨까?
        public void LoadZoneAsync(string zoneName)
        {
            StartCoroutine(OnLoadZoneAsync(zoneName));
        }

        private IEnumerator OnLoadZoneAsync(string zoneName)
        {
            var loadOperation = SceneManager.LoadSceneAsync(zoneName, LoadSceneMode.Additive);
            while (!loadOperation.isDone)
            {
                yield return null;
            }

            var currentPhase = GetCurrentPhase<BasePhase>();
            if (currentPhase != null)
                currentPhase.SendMessage("OnLoadedZone", SceneManager.GetSceneAt(1));
        }

        public ZoneType GetZoneType(string zoneName)
        {
            foreach (var zoneInfo in _zoneInfoContainer.data)
            {
                if (zoneName.Equals(zoneInfo.spaceCd))
                {
                    if (zoneInfo.zoneContentsInfo.zoneType < _zoneInfoContainer.zoneType.Count)
                        return _zoneInfoContainer.zoneType[zoneInfo.zoneContentsInfo.zoneType];

                    break;
                }
            }

            return null;
        }

        public ZoneType GetZoneTypeByInfo(ZoneInfoSO zoneInfo) => (zoneInfo.zoneContentsInfo.zoneType < _zoneInfoContainer.zoneType.Count) ? _zoneInfoContainer.zoneType[zoneInfo.zoneContentsInfo.zoneType] : null;

        public ZoneInfoSO GetZoneInfo(string zoneName)
        {
            return _zoneInfoContainer.data.FirstOrDefault(zoneInfo => zoneName.Equals(zoneInfo.spaceCd));
        }
    }
}

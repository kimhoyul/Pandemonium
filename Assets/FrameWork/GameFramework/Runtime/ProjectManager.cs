using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace TOONIPLAY
{
    public class ProjectManager : TSingletonMonoBehaviour<ProjectManager>
    {
        private ProjectSettingSO _projectSetting;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            var basePhase = GameObject.FindObjectOfType<BasePhase>();
            if (basePhase != null)
            {
                Debug.Log("[ProjectManager] Loading Empty Scene...");

                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
        }
#endif

        protected override void OnAwake()
        {
            base.OnAwake();

            _projectSetting = Resources.Load<ProjectSettingSO>(ProjectSettingSO.ProjectSettingAsset);

            var settingContainer = _projectSetting.builtSettingContainer;
            foreach (var systemSetting in settingContainer.container.systemSettingList)
            {
                AsyncOperationHandle handle = systemSetting.LoadAssetAsync<SystemSettingSO>();
                handle.Completed += Handle_Completed;
            }
        }

        private void Handle_Completed(AsyncOperationHandle obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                var setting = obj.Result as SystemSettingSO;
                if (setting != null)
                    setting.ApplySetting();
            }
            else
            {
                Debug.LogError($"AssetReference {obj} failed to load.");
            }
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TOONIPLAY
{
    public enum SettingPlatform
    {
        Desktop,
        Android,
        IOS,
        WebGL,
    };
    
    public class ProjectSettingSO : ScriptableObject
    {
        [System.Serializable]
        public class PlatformSystemSettingContainer
        {
#if UNITY_EDITOR
            public SettingPlatform settingPlatform;
#endif
            public SystemSettingContainerSO container;
        }

        private const string ResourcesPath = "Assets/Resources";

        public const string ProjectSettingAsset = "ProjectSetting";
        public const string ProjectSettingPath = ResourcesPath + "/" + ProjectSettingAsset + ".asset";

        public List<PlatformSystemSettingContainer> systemSettingContainers;
        public PlatformSystemSettingContainer builtSettingContainer;
    }
}

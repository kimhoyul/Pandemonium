using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using TOONIPLAY.Utilities;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif


namespace TOONIPLAY
{
    public class ProjectSettingWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset projectSettingWindowTreeAsset;

        private readonly List<SystemSettingSO> _settingMenuList = new();

        private ProjectSettingSO _projectSetting;

        [MenuItem("Tooniplay/Project Setting")]
        private static void ShowWindow()
        {
            GetWindow<ProjectSettingWindow>("Project Setting");
        }

        private void CreateGUI()
        {
            if (!Directory.Exists(Path.Combine(Application.dataPath, "Resources")))
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources"));

            _projectSetting = AssetDatabase.LoadAssetAtPath<ProjectSettingSO>(ProjectSettingSO.ProjectSettingPath);
            if (_projectSetting == null)
            {
                _projectSetting = CreateInstance<ProjectSettingSO>();
                AssetDatabase.CreateAsset(_projectSetting, ProjectSettingSO.ProjectSettingPath);
            }

            var labelFromUxml = projectSettingWindowTreeAsset.Instantiate();
            rootVisualElement.Add(labelFromUxml);

            var serializedSetting = new SerializedObject(_projectSetting);

            var systemSettingContainersField = labelFromUxml.Q<PropertyField>("systemSettingContainers");
            systemSettingContainersField?.Bind(serializedSetting);

            var createButton = labelFromUxml.Q<Button>("createButton");
            if (createButton != null)
                createButton.clicked += OnCreateSystemSettingContainer;

            var currentSettingPlatform = EditorUserBuildSettings.activeBuildTarget switch
            {
                BuildTarget.StandaloneWindows64 => SettingPlatform.Desktop,
                BuildTarget.StandaloneLinux64   => SettingPlatform.Desktop,
                BuildTarget.StandaloneWindows   => SettingPlatform.Desktop,
                BuildTarget.StandaloneOSX       => SettingPlatform.Desktop,
                BuildTarget.Android             => SettingPlatform.Android,
                BuildTarget.iOS                 => SettingPlatform.IOS,
                BuildTarget.WebGL               => SettingPlatform.WebGL,
                _                               => throw new ArgumentOutOfRangeException()
            };

            _projectSetting.builtSettingContainer
                = _projectSetting.systemSettingContainers.Find(x => x.settingPlatform == currentSettingPlatform);

            RefreshSystemSettingListView(rootVisualElement, _projectSetting.builtSettingContainer.container);
        }

        private static void OnCreateSystemSettingContainer()
        {
            var path = EditorUtility.SaveFilePanel("Create new SystemSettingContainer", "Assets/Resources", "New SystemSettingContainer", "asset");
            if (string.IsNullOrEmpty(path))
                return;

            var newSystemSettingContainer = CreateInstance<SystemSettingContainerSO>();
            AssetDatabase.CreateAsset(newSystemSettingContainer, AssetDatabase.GenerateUniqueAssetPath(path.Substring(path.IndexOf("Assets", StringComparison.Ordinal))));
            AssetDatabase.SaveAssets();
        }

        private void RefreshSystemSettingListView(VisualElement rootElement, SystemSettingContainerSO systemSettingContainer)
        {
            var systemSettingRootView = rootElement.Q<VisualElement>("systemSettingRootView");
            if (systemSettingRootView == null)
                return;

            if (systemSettingContainer == null)
            {
                systemSettingRootView.style.visibility = Visibility.Hidden;
                return;
            }

            systemSettingRootView.style.visibility = Visibility.Visible;

            var allSettings = ReflectionHelpers.GetTypesInAllDependentAssemblies(t => typeof(SystemSettingSO).IsAssignableFrom(t) && !t.IsAbstract);
            foreach (var settingType in allSettings)
            {
                var isExistSetting = systemSettingContainer.systemSettingList.Any(reference => reference.editorAsset.GetType() == settingType);
                if (isExistSetting)
                    continue;
                
                var assetPath = $"Assets/Resources/{settingType.Name}.asset";
                var assetInstance = ScriptableObject.CreateInstance(settingType);
                AssetDatabase.CreateAsset(assetInstance, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                systemSettingContainer.systemSettingList.Add(new AssetReference(AssetDatabase.AssetPathToGUID(assetPath)));
            }
            
            _settingMenuList.Clear();
            foreach (var setting in systemSettingContainer.systemSettingList.Select(reference => reference.editorAsset).OfType<SystemSettingSO>())
            {
                _settingMenuList.Add(setting);
            }

            // 왼쪽 ListView 생성
            var menuListView = rootElement.Q<ListView>("settingListView");
            if (menuListView == null)
                return;

            menuListView.makeItem = () => new Label();
            menuListView.bindItem = (item, index) =>
            {
                if (item is Label label)
                    label.text = _settingMenuList[index].displayName;
            };
            menuListView.itemsSource = _settingMenuList;

            // 오른쪽 ScrollView 생성
            var menuContentsElement = rootElement.Q<ScrollView>("propertyScrollView");

#if UNITY_2022_2_OR_NEWER
            menuListView.selectionChanged += (selectedItems) =>
#else
            menuListView.onSelectionChange += (selectedItems) =>
#endif
            {
                foreach (var item in selectedItems)
                {
                    if (menuContentsElement == null)
                        continue;

                    menuContentsElement.Clear();

                    var projectSetting = item as SystemSettingSO;
                    if (projectSetting != null)
                        projectSetting.OnPropertyDraw(ref menuContentsElement);
                }
            };

            if (_settingMenuList.Count > 0)
                menuListView.selectedIndex = 0;
        }
    }
}

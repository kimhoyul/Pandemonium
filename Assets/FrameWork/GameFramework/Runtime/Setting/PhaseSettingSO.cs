using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
#endif

namespace TOONIPLAY
{
    public class PhaseSettingSO : SystemSettingSO
    {
        public string initialBlockingTypeName;
        public string initialPhaseScene;
        public ZoneInfoContainer zoneInfoContainer;

        PhaseSettingSO()
        {
            displayName = "Phase";
        }

#if UNITY_EDITOR
        private readonly List<string> _phaseNameList = new();
        private readonly List<string> _phasePathList = new();
        private int _selectedPhaseIndex;

        public override void OnPropertyDraw(ref ScrollView rootElement)
        {
            var serializedSetting = new SerializedObject(this);
            var settingProperty = serializedSetting.GetIterator();
            settingProperty.Next(true);

            while (settingProperty.NextVisible(false))
            {
                if (string.Equals(settingProperty.name, "initialPhaseScene"))
                {
                    var sceneCount = SceneManager.sceneCountInBuildSettings;
                    for (var i = 0; i < sceneCount; i++)
                    {
                        var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                        _phasePathList.Add(scenePath);
                        _phaseNameList.Add(System.IO.Path.GetFileNameWithoutExtension(scenePath));
                    }

                    _selectedPhaseIndex = _phaseNameList.FindIndex(x => x == initialPhaseScene);
                    if (_selectedPhaseIndex == -1)
                        _selectedPhaseIndex = 0;

                    var rootField = new VisualElement
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Row
                        }
                    };

                    var initialPhaseField = new DropdownField("Initial Phase", _phaseNameList, _selectedPhaseIndex)
                    {
                        bindingPath = "initialPhaseScene"
                    };

                    initialPhaseField.RegisterCallback<ChangeEvent<string>>((evt) =>
                    {
                        initialPhaseScene = evt.newValue;

                        _selectedPhaseIndex = _phaseNameList.FindIndex(x => x == initialPhaseScene);
                    });

                    initialPhaseField.Bind(serializedSetting);

                    var openSceneButton = new Button(() => { EditorSceneManager.OpenScene(_phasePathList[_selectedPhaseIndex]); })
                    {
                        text = "Open Scene"
                    };

                    rootField.Add(initialPhaseField);
                    rootField.Add(openSceneButton);

                    rootElement.Add(rootField);
                }
                else if (string.Equals(settingProperty.name, "zoneInfoContainer"))
                {
                    var prop = new PropertyField(settingProperty);
                    prop.Bind(serializedSetting);
                    rootElement.Add(prop);
                }
                else
                {
                    var prop = new PropertyField(settingProperty);

                    if (string.Equals(settingProperty.name, "m_Script"))
                        prop.SetEnabled(false);

                    if (string.Equals(settingProperty.name, "displayName"))
                        prop.SetEnabled(false);

                    prop.Bind(serializedSetting);
                    rootElement.Add(prop);
                }
            }
        }
#endif

        public override void ApplySetting() => PhaseManager.Instance.ApplySetting(this);
    }
}

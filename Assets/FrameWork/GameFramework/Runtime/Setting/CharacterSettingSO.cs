using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace TOONIPLAY
{
    public class CharacterSettingSO : SystemSettingSO
    {
        public GameObject characterRootPrefab;
        public List<GameObject> modelPrefabs = new();

        CharacterSettingSO()
        {
            displayName = "Character";
        }

#if UNITY_EDITOR
        public override void OnPropertyDraw(ref ScrollView rootElement)
        {
            var serializedSetting = new SerializedObject(this);
            var settingProperty = serializedSetting.GetIterator();
            settingProperty.Next(true);

            while (settingProperty.NextVisible(false))
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
#endif

        public override void ApplySetting() => CharacterManager.Instance.ApplySetting(this);
    }
}

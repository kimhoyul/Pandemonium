using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace TOONIPLAY
{
    public class UISettingSO : SystemSettingSO
    {
        public List<string> uiComponentAssetKey;
        public List<AssetReference> uiAssetReferences;

        UISettingSO()
        {
            displayName = "UI";
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

                if (string.Compare(settingProperty.name, "m_Script") == 0)
                    prop.SetEnabled(false);

                if (string.Compare(settingProperty.name, "displayName") == 0)
                    prop.SetEnabled(false);

                if (string.Compare(settingProperty.name, "uiComponentAssetKey") == 0)
                    prop.SetEnabled(false);

                if (string.Compare(settingProperty.name, "uiAssetReferences") == 0)
                {
                    prop.RegisterValueChangeCallback((test) =>
                    {
                        Debug.Log(test);
                    });
                    prop.TrackSerializedObjectValue(serializedSetting, TestCallback);
                }

                prop.Bind(serializedSetting);
                rootElement.Add(prop);
            }
        }

        private void TestCallback(SerializedObject serializedObject)
        {
            var uiSetting = serializedObject.targetObject as UISettingSO;

            uiSetting.uiComponentAssetKey = new();
            foreach (var asset in uiSetting.uiAssetReferences)
            {
                var panel = asset.editorAsset as GameObject;
                var uiComponent = panel.GetComponent<UIComponent>();
                if (uiComponent != null)
                {
                    uiSetting.uiComponentAssetKey.Add(uiComponent.GetType().Name);
                }
                else
                {
                    uiSetting.uiComponentAssetKey.Add("");
                }
            }
        }
#endif

        public override void ApplySetting() => UIManager.Instance.ApplySetting(this);
    }
}

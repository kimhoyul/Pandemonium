using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using TOONIPLAY.Utilities;

namespace TOONIPLAY
{
    [CustomEditor(typeof(BaseCharacterController), true)]
    public class BaseCharacterControllerInspector : Editor
    {
        private static readonly List<string> AddonNameList = new();
        private static readonly List<Type> AddonTypeList = new();

        private DropdownField _addonDropdown;

        public override VisualElement CreateInspectorGUI()
        {
            var rootVisualElement = new VisualElement();

            var serializedProperty = serializedObject.GetIterator();
            serializedProperty.Next(true);

            while (serializedProperty.NextVisible(false))
            {
                var prop = new PropertyField(serializedProperty);

                if (string.Equals(serializedProperty.name, "m_Script"))
                    prop.SetEnabled(false);

                prop.Bind(serializedObject);

                rootVisualElement.Add(prop);
            }

            {
                AddonNameList.Clear();
                AddonTypeList.Clear();

                AddonNameList.Add("(select)");
                AddonTypeList.Add(null);

                var allExtensions = ReflectionHelpers.GetTypesInAllDependentAssemblies(t => typeof(CharacterControllerAddOn).IsAssignableFrom(t) && !t.IsAbstract);
                foreach (var t in allExtensions)
                {
                    AddonNameList.Add(t.Name);
                    AddonTypeList.Add(t);
                }
            }

            _addonDropdown = new DropdownField("Add Addons", AddonNameList, 0, OnSelectedValueAddon);

            rootVisualElement.Add(_addonDropdown);

            return rootVisualElement;
        }

        private string OnSelectedValueAddon(string addonName)
        {
            if (_addonDropdown != null)
            {
                var type = AddonTypeList[_addonDropdown.index];
                var targetObject = (target as BaseCharacterController)?.gameObject;
                if (targetObject != null && type != null && targetObject.GetComponent(type) == null)
                    Undo.AddComponent(targetObject, type);

                _addonDropdown.index = 0;
            }

            return AddonNameList[0];
        }
    }
}

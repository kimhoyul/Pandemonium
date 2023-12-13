using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Configuration;

namespace TOONIPLAY
{
    [CustomEditor(typeof(InteractableObject), true)]
    public class InteractableObjectInspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var rootVisualElement = new VisualElement();

            var serializedProperty = serializedObject.GetIterator();
            serializedProperty.Next(true);

            while (serializedProperty.NextVisible(false))
            {
                if (string.Equals(serializedProperty.name, "m_Delegates"))
                    continue;

                var prop = new PropertyField(serializedProperty);

                if (string.Equals(serializedProperty.name, "m_Script"))
                    prop.SetEnabled(false);

                prop.Bind(serializedObject);

                rootVisualElement.Add(prop);
            }

            return rootVisualElement;
        }
    }
}

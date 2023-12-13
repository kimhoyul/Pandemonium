using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UI;

namespace TOONIPLAY
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIToggle), true)]
    public class UIToggleInspector : SelectableEditor
    {
        SerializedProperty m_OnValueChangedProperty;
        SerializedProperty m_TransitionProperty;
        SerializedProperty m_GraphicProperty;
        SerializedProperty m_GroupProperty;
        SerializedProperty m_IsOnProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_TransitionProperty = serializedObject.FindProperty("toggleTransition");
            m_GraphicProperty = serializedObject.FindProperty("graphic");
            m_GroupProperty = serializedObject.FindProperty("m_Group");
            m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
            m_OnValueChangedProperty = serializedObject.FindProperty("onValueChanged");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            UIToggle tabButton = serializedObject.targetObject as UIToggle;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_IsOnProperty);
            if (EditorGUI.EndChangeCheck())
            {
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(tabButton.gameObject.scene);

                UIToggleGroup group = m_GroupProperty.objectReferenceValue as UIToggleGroup;

                tabButton.isOn = m_IsOnProperty.boolValue;

                if (group != null && group.isActiveAndEnabled && tabButton.IsActive())
                {
                    if (tabButton.isOn || (!group.AnyTogglesOn() && !group.allowSwitchOff))
                    {
                        tabButton.isOn = true;
                        group.NotifyToggleOn(tabButton);
                    }
                }
            }
            EditorGUILayout.PropertyField(m_TransitionProperty);
            EditorGUILayout.PropertyField(m_GraphicProperty);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_GroupProperty);
            if (EditorGUI.EndChangeCheck())
            {
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(tabButton.gameObject.scene);

                UIToggleGroup group = m_GroupProperty.objectReferenceValue as UIToggleGroup;
                tabButton.group = group;
            }

            EditorGUILayout.Space();

            // Draw the event notification options
            EditorGUILayout.PropertyField(m_OnValueChangedProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

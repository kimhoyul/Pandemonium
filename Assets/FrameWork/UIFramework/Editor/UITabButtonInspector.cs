using UnityEditor;

namespace TOONIPLAY
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UITabButton), true)]
    public class UITabButtonInspector : UIToggleInspector
    {
        SerializedProperty m_TabPanelProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_TabPanelProperty = serializedObject.FindProperty("tabContentPanel");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_TabPanelProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

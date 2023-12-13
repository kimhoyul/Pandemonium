using UnityEditor;
using UnityEditor.UI;

namespace TOONIPLAY
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIScrollRect), true)]
    public class UIScrollRectInspector : ScrollRectEditor
    {
        SerializedProperty m_LeftEdgeProperty;
        SerializedProperty m_RightEdgeProperty;
        SerializedProperty m_TopEdgeProperty;
        SerializedProperty m_BottomEdgeProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_LeftEdgeProperty = serializedObject.FindProperty("_leftEdge");
            m_RightEdgeProperty = serializedObject.FindProperty("_rightEdge");
            m_TopEdgeProperty = serializedObject.FindProperty("_topEdge");
            m_BottomEdgeProperty = serializedObject.FindProperty("_bottomEdge");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_LeftEdgeProperty);
            EditorGUILayout.PropertyField(m_RightEdgeProperty);
            EditorGUILayout.PropertyField(m_TopEdgeProperty);
            EditorGUILayout.PropertyField(m_BottomEdgeProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

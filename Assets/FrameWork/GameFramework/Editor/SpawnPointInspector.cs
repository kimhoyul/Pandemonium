using UnityEditor;
using UnityEngine;

namespace TOONIPLAY
{
    [CustomEditor(typeof(SpawnPoint))]
    public class SpawnPointInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20f);
            GUILayout.Label("Custom Editor", EditorStyles.boldLabel);

            if (GUILayout.Button("Open Spawn Point Manager", GUILayout.Height(50f)))
            {
                SpawnPointManagerEditorWindow.Init();
            }
        }
    }
}
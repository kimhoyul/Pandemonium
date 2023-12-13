#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TOONIPLAY
{
    public class SpawnPoint : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var position = transform.position;

            Handles.color = Color.black;
            Handles.DrawWireDisc(position, Vector3.up, 1f + float.Epsilon);
            Handles.color = new Color(0, 1, 1, 0.1f);
            Handles.DrawSolidDisc(position, Vector3.up, 1f);
            Gizmos.color = new Color(0, 0, 0, 0f);
            Gizmos.DrawCube(position, new Vector3(2, float.Epsilon, 2));

            Handles.color = Color.black;
            Handles.ArrowHandleCap(0, position, transform.rotation, 1f, EventType.Repaint);
        }
#endif
    }
}

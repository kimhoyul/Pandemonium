using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TOONIPLAY
{
#if !ODIN_INSPECTOR
    [CustomEditor(typeof(UIComponent), true)]
    public class UIComponentInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //if (GUI.changed)
            //{
            //    (target as UIComponent).SerializeComponentMap();
            //}
        }
    }
#endif
}

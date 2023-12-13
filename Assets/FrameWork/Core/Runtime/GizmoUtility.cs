using System;
using UnityEngine;


namespace TOONIPLAY.Utilities
{
    public static class GizmoUtility
    {
        /// <summary>
        /// 기즈모를 그리고 색상을 복원한다.
        /// </summary>
        /// <param name="targetColor"></param>
        /// <param name="defaultColor"></param>
        /// <param name="callback"></param>
        public static void Draw(Color targetColor, Color defaultColor, Action callback)
        {
            if (Application.isPlaying)
            {
                Gizmos.color = targetColor;
                callback.Invoke();
                Gizmos.color = defaultColor;
            }
        }
    }
}
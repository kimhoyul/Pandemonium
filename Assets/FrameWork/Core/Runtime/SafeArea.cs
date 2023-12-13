using UnityEngine;

namespace TOONIPLAY
{
    public class SafeArea : MonoBehaviour
    {
        private void Awake()
        {
            var rectTr = GetComponent<RectTransform>();
            rectTr.anchorMin = Vector2.zero;
            rectTr.anchorMax = Vector2.one;

            rectTr.offsetMin = Vector2.zero;
            rectTr.offsetMax = Vector2.zero;
        }

        private void OnRectTransformDimensionsChange()
        {
            var safeAreaRect = Screen.safeArea;
            var anchorMin = safeAreaRect.position;
            var anchorMax = safeAreaRect.position + safeAreaRect.size;

            //Calculate anchorMin
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;

            //Calculate anchorMax
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            var rectTr = GetComponent<RectTransform>();

            //Apply anchor
            rectTr.anchorMin = anchorMin;
            rectTr.anchorMax = anchorMax;
        }
    }
}

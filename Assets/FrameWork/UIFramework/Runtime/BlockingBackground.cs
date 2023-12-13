using UnityEngine;

namespace TOONIPLAY
{
    public class BlockingBackground : MonoBehaviour
    {
        [SerializeField] private RectTransform contentTrans;

        [SerializeField] private bool isManualHide;

        public bool IsManualHide { get { return isManualHide; } set { isManualHide = value; } }

        public Transform ContentTrans => contentTrans.gameObject.transform;

        public void SetContentRect(int left, int top, int right, int bottom)
        {
            if (contentTrans != null)
            {
                contentTrans.offsetMin = new Vector2(left, bottom);
                contentTrans.offsetMax = new Vector2(right, top);
            }
        }

        public void SetActive(bool value)
        {
            if (value || !isManualHide)
                gameObject.SetActive(value);
        }

        public void SetForceActive(bool value) => gameObject.SetActive(value);

        private void OnDisable()
        {
            InitOffset();
        }

        private void InitOffset()
        {
            contentTrans.offsetMin = Vector2.zero;
            contentTrans.offsetMax = Vector2.zero;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace TOONIPLAY
{
    public class UIContextComponent : UIComponent
    {
        [Header("Context Property")]
        [SerializeField]
        private bool stackable;

        [SerializeField]
        protected bool cancelable;

        [SerializeField]
        private bool needBlockingBackground;

        [SerializeField]
        private Button closeButton;

        public bool Stackable => stackable;
        public bool Cancelable => cancelable;
        public bool NeedBlockingBackground => needBlockingBackground;

        protected override void OnAwake()
        {
            base.OnAwake();

            if (closeButton != null)
                closeButton.onClick.AddListener(OnClickClose);
        }

        protected virtual void OnClose() { }

        protected override void OnShow(bool isShow)
        {
            base.OnShow(isShow);

            if (!isShow)
                OnClose();
        }

        public virtual void OnClickClose()
        {
            UIManager.Instance.CloseContextComponent();
        }

        public virtual void OnCanceled() { }
    }
}

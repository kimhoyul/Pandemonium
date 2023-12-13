using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TOONIPLAY
{
    public class UILongToggle : UIToggle
    {
        private bool held;
        public UnityEvent onLongPressed = new();

        public override void OnPointerDown(PointerEventData eventData)
        {
            held = false;
            Invoke("OnLongPress", 1.0f);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            CancelInvoke("OnLongPress");
        }
    
        public override void OnPointerExit(PointerEventData eventData)
        {
            CancelInvoke("OnLongPress");
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (held)
                return;
            base.OnPointerClick(eventData);
        }

        void OnLongPress()
        {
            held = true;
            onLongPressed.Invoke();
        }
    }
}

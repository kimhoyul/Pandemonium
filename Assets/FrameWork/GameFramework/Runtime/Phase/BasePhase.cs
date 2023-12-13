using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TOONIPLAY
{
    [DisallowMultipleComponent]
    public class BasePhase : MonoBehaviour
    {
        protected UIController uiController;

        public UIController UIController => uiController;

        public UIController GetUIController()
        {
            return uiController;
        }

        public T GetUIController<T>() where T : UIController
        {
            return uiController as T;
        }

        private void Awake()
        {
            uiController = GetComponent<UIController>();

            OnAwake();
        }

        protected virtual void OnAwake() { }

        public virtual void OnLoadedPhase(object sceneParams) { }
    }
}

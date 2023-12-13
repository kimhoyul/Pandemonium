using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TOONIPLAY
{
    [RequireComponent(typeof(BasePhase))]
    public class UIController : UIComponent
    {
        protected BasePhase ownedPhase;

        protected override void OnAwake() => ownedPhase = GetComponent<BasePhase>();
    }
}

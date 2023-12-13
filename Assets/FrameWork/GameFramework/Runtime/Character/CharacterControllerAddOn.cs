using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TOONIPLAY
{
    public abstract class CharacterControllerAddOn : MonoBehaviour
    {
        protected TooniCharacter Owner;

        private void Awake()
        {
            Owner = GetComponent<TooniCharacter>();
        }
    }
}

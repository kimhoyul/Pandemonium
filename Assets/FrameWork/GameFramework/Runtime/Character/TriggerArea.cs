using UnityEngine;

namespace TOONIPLAY
{
    public class TriggerArea : MonoBehaviour
    {
        public InteractableObject receiverObject;

        private void OnTriggerEnter(Collider other)
        {
            if (receiverObject != null)
                receiverObject.SendMessage("OnTriggerEnter", other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (receiverObject != null)
                receiverObject.SendMessage("OnTriggerExit", other);
        }
    }
}

using Plugins.Outline;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TOONIPLAY
{
    public class InteractableObject : EventTrigger
    {
        public static int LAYER_INTERACTABLE_OBJECT = 0;

        [SerializeField] private string interactionUid;

        public string InteractionUid
        {
            get => interactionUid;
            set => interactionUid = value;
        }

        [SerializeField]
        private OutlineBehaviour selectionOutline;

        [SerializeField] private bool enterToInteract;
        [SerializeField] private bool isTransient;

        public OutlineBehaviour SelectionOutline => selectionOutline;

        public bool IsTransient => isTransient;

        public virtual bool IsInteractable => true;

        protected virtual void Awake()
        {
            if (LAYER_INTERACTABLE_OBJECT == 0)
            {
                LAYER_INTERACTABLE_OBJECT = 1 << LayerMask.NameToLayer("InteractableObject");
                LAYER_INTERACTABLE_OBJECT = 1 << LayerMask.NameToLayer("BoardNote");
            }

            if (selectionOutline != null)
                selectionOutline.enabled = false;
        }

        private void OnEnable()
        {
            Select(false);
        }

        public void Select(bool select)
        {
            if (selectionOutline != null)
                selectionOutline.enabled = select;

            OnSelect(select);
        }

        protected virtual void OnSelect(bool select) { }

        public virtual void Interact(TooniCharacter characterController) { }

        public virtual void StopInteract(TooniCharacter characterController) { }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<TooniCharacter>(out var characterController))
                return;
            
            var playerController = CharacterManager.Instance.GetMainCharacterController();
            if (characterController != playerController)
                return;
            
            if (enterToInteract)
            {
                if (playerController != null)
                    Interact(playerController);
            }
            else
            {
                var characterInteraction = characterController.GetAddonComponent<CharacterInteraction>();
                if (characterInteraction != null)
                    characterInteraction.RegisterInteractableObject(this);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<TooniCharacter>(out var characterController))
                return;
            
            if (characterController != CharacterManager.Instance.GetMainCharacterController())
                return;
            
            var characterInteraction = characterController.GetAddonComponent<CharacterInteraction>();
            if (characterInteraction != null)
                characterInteraction.UnregisterInteractableObject(this);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (enterToInteract)
                return;

            var targetCharacter = CharacterManager.Instance.GetMainCharacterController();
            if (targetCharacter == null)
                return;

            var characterInteraction = targetCharacter.GetAddonComponent<CharacterInteraction>();
            if (characterInteraction != null)
                characterInteraction.SetPickedInteractableObject(this);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (enterToInteract)
                return;

            var targetCharacter = CharacterManager.Instance.GetMainCharacterController();
            if (targetCharacter == null)
                return;

            var characterInteraction = targetCharacter.GetAddonComponent<CharacterInteraction>();
            if (characterInteraction != null)
                characterInteraction.Interact();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (enterToInteract)
                return;

            var targetCharacter = CharacterManager.Instance.GetMainCharacterController();
            if (targetCharacter == null)
                return;

            var characterInteraction = targetCharacter.GetAddonComponent<CharacterInteraction>();
            if (characterInteraction != null)
                characterInteraction.SetPickedInteractableObject(null);
        }
    }
}

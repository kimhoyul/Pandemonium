using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TOONIPLAY
{
    public class CharacterInteraction : CharacterControllerAddOn
    {
        private readonly List<InteractableObject> _interactableObjects = new(10);

        private InteractableObject _pickedInteractableObject;
        private InteractableObject _selectedInteractableObject;
        private InteractableObject _interactingInteractableObject;

        private Coroutine _updateInteractableObjects;

        public bool IsInteracting => _interactingInteractableObject != null;

        public string InteractingUid => _interactingInteractableObject == null ? null : _interactingInteractableObject.InteractionUid;

        public void Interact()
        {
            var mainCharacterController = CharacterManager.Instance.GetMainCharacterController();
            if (mainCharacterController == null || gameObject != mainCharacterController.gameObject)
                return;

            if (_interactingInteractableObject != null)
            {
                // 선택한 InteractableObject가 단발성이 아닐 때만 취소한다.
                // 즉, 선택한 InteractableObject가 단발성이라면 기존의 InteractableObject의 상호작용 상태를 취소하지 않고 그대로 둔다.

                if (_pickedInteractableObject != null)
                {
                    if (!_pickedInteractableObject.IsTransient)
                    {
                        _interactingInteractableObject.StopInteract(Owner);
                        _interactingInteractableObject = null;
                    }
                }
                else if (_selectedInteractableObject != null)
                {
                    if (!_selectedInteractableObject.IsTransient)
                    {
                        _interactingInteractableObject.StopInteract(Owner);
                        _interactingInteractableObject = null;
                    }
                }

                // 다른 형식이 추가될 때마다 조건을 추가해줘야 한다.
                // ※ 다른 형식의 interactableObject가 늘어날 수록, if-else 문이 거대해진다.
            }

            if (_pickedInteractableObject != null)
            {
                _pickedInteractableObject.Interact(Owner);

                // 즉발성의 InteractableObject은 현재 상호작용 중인 InteractableObject로 취급하지 않는다.
                // 예) 의자에 앉아서, 스크린을 누른다. 이때 상호작용 중인 InteractableObject는 의자에 앉아 있는 것이다.

                if (!_pickedInteractableObject.IsTransient)
                {
                    _interactingInteractableObject = _pickedInteractableObject;

                    if (gameObject == mainCharacterController.gameObject)
                        _interactingInteractableObject.Select(false);
                }
            }
            else if (_selectedInteractableObject != null)
            {
                _selectedInteractableObject.Interact(Owner);

                if (!_selectedInteractableObject.IsTransient)
                {
                    _interactingInteractableObject = _selectedInteractableObject;

                    if (gameObject == mainCharacterController.gameObject)
                        _interactingInteractableObject.Select(false);
                }
            }
        }

        public void StopInteract()
        {
            if (_interactingInteractableObject != null)
            {
                _interactingInteractableObject.StopInteract(Owner);
                _interactingInteractableObject = null;
            }
        }

        public void RegisterInteractableObject(InteractableObject interactableObject)
        {
            if (!_interactableObjects.Contains(interactableObject))
                _interactableObjects.Add(interactableObject);

            _updateInteractableObjects ??= StartCoroutine(CoUpdateInteractableObjects());
        }

        public void SetPickedInteractableObject(InteractableObject interactableObject)
        {
            if (interactableObject != null && _interactingInteractableObject == interactableObject)
                return;

            if (interactableObject != null && !interactableObject.IsInteractable)
                return;

            if (_pickedInteractableObject != null)
                _pickedInteractableObject.Select(false);

            _pickedInteractableObject = interactableObject;

            if (_pickedInteractableObject != null)
            {
                ChangeSelectedInteractableObject(null);
                _pickedInteractableObject.Select(true);
            }
            else
            {
                ChangeSelectedInteractableObject(CalcCandidateInteractableObject());
            }
        }

        public void UnregisterInteractableObject(InteractableObject interactableObject)
        {
            var mainCharacterController = CharacterManager.Instance.GetMainCharacterController();
            if (mainCharacterController == null || gameObject != mainCharacterController.gameObject)
                return;

            interactableObject.Select(false);
            _interactableObjects.Remove(interactableObject);

            if (_interactableObjects.Count == 0 && _updateInteractableObjects != null)
            {
                StopCoroutine(_updateInteractableObjects);

                ChangeSelectedInteractableObject(null);
                _updateInteractableObjects = null;
            }
        }

        private IEnumerator CoUpdateInteractableObjects()
        {
            while (_interactableObjects.Count > 0)
            {
                if (_interactingInteractableObject == null && _pickedInteractableObject == null)
                    ChangeSelectedInteractableObject(CalcCandidateInteractableObject());

                yield return new WaitForSeconds(0.5f);
            }

            ChangeSelectedInteractableObject(null);
            _updateInteractableObjects = null;
        }

        private InteractableObject CalcCandidateInteractableObject()
        {
            InteractableObject candidateObject = null;
            var rate = float.MaxValue;

            var cameraMain = Camera.main;
            if (cameraMain == null)
                return null;

            var cameraForward = cameraMain.transform.forward;
            var position = gameObject.transform.position;
            
            foreach (var interactableObject in _interactableObjects)
            {
                if (!interactableObject.IsInteractable)
                    continue;

                var objectTrans = interactableObject.transform;
                
                var length = (objectTrans.position - position).sqrMagnitude;
                var angle = Vector3.Dot(objectTrans.forward, cameraForward);

                var tempRate = (Mathf.Clamp(length, 0.0f, 25.0f) / 25.0f) * 0.5f + ((Mathf.Clamp(angle, -1.0f, -0.5f) + 1.0f) / 0.5f) * 0.5f;
                if (!(tempRate < rate))
                    continue;

                candidateObject = interactableObject;
                rate = tempRate;
            }

            return _interactingInteractableObject == candidateObject ? null : candidateObject;
        }

        private void ChangeSelectedInteractableObject(InteractableObject candidateObject)
        {
            if (_selectedInteractableObject == candidateObject)
                return;

            var mainCharacterController = CharacterManager.Instance.GetMainCharacterController();
            if (mainCharacterController == null || gameObject != mainCharacterController.gameObject)
                return;

            if (_selectedInteractableObject != null)
                _selectedInteractableObject.Select(false);

            _selectedInteractableObject = candidateObject;

            if (_selectedInteractableObject != null)
                _selectedInteractableObject.Select(true);
        }
    }
}

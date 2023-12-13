using EasyCharacterMovement;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace TOONIPLAY
{
    public class TooniCharacter : Character
    {
        [HideInInspector]
        public IPeerHandler parentPeer;

        [HideInInspector]
        public GameObject characterModel;

        protected string uid;

        protected CharacterController controller;
        protected InputHandler inputHandler;

        public InputHandler InputHandler => inputHandler;
        public virtual void SetAction(Packets.ActionCmd actionCmd, Vector3 position, Quaternion rotation, float speed) { }

        NavMeshAgent _agent;
        protected NavMeshAgent agent => _agent ??= GetComponent<NavMeshAgent>();

        public float moveSpeed
        {
            get => maxWalkSpeed;
            set
            {
                maxWalkSpeed = value;
                if (agent != null)
                    agent.speed = maxWalkSpeed;
                // agent.speed와 maxWalkSpeed의 실제 이동속도가 다르다.
                // agent.speed와 maxWalkSpeed 간의 비례식을 구해야 한다.
                // 비례식에 의거하여 비례 상수를 곱해준다.
            }
        }

        private Vector3 _skillStartPoint;
        public Vector3 skillStartPoint
        {
            get => _skillStartPoint;
            set
            {
                _skillStartPoint = value;
            }
        }

        public enum InputType
        {
            None,
            Human,
            AI,
            Network,
        }

        public T GetAddonComponent<T>() where T : CharacterControllerAddOn
        {
            return GetComponent<T>();
        }

        private InputType _currentInputType;
        //private InputActionAsset _cacheInputActions;


        public InputType CurrentInputType
        {
            get => _currentInputType;
            set
            {
                _currentInputType = value;

                if (_currentInputType == InputType.Human)
                {
                    //inputActions = _cacheInputActions;
                    if (movementInputAction == null)
                        InitCustomPlayerInput();
                }
                else if (_currentInputType == InputType.Network)
                {
                    inputActions = null;
                    if (movementInputAction != null)
                        DeinitPlayerInput();
                }
            }
        }
        private Animator _baseAnimator;

        protected new Animator animator
        {
            get
            {
                if (_baseAnimator == null)
                    _baseAnimator = GetComponentInChildren<Animator>();

                return _baseAnimator;
            }
        }

        private void InitCustomPlayerInput()
        {
            // Call base method

            base.InitPlayerInput();
        }
    }
}


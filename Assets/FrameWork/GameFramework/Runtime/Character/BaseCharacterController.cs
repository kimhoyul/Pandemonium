using System;
using EasyCharacterMovement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TOONIPLAY
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class BaseCharacterController : Character
    {
        [HideInInspector]
        public IPeerHandler parentPeer;

        [HideInInspector]
        public GameObject characterModel;
        [HideInInspector]
        public int hairUID;
        public int topUID;
        public int bottomUID;
        [HideInInspector]
        public bool isRandom;

        [HideInInspector]
        public GameObject chattingText = null;
        [HideInInspector]
        public GameObject emoji = null;

        [HideInInspector]
        public int displayChattingCurTime = 0;

        protected string uid;
        protected CharacterController controller;
        protected InputHandler inputHandler;

        [SerializeField]
        protected CameraTargetHandler cameraTargetHandler;

        public CameraTargetHandler CameraTargetHandler => cameraTargetHandler;

        public InputHandler InputHandler => inputHandler;

        public T GetAddonComponent<T>() where T : CharacterControllerAddOn
        {
            return (T)GetComponent<T>();
        }

        public virtual void SetAction(Packets.ActionCmd actionCmd, Vector3 position, Quaternion rotation, float speed) { }

        public virtual void EnableConnectionRange(bool value) { }



        // ===================================== New =============================================================================
        public enum InputType
        {
            None,
            Human,
            AI,
            Network,
        }

        private InputType _currentInputType;
        private InputActionAsset _cacheInputActions;

        public InputType CurrentInputType
        {
            get => _currentInputType;
            set
            {
                _currentInputType = value;
                
                if (_currentInputType == InputType.Human)
                {
                    inputActions = _cacheInputActions;
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
        public Transform cameraTarget;

        [Space(15f)]
        [Tooltip("Should the agent brake automatically to avoid overshooting the destination point? \n" +
         "If true, the agent will brake automatically as it nears the destination.")]
        [SerializeField]
        private bool _autoBraking = true;

        [Tooltip("Distance from target position to start braking.")]
        [SerializeField]
        private float _brakingDistance;

        [Tooltip("Stop within this distance from the target position.")]
        [SerializeField]
        private float _stoppingDistance = 1.0f;

        private float _pitch;
        private float _yaw;

        private CharacterLook _characterLook;

        private NavMeshAgent _agent;


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

        /// <summary>
        /// Cached CharacterLook component.
        /// </summary>

        protected CharacterLook characterLook
        {
            get
            {
                if (_characterLook == null)
                    _characterLook = GetComponent<CharacterLook>();

                return _characterLook;
            }
        }

        /// <summary>
        /// Cached NavMeshAgent component.
        /// </summary>

        protected NavMeshAgent agent
        {
            get
            {
                if (_agent == null)
                    _agent = GetComponent<NavMeshAgent>();

                return _agent;
            }
        }

        /// <summary>
        /// Should the agent brake automatically to avoid overshooting the destination point?
        /// If this property is set to true, the agent will brake automatically as it nears the destination.
        /// </summary>

        public bool autoBraking
        {
            get => _autoBraking;
            set
            {
                _autoBraking = value;

                agent.autoBraking = _autoBraking;
            }
        }

        /// <summary>
        /// Distance from target position to start braking.
        /// </summary>

        public float brakingDistance
        {
            get => _brakingDistance;
            set => _brakingDistance = Mathf.Max(0.0001f, value);
        }

        /// <summary>
        /// The ratio (0 - 1 range) of the agent's remaining distance and the braking distance.
        /// 1 If no auto braking or if agent's remaining distance is greater than brakingDistance.
        /// less than 1, if agent's remaining distance is less than brakingDistance.
        /// </summary>

        public float brakingRatio
        {
            get
            {
                if (!autoBraking)
                    return 1f;

                return agent.hasPath ? Mathf.Clamp(agent.remainingDistance / brakingDistance, 0.1f, 1f) : 1f;
            }
        }

        /// <summary>
        /// Stop within this distance from the target position.
        /// </summary>

        public float stoppingDistance
        {
            get => _stoppingDistance;
            set
            {
                _stoppingDistance = Mathf.Max(0.0f, value);

                if (agent != null)
                    agent.stoppingDistance = _stoppingDistance;
            }
        }

        /// <summary>
        /// Mouse Look InputAction.
        /// </summary>

        protected InputAction mouseLookInputAction { get; set; }

        private Vector2 lookInputValue;

        public void SetMouseLookInput(Vector2 lookInputValue)
        {
            this.lookInputValue = lookInputValue;
        }

        /// <summary>
        /// Gets the mouse look value.
        /// Return its current value or zero if no valid InputAction found.
        /// </summary>

        protected virtual Vector2 GetMouseLookInput()
        {
            return lookInputValue.sqrMagnitude > 0.005f ? lookInputValue : mouseLookInputAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        public delegate void ArrivedEventHandler();

        /// <summary>
        /// Event triggered when agent reaches its destination.
        /// </summary>

        public event ArrivedEventHandler Arrived;

        /// <summary>
        /// Trigger Arrived event.
        /// Called when agent reaches its destination.
        /// </summary>

        public virtual void OnArrive()
        {
            Arrived?.Invoke();
        }

        /// <summary>
        /// Called after MovementMode has changed.
        /// Does special handling for starting certain modes, eg: enable / disable ground constraint, etc.
        /// If overridden, must call base.OnMovementModeChanged.
        /// </summary>

        protected override void OnMovementModeChanged(MovementMode prevMovementMode, int prevCustomMode)
        {
            // Call base implementation

            base.OnMovementModeChanged(prevMovementMode, prevCustomMode);

            if (prevMovementMode == MovementMode.Falling)
                StopJumping();

            // If movement mode has ben changed to None (eg: disabled movement),
            // stop path following movement

            if (IsDisabled())
                StopMovement();
        }

        /// <summary>
        /// Return the NavMeshAgent component. This is guaranteed to be not null.
        /// </summary>

        public virtual NavMeshAgent GetNavMeshAgent()
        {
            return agent;
        }

        /// <summary>
        /// Synchronize the NavMeshAgent with Character (eg: speed, acceleration, velocity, etc) as we moves the Agent.
        /// Called on OnLateFixedUpdate.
        /// </summary>

        protected virtual void SyncNavMeshAgent()
        {
            agent.angularSpeed = rotationRate;

            agent.speed = GetMaxSpeed();
            agent.acceleration = GetMaxAcceleration();

            agent.velocity = GetVelocity();
            agent.nextPosition = GetPosition();

            agent.radius = characterMovement.radius;
            agent.height = IsCrouching() ? crouchedHeight : unCrouchedHeight;
        }

        /// <summary>
        /// Does the Agent currently has a path?
        /// </summary>

        public virtual bool HasPath()
        {
            return agent.hasPath;
        }

        /// <summary>
        /// True if Agent is following a path, false otherwise.
        /// </summary>

        public virtual bool IsPathFollowing()
        {
            return agent.hasPath && !agent.isStopped;
        }

        /// <summary>
        /// Issue the Agent to move to desired location (in world space). 
        /// </summary>

        public virtual void MoveToLocation(Vector3 location)
        {
            Vector3 toLocation = (location - GetPosition()).projectedOnPlane(GetUpVector());
            if (toLocation.sqrMagnitude >= MathLib.Square(stoppingDistance))
                agent.SetDestination(location);
        }

        /// <summary>
        /// Halts Character's current path following movement.
        /// </summary>

        public virtual void StopMovement()
        {
            agent.ResetPath();

            SetMovementDirection(Vector3.zero);
        }

        /// <summary>
        /// Makes the character's follow Agent's path (if any).
        /// Eg: Keep updating Character's movement direction vector to steer towards Agent's destination until reached.
        /// </summary>

        protected virtual void PathFollowing()
        {
            // Is movement is disabled, return

            if (IsDisabled())
                return;

            // If agent has no path or not following it (eg: paused), return

            if (!IsPathFollowing())
                return;

            // Is destination reached ?

            if (agent.remainingDistance <= stoppingDistance)
            {
                // Destination is reached, stop movement

                StopMovement();

                // Trigger event

                OnArrive();
            }
            else
            {
                // If destination not reached, feed agent's desired velocity (lateral only) as the character move direction

                // Do not allow to move at a speed less than minAnalogWalkSpeed

                Vector3 planarDesiredVelocity = agent.desiredVelocity.projectedOnPlane(GetUpVector()) * brakingRatio;

                if (planarDesiredVelocity.sqrMagnitude < MathLib.Square(minAnalogWalkSpeed))
                    planarDesiredVelocity = planarDesiredVelocity.normalized * minAnalogWalkSpeed;

                SetMovementDirection(planarDesiredVelocity.normalized * ComputeAnalogInputModifier(planarDesiredVelocity));
            }
        }

        /// <summary>
        /// Extends Move method to handle PathFollowing state.
        /// </summary>

        protected override void Move()
        {
            // Handle PathFollowing state

            PathFollowing();

            // Call base implementation (e.g. Default movement modes and states).

            base.Move();
        }

        /// <summary>
        /// Our Reset method. Set this default values.
        /// If overridden, must call base method in order to fully initialize the class.
        /// </summary>

        protected override void OnReset()
        {
            // Base class defaults

            base.OnReset();

            // This defaults

            autoBraking = true;

            brakingDistance = 2.0f;
            stoppingDistance = 1.0f;
        }

        /// <summary>
        /// Our OnValidate method.
        /// If overridden, must call base method in order to fully initialize the class.
        /// </summary>

        protected override void OnOnValidate()
        {
            // Validate base class

            base.OnOnValidate();

            // Validate this editor exposed fields

            brakingDistance = _brakingDistance;
            stoppingDistance = _stoppingDistance;
        }

        /// <summary>
        /// Called when the script instance is being loaded (Awake).
        /// If overridden, must call base method in order to fully initialize the class.
        /// </summary>

        protected override void OnAwake()
        {
            // Init base class

            base.OnAwake();

            // Initialize NavMeshAgent

            agent.autoBraking = autoBraking;
            agent.stoppingDistance = stoppingDistance;

            // Turn-off NavMeshAgent control, we control it, not the other way

            agent.updatePosition = false;
            agent.updateRotation = false;

            agent.updateUpAxis = false;

            _cacheInputActions = inputActions;
        }

        /// <summary>
        /// Rotate the camera along its yaw.
        /// </summary>

        public void AddCameraYawInput(float value)
        {
            _yaw = MathLib.Clamp0360(_yaw + value);
        }

        /// <summary>
        /// Rotate the camera along its pitch.
        /// </summary>

        public void AddCameraPitchInput(float value)
        {
            value = characterLook.invertLook ? value : -value;

            _pitch = Mathf.Clamp(_pitch + value, characterLook.minPitchAngle, characterLook.maxPitchAngle);
        }

        protected override void OnCrouch(InputAction.CallbackContext context)
        {
            var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (currentSelectedGameObject != null && currentSelectedGameObject.GetComponent<TMP_InputField>() != null)
                return;

            base.OnCrouch(context);
        }

        protected override void OnJump(InputAction.CallbackContext context)
        {
            var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (currentSelectedGameObject != null && currentSelectedGameObject.GetComponent<TMP_InputField>() != null)
                return;

            base.OnJump(context);
        }

        /// <summary>
        /// Extends HandleInput method to add camera input.
        /// </summary>

        protected override void HandleInput()
        {
            // Call base method
            var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (currentSelectedGameObject != null && currentSelectedGameObject.GetComponent<TMP_InputField>() != null)
                return;

            base.HandleInput();

            // Camera input (mouse look),
            // Rotates the camera target independently of the Character's rotation,
            // basically we are manually rotating the Cinemachine camera here

            if (IsDisabled())
                return;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
#else
            if (Mouse.current.rightButton.isPressed)
#endif
            {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
#else
                characterLook.LockCursor();
#endif

                Vector2 mouseLookInput = GetMouseLookInput();

                if (mouseLookInput.x != 0.0f)
                    AddCameraYawInput(mouseLookInput.x * characterLook.mouseHorizontalSensitivity);

                if (mouseLookInput.y != 0.0f)
                    AddCameraPitchInput(mouseLookInput.y * characterLook.mouseVerticalSensitivity);
            }
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
#else
            else
            {
                characterLook.UnlockCursor();
            }
#endif
        }

        protected override void OnLateUpdate()
        {
            // Move Agent with us

            SyncNavMeshAgent();

            // Set final camera rotation
            if (CurrentInputType != InputType.Network)
            {
                if (CameraManager.Instance.isLetterRoad)
                {
                    if (cameraTarget.transform.eulerAngles.x > 29 && cameraTarget.transform.eulerAngles.x < 31)
                        cameraTarget.transform.rotation = Quaternion.Euler(30, _yaw, 0.0f);
                    else if (cameraTarget.transform.eulerAngles.x > 180)
                        cameraTarget.transform.eulerAngles = new Vector3(cameraTarget.transform.eulerAngles.x + 0.5f, _yaw, 0.0f);
                    else if (cameraTarget.transform.eulerAngles.x < 30)
                        cameraTarget.transform.eulerAngles = new Vector3(cameraTarget.transform.eulerAngles.x + 0.5f, _yaw, 0.0f);
                    else if (cameraTarget.transform.eulerAngles.x <= 180 && cameraTarget.transform.eulerAngles.x > 30)
                        cameraTarget.transform.eulerAngles = new Vector3(cameraTarget.transform.eulerAngles.x - 0.5f, _yaw, 0.0f);
                }
                else
                {
                    cameraTarget.rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
                }
            }
        }

        public void InitCustomPlayerInput()
        {
            // Call base method

            base.InitPlayerInput();

            // Setup input action handlers

            mouseLookInputAction = inputActions.FindAction("Mouse Look");
            mouseLookInputAction?.Enable();
        }

        private Vector2 movementInputValue;

        public virtual void SetMovementInput(Vector2 movementInputValue)
        {
            this.movementInputValue = movementInputValue;
        }

        protected override Vector2 GetMovementInput()
        {
            return movementInputValue.sqrMagnitude > 0.005f ? movementInputValue : base.GetMovementInput();
        }

        protected override void InitPlayerInput()
        {
            // Do Nothing
        }

        protected override void DeinitPlayerInput()
        {
            // Call base method

            base.DeinitPlayerInput();

            if (mouseLookInputAction != null)
            {
                mouseLookInputAction.Disable();
                mouseLookInputAction = null;
            }
        }

        /// <summary>
        /// Overrides OnStart to initialize this.
        /// </summary>

        protected override void OnStart()
        {
            // Call base method

            base.OnStart();

            // Cache camera's initial orientation (yaw / pitch)

            Vector3 cameraTargetEulerAngles = cameraTarget.eulerAngles;

            _pitch = cameraTargetEulerAngles.x;
            _yaw = cameraTargetEulerAngles.y;
        }

        public void SetCamera(Camera camera)
        {
            this.camera = camera;
        }

        public override Animator GetAnimator()
        {
            return animator;
        }

        public void SetAnimator(Animator animator)
        {
            _baseAnimator = animator;
        }
    }
}

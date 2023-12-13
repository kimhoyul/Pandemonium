using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace EasyCharacterMovement.Examples.Cinemachine.ThirdPersonExample
{
    /// <summary>
    /// This example shows how to extend the Character class to implement a Cinemachine based third person camera.
    ///
    /// This uses the Cinemachine 3rd person follow method to implement the third person camera.
    /// </summary>

    public class CMThirdPersonCharacter : Character
    {
        #region EDITOR EXPOSED FIELDS

        public Transform cameraTarget;

        #endregion

        #region FIELDS

        private float _pitch;
        private float _yaw;

        private CharacterLook _characterLook;

        #endregion

        #region PROPERTIES

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

        #endregion

        #region INPUT ACTIONS

        /// <summary>
        /// Mouse Look InputAction.
        /// </summary>

        protected InputAction mouseLookInputAction { get; set; }

        /// <summary>
        /// Cursor lock InputAction.
        /// </summary>

        protected InputAction cursorLockInputAction { get; set; }

        /// <summary>
        /// Cursor unlock InputAction.
        /// </summary>

        protected InputAction cursorUnlockInputAction { get; set; }

        #endregion

        #region INPUT ACTION HANDLERS

        /// <summary>
        /// Gets the mouse look value.
        /// Return its current value or zero if no valid InputAction found.
        /// </summary>

        protected virtual Vector2 GetMouseLookInput()
        {
            return mouseLookInputAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        /// <summary>
        /// Cursor lock input action handler.
        /// </summary>

        protected virtual void OnCursorLock(InputAction.CallbackContext context)
        {
            // Do not allow to lock cursor if using UI

            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
                return;

            if (context.started)
                characterLook.LockCursor();
        }

        /// <summary>
        /// Cursor unlock input action handler.
        /// </summary>

        protected virtual void OnCursorUnlock(InputAction.CallbackContext context)
        {
            if (context.started)
                characterLook.UnlockCursor();
        }

        #endregion

        #region METHODS

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

        /// <summary>
        /// Extends HandleInput method to add camera input.
        /// </summary>

        protected override void HandleInput()
        {
            // Call base method

            base.HandleInput();

            // Camera input (mouse look),
            // Rotates the camera target independently of the Character's rotation,
            // basically we are manually rotating the Cinemachine camera here

            if (IsDisabled())
                return;

            Vector2 mouseLookInput = GetMouseLookInput();

            if (mouseLookInput.x != 0.0f)
                AddCameraYawInput(mouseLookInput.x * characterLook.mouseHorizontalSensitivity);

            if (mouseLookInput.y != 0.0f)
                AddCameraPitchInput(-mouseLookInput.y * characterLook.mouseVerticalSensitivity);
        }

        protected override void OnLateUpdate()
        {
            // Call base method

            base.OnLateUpdate();

            // Set final camera rotation

            cameraTarget.rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
        }

        protected override void InitPlayerInput()
        {
            // Call base method

            base.InitPlayerInput();

            // Setup input action handlers

            mouseLookInputAction = inputActions.FindAction("Mouse Look");
            mouseLookInputAction?.Enable();

            cursorLockInputAction = inputActions.FindAction("Cursor Lock");
            if (cursorLockInputAction != null)
            {
                cursorLockInputAction.started += OnCursorLock;
                cursorLockInputAction.Enable();
            }

            cursorUnlockInputAction = inputActions.FindAction("Cursor Unlock");
            if (cursorUnlockInputAction != null)
            {
                cursorUnlockInputAction.started += OnCursorUnlock;
                cursorUnlockInputAction.Enable();
            }
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

            if (cursorLockInputAction != null)
            {
                cursorLockInputAction.started -= OnCursorLock;

                cursorLockInputAction.Disable();
                cursorLockInputAction = null;
            }

            if (cursorUnlockInputAction != null)
            {
                cursorUnlockInputAction.started -= OnCursorUnlock;

                cursorUnlockInputAction.Disable();
                cursorUnlockInputAction = null;
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

        #endregion
    }
}

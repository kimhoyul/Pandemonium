using UnityEngine;
using UnityEngine.InputSystem;

namespace TOONIPLAY
{
	public class HumanInputHandler : BaseInputHandler
	{
		[Header("Character Input Values")]
		private Vector2 _move;
		private Vector2 _look;
		private bool _jump;
		private bool _sprint;

		[Header("Movement Settings")]
		private bool _analogMovement;

        [SerializeField] private PlayerInput playerInput;

		public override InputHandler.InputType InputType => InputHandler.InputType.Human;

		public override bool IsCurrentDeviceMouse => playerInput.currentControlScheme == "KeyboardMouse";

		public override Vector2 Look => _look;

		public override Vector3 Velocity {
			get {
				var length = _move.magnitude;
				var d = _move.normalized;
				var targetRotation = Mathf.Atan2(d.x, d.y) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;

				return Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward * length;
			}
		}

		public override bool Sprint => _sprint;

        public override bool AnalogMovement => _analogMovement;

		private GameObject _mainCamera;

        private void Awake()
        {
			playerInput.onActionTriggered += OnActionTriggered;
			playerInput.onDeviceLost += OnDeviceLost;
			playerInput.onDeviceRegained += OnDeviceRegained;

			//playerInput.uiInputModule = UIManager.Instance.GetComponentInChildren<InputSystemUIInputModule>();

            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void OnEnable()
        {
            playerInput.enabled = true;
            
			InputManager.Instance.OnToggleInterface += OnToggleInterface;
        }

        private void OnDisable()
        {
            InputManager.Instance.OnToggleInterface -= OnToggleInterface;

            playerInput.enabled = false;
        }

        private void OnDestroy()
        {
            if (playerInput != null)
			{
                playerInput.onActionTriggered -= OnActionTriggered;
				playerInput.onDeviceLost -= OnDeviceLost;
				playerInput.onDeviceRegained -= OnDeviceRegained;
            }
        }

        public override void OnInitialize(InputHandler rootHandler)
        {
            base.OnInitialize(rootHandler);

            playerInput.camera = Camera.main;

			rootHandler.obstacleAgent.SetEnableAgent(false);

			SetEnableActionMode(true);
        }

        private void OnActionTriggered(InputAction.CallbackContext context)
		{
			if (!context.performed)
				return;

			switch (context.action.name)
			{
				case "Move":
					OnMove(context);
                    break;

				case "Look":
					OnLook(context);

                    break;

				case "Jump":
					OnJump(context);
                    break;

				case "Sprint":
					OnSprint(context);
                    break;
			}
		}

		private void OnDeviceLost(PlayerInput playerInput)
		{

		}

		private void OnDeviceRegained(PlayerInput playerInput)
		{

		}

        public override bool GetJump()
        {
            return _jump;
        }

        public override void SetJump(bool jump)
        {
			this._jump = jump;
		}

        public override void SetMove(Vector2 move)
        {
            this._move = move;
        }

        public void OnMove(InputAction.CallbackContext context)
		{
			_move = context.ReadValue<Vector2>();
		}

		public void OnLook(InputAction.CallbackContext context)
		{
            _look = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
		{
            Debug.Log("[InputSystem] OnJump");
            SetJump(context.ReadValueAsButton());
        }

        public void OnSprint(InputAction.CallbackContext context)
		{
            _sprint = context.ReadValueAsButton();
        }
		
		private void OnToggleInterface(InputAction.CallbackContext context) => SetEnableActionMode(string.Equals(playerInput.currentActionMap.name, "UIMode"));

		public void SetEnableActionMode(bool enable)
		{
            if (enable)
            {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
				playerInput.SwitchCurrentActionMap("TouchMode");
#else
                playerInput.SwitchCurrentActionMap("ConsoleMode");
#endif
            }
            else
            {
                playerInput.SwitchCurrentActionMap("UIMode");
            }
        }
    }
}

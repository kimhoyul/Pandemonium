using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TOONIPLAY
{
    public class InputHandler : MonoBehaviour
    {
        public enum InputType
        {
            None,
            Human,
            AI,
            Network,
            Recorded,
        }

        public List<BaseInputHandler> characterInputHandlerList = new();

        [Header("Cinemachine")]
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        private BaseInputHandler _currentInputHandler;

        private BaseCharacterController _owner;

        public bool IsCurrentDeviceMouse => _currentInputHandler != null && _currentInputHandler.IsCurrentDeviceMouse;

        public Vector2 Look => _currentInputHandler != null ? _currentInputHandler.Look : Vector2.zero;

        public Vector3 Velocity => _currentInputHandler != null ? _currentInputHandler.Velocity : Vector3.zero;

        public bool Sprint => _currentInputHandler != null && _currentInputHandler.Sprint;

        public bool AnalogMovement => _currentInputHandler != null && _currentInputHandler.AnalogMovement;

        public InputType CurrentInputType => _currentInputHandler.InputType;

        public ObstacleAgent obstacleAgent;

        private void Awake()
        {
            obstacleAgent = GetComponent<ObstacleAgent>();
            _owner = GetComponent<BaseCharacterController>();

            foreach (var inputHandler in characterInputHandlerList)
            {
                inputHandler.parentHandler = this;
            }
        }

        public void SetInputType(InputType type)
        {
            foreach (var inputHandler in characterInputHandlerList)
            {
                if (inputHandler.InputType == type)
                {
                    _currentInputHandler = inputHandler;
                    _currentInputHandler.gameObject.SetActive(true);
                }
                else
                {
                    inputHandler.gameObject.SetActive(false);
                }
            }

            _currentInputHandler.OnInitialize(this);
        }

        public InputType GetInputType() => _currentInputHandler != null ? _currentInputHandler.InputType : InputType.None;

        public void SetJump(bool jump) => _currentInputHandler?.SetJump(jump);
        public bool GetJump() => _currentInputHandler != null && _currentInputHandler.GetJump();

        public void SetMove(Vector2 move) => _currentInputHandler?.SetMove(move);

        public void UpdateInput()
        {
//            foreach (var inputHandler in characterInputHandlerList)
//            {
//                inputHandler.OnUpdateInput();
//            }

//#if UNITY_EDITOR || !(UNITY_ANDROID || UNITY_IOS)
//            if (GetInputType() == InputType.Human || GetInputType() == InputType.Recorded)
//                if (Input.GetMouseButton(1))
//                    owner.CameraTargetHandler.look = Look;
//#endif
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TOONIPLAY
{
    public class InputManager : TSingletonMonoBehaviour<InputManager>
    {
        private GlobalInputActions _globalInputActions;

        public GlobalInputActions GlobalInputActions => _globalInputActions;

        private CursorLockMode _lastCursorLockMode;

        public event Action<InputAction.CallbackContext> OnToggleInterface;

        public CursorLockMode LastCursorLockMode => _lastCursorLockMode;

        private Vector2 _lastMousePosition;

        private readonly Dictionary<double, Action> _idleFuncMap = new();
        private Dictionary<double, Action> _currentFuncMap = new();
        private Action _initialIdleFunc;

        private DateTime _lastInputTime;
        private DateTime _pauseDateTime;

        protected override void OnAwake()
        {
            base.OnAwake();

            _globalInputActions = new GlobalInputActions();
            //lastCursorLockMode = Cursor.lockState;
            _lastCursorLockMode = CursorLockMode.Confined;

            //OnToggleInterface += _ => SetCursorState(lastCursorLockMode != CursorLockMode.Locked ? true : false);
            OnToggleInterface += _ => SetCursorState(_lastCursorLockMode != CursorLockMode.Confined);

            _globalInputActions.UI.ToggleInterface.started += (context) => OnToggleInterface?.Invoke(context);

            InputSystem.settings.SetInternalFeatureFlag("USE_OPTIMIZED_CONTROLS", true);
            InputSystem.settings.SetInternalFeatureFlag("USE_READ_VALUE_CACHING", true);
            InputSystem.settings.SetInternalFeatureFlag("PARANOID_READ_VALUE_CACHING_CHECKS", true);

            _lastInputTime = DateTime.Now;
        }

        private void OnEnable()
        {
            _globalInputActions.Enable();
            StartCoroutine(CoCalculateIdleTime());
        }

        private void OnDisable()
        {
            StopCoroutine(CoCalculateIdleTime());
            _globalInputActions.Disable();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
#if !UNITY_EDITOR
            Cursor.lockState = hasFocus ? _lastCursorLockMode : CursorLockMode.None;
#endif
        }

        public void SetCursorState(bool newState)
        {
            //Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.lockState = newState ? CursorLockMode.Confined : CursorLockMode.None;
            _lastCursorLockMode = Cursor.lockState;
        }

        public void SetInitialIdleFunc(Action initialIdleFunc) => this._initialIdleFunc = initialIdleFunc;

        public void SetIdleFunction(double time, Action action)
        {
            _idleFuncMap[time] = action;
            _currentFuncMap[time] = action;
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                _pauseDateTime = DateTime.Now;
            }
            else
            {
                // if (DateTime.Now - _pauseDateTime > TimeSpan.FromHours(0.5))
                // {
                //     try
                //     {
                //         Debug.Log("OnApplicationPause");
                //         GameServer.Instance.Disconnect();
                //         await GameServer.Logout();
                //     }
                //     finally
                //     {
                //         UIManagerHelper.Instance.OpenSystemPopup("", "장시간 입력이 없어서 클라이언트를 종료합니다.", SystemContextPanel.PopupType.Confirm, (value) =>
                //         {
                //             ProjectManager.Instance.Quit();
                //         });
                //     }
                // }
            }
        }

        private IEnumerator CoCalculateIdleTime()
        {
            _currentFuncMap = _idleFuncMap.ToDictionary(entry => entry.Key, entry => entry.Value);

            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);

                if (_currentFuncMap.Count <= 0)
                    continue;

                if (IsAnyInputActive())
                {
                    _lastInputTime = DateTime.Now;
                    _currentFuncMap = InitializeIdleFuncMap(callback: () => _initialIdleFunc?.Invoke());
                }
                else
                {
                    InvokeExpiredCallbacks(_currentFuncMap, _lastInputTime);
                    _currentFuncMap = CreateRemovedExpiredCallbacksFuncMap(_currentFuncMap, _lastInputTime);
                }
#if UNITY_EDITOR || !(UNITY_ANDROID || UNITY_IOS)
                _lastMousePosition = Mouse.current.position.ReadValue();
#endif
            }
        }

        private Dictionary<double, Action> InitializeIdleFuncMap(Action callback)
        {
            Dictionary<double, Action> dic;

            if (_currentFuncMap.Count != _idleFuncMap.Count)
            {
                callback.Invoke();
                dic = _idleFuncMap.ToDictionary(entry => entry.Key, entry => entry.Value);
            }
            else
            {
                dic = _currentFuncMap;
            }

            return dic;
        }

        private void InvokeExpiredCallbacks(Dictionary<double, Action> funcMap, DateTime dateTime)
        {
            var validFuncMap = CreateValidFuncMap(funcMap, dateTime);

            foreach (var entry in validFuncMap)
            {
                entry.Value?.Invoke();               
            }
        }

        private Dictionary<double, Action> CreateRemovedExpiredCallbacksFuncMap(Dictionary<double, Action> funcMap, DateTime dateTime)
        {
            var validFuncMapKeyList = CreateValidFuncMapKeyList(funcMap, dateTime);
            foreach (var key in validFuncMapKeyList)
            {
                funcMap.Remove(key);
            }

            return funcMap;
        }

        private static Dictionary<double, Action> CreateValidFuncMap(Dictionary<double, Action> funcMap, DateTime dateTime)
        {
            return funcMap.Where(entry => IsDateTimeThresholdExceeded(entry.Key, dateTime))
                .ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        private static List<double> CreateValidFuncMapKeyList(Dictionary<double, Action> funcMap, DateTime dateTime)
        {
            return funcMap.Where(entry => IsDateTimeThresholdExceeded(entry.Key, dateTime))
                .Select(entry => entry.Key)
                .ToList();
        }

        private static bool IsDateTimeThresholdExceeded(double key, DateTime currentDateTime) =>
            DateTime.Now >= currentDateTime.AddSeconds(key);

        private bool IsAnyInputActive()
        {
            var isAnyKeyPressed = Keyboard.current.anyKey.isPressed;
#if UNITY_EDITOR || !(UNITY_ANDROID || UNITY_IOS)
            var isMouseLeftButtonPressed = Mouse.current.leftButton.isPressed;
            var isMouseRightButtonPressed = Mouse.current.rightButton.isPressed;
            var isMousePressed = Mouse.current.press.isPressed;
            var isMousePositionMoved = Mouse.current.position.ReadValue() != _lastMousePosition;
#endif
            var isTouched = (Touchscreen.current != null && Input.touchCount > 0);
            var isGamePadPressed = (Gamepad.current != null && Gamepad.current.allControls.Any(control => !Mathf.Approximately(control.EvaluateMagnitude(), 0f)));

            return isAnyKeyPressed           ||
#if UNITY_EDITOR || !(UNITY_ANDROID || UNITY_IOS)
                   isMouseLeftButtonPressed ||
                   isMouseRightButtonPressed ||
                   isMousePressed            ||                    
                   isMousePositionMoved      ||
#endif
                   isTouched                 ||
                   isGamePadPressed;
        }
    }
}

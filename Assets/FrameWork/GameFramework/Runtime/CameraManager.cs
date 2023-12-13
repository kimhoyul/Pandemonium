using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TOONIPLAY
{
    public class CameraManager : TSingletonMonoBehaviour<CameraManager>
    {
        public bool isLetterRoad = false;
        public enum CameraType
        {
            FirstPerson,
            ThirdPerson,
        }

        [SerializeField]
        [Range(0f, 1f)]
        private float cameraDefaultDistanceRatio = 0.5f;

        [SerializeField]
        private float camearMaxLength = 10.0f;

        [SerializeField]
        private float zoomSpeed = 1.0f;

        [SerializeField]
        private CinemachineVirtualCamera firstPersonVirtualCamera;

        [SerializeField]
        private CinemachineVirtualCamera thirdPersonVirtualCamera;

        private CameraType _cameraType;

        private CinemachineVirtualCamera _currentVirtualCamera;
        private Cinemachine3rdPersonFollow _current3RdFollowComponent;

        private CameraInputActions _cameraInputActions;

        private float _currentCameraDistance;
        private float _targetCameraDistance;

        private Coroutine _coZooming;

        public UnityEvent<CameraType, CinemachineVirtualCamera, CinemachineVirtualCamera> onChangedCamera = new();

        public CameraType CurrentCameraType => _cameraType;

        public float CurrentCameraDistance => _currentCameraDistance;

        protected override void OnAwake()
        {
            base.OnAwake();

            _cameraInputActions = new CameraInputActions();
            _cameraInputActions.Camera.Zoom.performed += OnPerformedZoom;
        }

        private void OnEnable()
        {
            _cameraInputActions.Enable();
        }

        private void OnDisable()
        {
            _cameraInputActions.Disable();

            StopAllCoroutines();
        }

        private void Start()
        {
            GetCurrentVirtualCamera();
        }

        public CinemachineVirtualCamera GetCurrentVirtualCamera()
        {
            if (_currentVirtualCamera != null)
                return _currentVirtualCamera;

            if (CinemachineCore.Instance.VirtualCameraCount == 0)
                return SetCameraComponent(null);

            return SetCurrentVirtualCamera(CinemachineCore.Instance.GetVirtualCamera(0) as CinemachineVirtualCamera);
        }

        public CinemachineVirtualCamera SetCamera(CameraType cameraType, Transform followTarget, Transform lookAtTarget)
        {
            var virtualCamera = GetCurrentVirtualCamera();
            if (virtualCamera == null)
                return null;

            if (this._cameraType != cameraType)
            {
                switch (cameraType)
                {
                    case CameraType.FirstPerson:
                        (firstPersonVirtualCamera.Priority, virtualCamera.Priority) = (virtualCamera.Priority, firstPersonVirtualCamera.Priority);
                        virtualCamera = SetCurrentVirtualCamera(firstPersonVirtualCamera);
                        break;

                    case CameraType.ThirdPerson:
                        (thirdPersonVirtualCamera.Priority, virtualCamera.Priority) = (virtualCamera.Priority, thirdPersonVirtualCamera.Priority);
                        virtualCamera = SetCurrentVirtualCamera(thirdPersonVirtualCamera);
                        break;
                }
            }

            virtualCamera.Follow = followTarget;
            virtualCamera.LookAt = lookAtTarget;

            return _currentVirtualCamera;
        }

        private void OnPerformedZoom(InputAction.CallbackContext callbackContext)
        {
            if (InputManager.Instance.LastCursorLockMode == CursorLockMode.None)
                return;

            if (_currentVirtualCamera == null)
                _currentVirtualCamera = GetCurrentVirtualCamera();

            var value = callbackContext.ReadValue<Vector2>();

            Zooming(value.y);
        }

        public float ZoomSpeed { set { zoomSpeed = value; } }

        public void Zooming(float y)
        {
            if (_currentVirtualCamera == null)
                return;

            if (CinemachineCore.Instance.IsLiveInBlend(_currentVirtualCamera))
                return;

            _currentCameraDistance -= y * Time.deltaTime * zoomSpeed;
            _currentCameraDistance = Math.Clamp(_currentCameraDistance, 0.3f, camearMaxLength);

            //if (currentCameraDistance <= 0.3f)
            //{
            //    SetCamera(CameraType.FirstPerson, currentVirtualCamera.Follow, currentVirtualCamera.LookAt);
            //}
            //else
            {
                SetCamera(CameraType.ThirdPerson, _currentVirtualCamera.Follow, _currentVirtualCamera.LookAt);

                if (_current3RdFollowComponent != null)
                    _current3RdFollowComponent.CameraDistance = _currentCameraDistance;
            }
        }

        private CinemachineVirtualCamera SetCurrentVirtualCamera(CinemachineVirtualCamera virtualCamera)
        {
            var prevVirtualCamera = _currentVirtualCamera;

            _currentVirtualCamera = virtualCamera;

            SetCameraComponent(_currentVirtualCamera);

            onChangedCamera?.Invoke(_cameraType, prevVirtualCamera, _currentVirtualCamera);

            return _currentVirtualCamera;
        }

        private CinemachineVirtualCamera SetCameraComponent(CinemachineVirtualCamera virtualCamera)
        {
            if (_coZooming != null)
            {
                StopCoroutine(_coZooming);
                _coZooming = null;
            }

            _current3RdFollowComponent = (virtualCamera != null) ? virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>() : null;
            if (_current3RdFollowComponent != null)
            {
                _cameraType = CameraType.ThirdPerson;
                _currentCameraDistance = _targetCameraDistance = _current3RdFollowComponent.CameraDistance = camearMaxLength * cameraDefaultDistanceRatio;
            }
            else
            {
                _cameraType = CameraType.FirstPerson;
                _currentCameraDistance = _targetCameraDistance = 0.0f;
            }

            return virtualCamera;
        }

        private float Distance(float a, float b) => Mathf.Abs(Mathf.Abs(a) - Mathf.Abs(b));
    }
}

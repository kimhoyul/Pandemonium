using UnityEngine;

namespace TOONIPLAY
{
    public class CameraTargetHandler : MonoBehaviour
    {
        // cinemachine
        //private float _cinemachineTargetYaw;
        //private float _cinemachineTargetPitch;

        //private const float _threshold = 0.01f;

        //private BaseCharacterController owner;

        [HideInInspector]
        public Vector2 look;

        //private void Awake()
        //{
        //    owner = GetComponentInParent<BaseCharacterController>();
        //}

        //private void Start()
        //{
        //    _cinemachineTargetYaw = transform.rotation.eulerAngles.y;
        //}

        //private void LateUpdate()
        //{
        //    if (owner.InputHandler == null)
        //        return;

        //    if (owner.InputHandler.GetInputType() == InputHandler.InputType.Human || owner.InputHandler.GetInputType() == InputHandler.InputType.Recorded)
        //        CameraRotation(look);

        //    look = Vector2.zero;
        //}

        //public void CameraRotation(Vector2 lookValue)
        //{
        //    // if there is an input and camera position is not fixed
        //    if (lookValue.sqrMagnitude >= _threshold && !owner.InputHandler.LockCameraPosition)
        //    {
        //        //Don't multiply mouse input by Time.deltaTime;
        //        float deltaTimeMultiplier = owner.InputHandler.IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

        //        _cinemachineTargetYaw += lookValue.x * deltaTimeMultiplier;
        //        _cinemachineTargetPitch += ((CameraManager.Instance.CurrentCameraType == CameraManager.CameraType.ThirdPerson) ? lookValue.y : -lookValue.y) * deltaTimeMultiplier;
        //    }

        //    // clamp our rotations so our values are limited 360 degrees
        //    _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        //    _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, owner.InputHandler.BottomClamp, owner.InputHandler.TopClamp);

        //    // Cinemachine will follow this target
        //    ApplyTransform(_cinemachineTargetPitch + owner.InputHandler.CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        //}

        //public void ApplyTransform(float pitch, float yaw, float roll)
        //{
        //    switch (CameraManager.Instance.CurrentCameraType)
        //    {
        //        case CameraManager.CameraType.FirstPerson:
        //            {
        //                gameObject.transform.localRotation = Quaternion.Euler(pitch, 0.0f, roll);

        //                if (owner != null)
        //                    owner.gameObject.transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
        //            }
        //            break;

        //        case CameraManager.CameraType.ThirdPerson:
        //            if (CameraManager.Instance.isLetterRoad)
        //            {
        //                if (gameObject.transform.eulerAngles.x > 29 && gameObject.transform.eulerAngles.x < 31)
        //                    gameObject.transform.rotation = Quaternion.Euler(30, yaw, roll);
        //                else if (gameObject.transform.eulerAngles.x > 180)
        //                    gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x + 0.5f, yaw, roll);
        //                else if (gameObject.transform.eulerAngles.x < 30)
        //                    gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x + 0.5f, yaw, roll);
        //                else if (gameObject.transform.eulerAngles.x <= 180 && gameObject.transform.eulerAngles.x > 30)
        //                    gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x - 0.5f, yaw, roll);
        //            }
        //            else
        //            {
        //                gameObject.transform.rotation = Quaternion.Euler(pitch, yaw, roll);
        //            }
        //            break;
        //    }
        //}

        //private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        //{
        //    if (lfAngle < -360f) lfAngle += 360f;
        //    if (lfAngle > 360f) lfAngle -= 360f;
        //    return Mathf.Clamp(lfAngle, lfMin, lfMax);
        //}
    }
}

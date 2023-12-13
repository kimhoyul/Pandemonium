using Cinemachine;

namespace TOONIPLAY
{
    public class CinemachinePOVExtension : CinemachineExtension
    {
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (!vcam.Follow)
                return;

            if (stage != CinemachineCore.Stage.Aim)
                return;
            
            var virtualCamera = vcam as CinemachineVirtualCamera;
            if (virtualCamera != null && virtualCamera.Follow != null) 
            {
                state.RawOrientation = virtualCamera.Follow.rotation;
            }
            else
            {
                state.RawOrientation = transform.rotation;
            }
        }
    }
}

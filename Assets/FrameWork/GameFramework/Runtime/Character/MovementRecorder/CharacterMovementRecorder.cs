using System.Collections;
using UnityEditor;
using UnityEngine;

namespace TOONIPLAY
{
    public class CharacterMovementRecorder : CharacterControllerAddOn
    {
        [Header("Recording")]
        public bool isRecording;

        public RecordedInputDataSO recordData;

        private RecordedInputDataSO _recordingData;
        private float _startRecordingTime;

        public void StartRecording()
        {
#if UNITY_EDITOR
            isRecording = true;
            _startRecordingTime = Time.unscaledTime;

            _recordingData = ScriptableObject.CreateInstance<RecordedInputDataSO>();

            var trans = gameObject.transform;
            _recordingData.startPosition = trans.position;
            _recordingData.startRotation = trans.rotation;
            //recordingData.cameraPitch = _cinemachineTargetPitch;
            //recordingData.cameraYaw = _cinemachineTargetYaw;

            //recordingData.startCameraRotation = owner.CameraTargetHandler.transform.rotation;

            _recordingData.recoredInputData = new();
#endif
        }

        public void FinishRecording()
        {
#if UNITY_EDITOR
            isRecording = false;

            AssetDatabase.CreateAsset(_recordingData, "Assets/RecordingInputData.asset");

            _recordingData = null;
#endif
        }

        public void PlayRecordedInputData()
        {
            if (recordData == null)
                return;

            StartCoroutine(CoPlayInputData());

            Debug.Log("Playing");
        }

        private IEnumerator CoPlayInputData()
        {
            yield return new WaitForEndOfFrame();

            var trans = gameObject.transform;
            trans.position = recordData.startPosition;
            trans.rotation = recordData.startRotation;

            //owner.CameraTargetHandler.transform.rotation = recordData.startCameraRotation;
            //_cinemachineTargetPitch = recordData.cameraPitch;
            //_cinemachineTargetYaw = recordData.cameraYaw;

            Owner.InputHandler.SetInputType(InputHandler.InputType.Recorded);

            Debug.Log("Complete to load input data");

            yield return new WaitForSeconds(1.0f);

            //(currentInputHandler as RecordedInputHandler).SetRecordedData(recordData.recoredInputData);
        }

        private void Update()
        {
            if (!isRecording || _recordingData == null)
                return;
            
            var inputData = new RecordedInputDataSO.InputData
            {
                time = Time.unscaledTime - _startRecordingTime,
                velocity = Vector3.zero,
                look = Owner.InputHandler.Look,
                sprint = Owner.InputHandler.Sprint,
                jump = Owner.InputHandler.GetJump(),
                isCurrentDeviceMouse = Owner.InputHandler.IsCurrentDeviceMouse
            };

            _recordingData.recoredInputData.Add(inputData);
        }
    }
}

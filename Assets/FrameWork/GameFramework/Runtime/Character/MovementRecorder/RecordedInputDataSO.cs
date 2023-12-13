using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TOONIPLAY
{
    public class RecordedInputDataSO : ScriptableObject
    {
        public Vector3 startPosition;
        public Quaternion startRotation;

        public Quaternion startCameraRotation;
        public float cameraPitch;
        public float cameraYaw;

        [System.Serializable]
        public struct InputData
        {
            public float time;
            public Vector3 velocity;
            public Vector2 look;
            public bool jump;
            public bool sprint;
            public bool isCurrentDeviceMouse;
        }

        public List<InputData> recoredInputData;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TOONIPLAY
{
    public class RecordedInputHandler : BaseInputHandler
    {
        public List<RecordedInputDataSO.InputData> recordData;
        private int _index;

        public override InputHandler.InputType InputType => InputHandler.InputType.Recorded;

        private float _startPlayingTime;
        private Vector3 _velocity;
        private Vector2 _look;
        private bool _jump;
        private bool _sprint;
        private bool _isCurrentDeviceMouse;

        public override Vector2 Look => _look;

        public override Vector3 Velocity => _velocity;

        public override bool Sprint => _sprint;

        public override bool IsCurrentDeviceMouse => _isCurrentDeviceMouse;

        public override bool GetJump() => _jump;

        public void SetRecordedData(List<RecordedInputDataSO.InputData> inputData)
        {
            recordData = inputData;
            _index = 0;
            _velocity = recordData[_index].velocity;
            _look = recordData[_index].look;
            _jump = recordData[_index].jump;
            _sprint = recordData[_index].sprint;
            _isCurrentDeviceMouse = recordData[_index].isCurrentDeviceMouse;

            _startPlayingTime = Time.unscaledTime;
        }

        public override void OnUpdateInput()
        {
            if (recordData == null)
                return;

            float currentPlayingTime = Time.unscaledTime - _startPlayingTime;
            if (0 < recordData.Count && recordData[_index].time < currentPlayingTime)
            {
                if (_index + 1 == recordData.Count)
                {
                    recordData = null;
                    _index = 0;
                    parentHandler.SetInputType(InputHandler.InputType.Human);
                    return;
                }

                _index++;
                _velocity = recordData[_index].velocity;
                _look = recordData[_index].look;
                _jump = recordData[_index].jump;
                _sprint = recordData[_index].sprint;
                _isCurrentDeviceMouse = recordData[_index].isCurrentDeviceMouse;
            }
        }
    }
}

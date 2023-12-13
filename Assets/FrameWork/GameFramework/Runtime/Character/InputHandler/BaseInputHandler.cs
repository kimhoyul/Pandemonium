using UnityEngine;

namespace TOONIPLAY
{
    public class BaseInputHandler : MonoBehaviour
    {
        public virtual InputHandler.InputType InputType => InputHandler.InputType.None;

        public virtual bool IsCurrentDeviceMouse => false;

        public InputHandler parentHandler;

        public virtual Vector2 Look => Vector2.zero;

        public virtual Vector3 Velocity => Vector3.zero;

        public virtual bool AnalogMovement => false;

        public virtual bool Sprint => false;

        public virtual void SetJump(bool jump) { }
        public virtual bool GetJump() => false;

        public virtual void SetMove(Vector2 move) { }

        public virtual void OnInitialize(InputHandler rootHandler) { }

        public virtual void OnUpdateInput() { }
    }
}

namespace TOONIPLAY
{
    public class NetworkInputHandler : BaseInputHandler
    {
        private bool _jump;

        public override InputHandler.InputType InputType => InputHandler.InputType.Network;

        public override void SetJump(bool jump) => this._jump = jump;

        public override bool GetJump() => _jump;

        public override void OnInitialize(InputHandler rootHandler)
        {
            base.OnInitialize(rootHandler);

            rootHandler.obstacleAgent.SetEnableAgent(false);
        }
    }
}

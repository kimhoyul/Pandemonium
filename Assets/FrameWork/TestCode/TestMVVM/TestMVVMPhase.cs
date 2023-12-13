using TOONIPLAY;

public class TestMVVMPhase : BasePhase
{
    private void Start()
    {
        TOONIPLAY.GameManager.Instance.RegisterDataStorage(new TestDataStorage());
    }
}

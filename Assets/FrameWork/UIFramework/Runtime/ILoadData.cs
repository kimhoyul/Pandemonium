using UnityEngine;

public interface ILoadData
{
    public void StartLoading();
    public void SetProgressValue(float value);
    public void EndLoading();
}

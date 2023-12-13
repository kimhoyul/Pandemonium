using UnityEngine;

public static class CoroutineExtensions
{
    public static void SafeStopCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine coroutine)
    {
        if (coroutine == null)
            return;
        
        monoBehaviour.StopCoroutine(coroutine);
        coroutine = null;
    }   
}

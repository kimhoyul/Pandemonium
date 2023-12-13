using System;
using System.Collections;
using UnityEngine;

public class Debounce : MonoBehaviour
{ 
    public bool IsProgress { get; private set; }

    private Coroutine _debounceCoroutine;

    public void OnDebounce(Action action, float delayTime)
    {
        if (_debounceCoroutine != null)
        {
            StopCoroutine(_debounceCoroutine);
        }

        _debounceCoroutine = StartCoroutine(CoDebounce(action, delayTime));
        IsProgress = true;
    }

    private IEnumerator CoDebounce(Action action, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        action.Invoke();
        IsProgress = false;
    }
}

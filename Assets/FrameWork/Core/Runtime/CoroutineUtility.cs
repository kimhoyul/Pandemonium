using System;
using System.Collections;
using UnityEngine;


namespace TOONIPLAY.Utilities
{
    public static class CoroutineUtility
    {
        public static void SafeStopCoroutine(MonoBehaviour monoBehaviour, Coroutine coroutine)
        {
            if (coroutine == null)
                return;

            monoBehaviour.StopCoroutine(coroutine);
            coroutine = null;
        }

        public static Coroutine RunForDuration(MonoBehaviour monoBehaviour, Action callback, float duration)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(RunForDurationCoroutine(callback, duration));

            return null;
        }

        private static IEnumerator RunForDurationCoroutine(Action callback, float duration)
        {
            float timer = 0;

            while (timer < duration)
            {
                callback.Invoke();         
                timer += Time.deltaTime;
                yield return null;
            }
        }

        public static Coroutine RunWhileLoopWithInterval(MonoBehaviour monoBehaviour, Action callback, float interval)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(RunWhileLoopWithIntervalCoroutine(callback, interval));

            return null;
        }

        private static IEnumerator RunWhileLoopWithIntervalCoroutine(Action callback, float interval)
        {
            while (true)
            {
                callback.Invoke();
                yield return new WaitForSeconds(interval);
            }
        }

        public static Coroutine RunWhileLoopWithFrame(MonoBehaviour monoBehaviour, Action callback)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(RunWhileLoopWithFrameCoroutine(callback));

            return null;
        }

        private static IEnumerator RunWhileLoopWithFrameCoroutine(Action callback)
        {
            while (true)
            {
                callback.Invoke();
                yield return null;
            }
        }

        public static Coroutine RunWhileLoopWithCondition(MonoBehaviour monoBehaviour, Action callback, Func<bool> condition)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(RunWhileLoopWithConditionCoroutine(callback, condition));

            return null;
        }

        private static IEnumerator RunWhileLoopWithConditionCoroutine(Action callback, Func<bool> condition)
        {
            while (true)
            {
                callback.Invoke();
                yield return new WaitUntil(condition);
            }
        }

        public static Coroutine RunForLoopWithInterval(MonoBehaviour monoBehaviour, Action callback, int count, float interval, Action startCallback = null, Action endCallback = null)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(RunForLoopWithIntervalCoroutine(callback, count, interval, startCallback, endCallback));

            return null;
        }

        private static IEnumerator RunForLoopWithIntervalCoroutine(Action callback, int count, float interval, Action startCallback = null, Action endCallback = null)
        {
            startCallback?.Invoke();

            for (int i = 0; i < count; i++)
            {
                callback.Invoke();
                yield return new WaitForSeconds(interval);
            }

            endCallback?.Invoke();
        }

        public static Coroutine RunForLoopWithCondition(MonoBehaviour monoBehaviour, Action callback, int count, Func<bool> condition, Action endCallback = null)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(RunForLoopWithConditionCoroutine(callback, count, condition, endCallback));

            return null;
        }

        private static IEnumerator RunForLoopWithConditionCoroutine(Action callback, int count, Func<bool> condition, Action endCallback = null)
        {
            for (int i = 0; i < count; i++)
            {
                callback.Invoke();
                yield return new WaitUntil(condition);
            }
            endCallback?.Invoke();
        }

        public static Coroutine RunForLoopWithIntervalArray(MonoBehaviour monoBehaviour, Action callback, int count, float[] intervalArray, Action endCallback = null)
        {
            if (count != intervalArray.Length)
                Debug.LogError("The count and interval length cannot be matched");
        
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(RunForLoopWithIntervalArrayCorotune(callback, count, intervalArray, endCallback));

            return null;
        }

        static IEnumerator RunForLoopWithIntervalArrayCorotune(Action callback, int count, float[] intervalArr, Action endCallback = null)
        {
            for (var i = 0; i < count; i++)
            {
                callback.Invoke();
                yield return new WaitForSeconds(intervalArr[i]);
            }
            endCallback?.Invoke();
        }

        public static Coroutine RunWhileLoopWithIntervalAndDelay(MonoBehaviour monoBehaviour, Action callback, float interval, float delay)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(RunWhileLoopWithIntervalAndDelayCoroutine(callback, interval, delay));

            return null;
        }

        private static IEnumerator RunWhileLoopWithIntervalAndDelayCoroutine(Action callback, float seconds, float delay)
        {
            yield return new WaitForSeconds(delay);

            while (true)
            {
                callback.Invoke();
                yield return new WaitForSeconds(seconds);
            }
        }

        public static Coroutine ExecuteSwitchingActionWithInterval(MonoBehaviour monoBehaviour, Action callback1, Action callback2, float interval)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(ExecuteSwitchingActionWithIntervalCoroutine(callback1, callback2, interval));

            return null;
        }

        private static IEnumerator ExecuteSwitchingActionWithIntervalCoroutine(Action callback1, Action callback2, float interval)
        {
            callback1.Invoke();
            yield return new WaitForSeconds(interval);
            callback2.Invoke();
        }

        public static Coroutine ExecuteForCondition(MonoBehaviour monoBehaviour, Action callback, Func<bool> condition)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(ExecuteForConditionCoroutine(callback, condition));

            return null;
        }

        private static IEnumerator ExecuteForConditionCoroutine(Action callback, Func<bool> condition)
        {
            yield return new WaitUntil(condition);
            callback.Invoke();
        }

        public static Coroutine ExecuteForDelayTime(MonoBehaviour monoBehaviour, Action callback, float delay)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(ExecuteForDelayTimeCoroutine(callback, delay));

            return null;
        }

        private static IEnumerator ExecuteForDelayTimeCoroutine(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }

        public static Coroutine ExecuteForDelayRealTime(MonoBehaviour monoBehaviour, Action callback, float delay)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(ExecuteForDelayRealTimeCoroutine(callback, delay));

            return null;
        }

        private static IEnumerator ExecuteForDelayRealTimeCoroutine(Action callback, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            callback.Invoke();
        }

        public static Coroutine ExecuteForDelayFrame(MonoBehaviour monoBehaviour, Action callback, int count = 0)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(ExecuteForDelayFrameCoroutine(callback, count));

            return null;
        }

        private static IEnumerator ExecuteForDelayFrameCoroutine(Action callback, int count = 0)
        {
            for (var i = 0; i < count; i++)
                yield return null;
            callback.Invoke();
        }

        public static Coroutine ExecuteForDelayFixedFrame(MonoBehaviour monoBehaviour, Action callback, int count = 0)
        {
            if (monoBehaviour.isActiveAndEnabled)
                return monoBehaviour.StartCoroutine(ExecuteForDelayFixedFrameCoroutine(callback, count));

            return null;
        }

        private static IEnumerator ExecuteForDelayFixedFrameCoroutine(Action callback, int count = 0)
        {
            for (var i = 0; i < count; i++)
                yield return new WaitForFixedUpdate();

            callback.Invoke();
        }
    }
}

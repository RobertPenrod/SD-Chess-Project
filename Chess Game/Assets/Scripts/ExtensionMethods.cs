using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2, bool clamp = true)
    {
        float result = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        if (clamp) 
        {
            result = Mathf.Clamp(result, Mathf.Min(from2, to2), Mathf.Max(from2, to2));
        }
        return result;
    }

    public static void DelayedInvoke(float delay, MonoBehaviour monoBehavior, Action function)
    {
        monoBehavior.StartCoroutine(DelayedInvoke_Coroutine(function, delay));
    }
    static IEnumerator DelayedInvoke_Coroutine(Action function, float delayTime)
    {
        if (delayTime <= 0f)
        {
            for(; delayTime < 0f; delayTime++)
            {
                yield return null;
            }
        }
        else
            yield return new WaitForSecondsRealtime(delayTime);
        function.Invoke();
    }
    public static void RepeatingInvoke(float waitTime, float repeatRate, int repeatCount, MonoBehaviour monoBehaviour, Action function)
    {
        monoBehaviour.StartCoroutine(RepeatingInvoke_Coroutine(waitTime, repeatRate, repeatCount, function));
    }
    static IEnumerator RepeatingInvoke_Coroutine(float waitTime, float repeatRate, int repeatCount, Action function)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        for(int c = 0; repeatCount <= 0 || c < repeatCount; c++)
        {
            yield return new WaitForSecondsRealtime(repeatRate);
            function.Invoke();
        }
    }

    public static void ChangeFullTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public static string SecondsToTimeString(float ellapsedSeconds)
    {
        if (ellapsedSeconds < 0)
        {
            Debug.LogWarning("EXtensionMethoids -> SecondsToTimeString() given a time < 0");
            return "--:--:--";
        }

        float time = ellapsedSeconds;

        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        float ms = time % 1 * 1000;

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, ms);
    }

    public static List<T> ShuffleList<T>(List<T> list)
    {
        int swapCount = list.Count * 2;
        for(int i = 0; i < swapCount; i++)
        {
            int indexA = Random.Range(0, list.Count);
            int indexB = Random.Range(0, list.Count);
            if (indexA == indexB) continue;

            // Swap
            T temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }
        return list;
    }

    public static T RandomListItem<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}

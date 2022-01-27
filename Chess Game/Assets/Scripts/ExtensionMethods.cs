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
}

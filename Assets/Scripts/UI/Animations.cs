using System;
using System.Collections;
using UnityEngine;

public class Animations : MonoBehaviour
{
    // Singleton pattern
    public static Animations Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeIn(Action<float> setAlpha, float duration = 1f)
    {
        if (setAlpha == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            setAlpha(Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        setAlpha(1f);
    }
}

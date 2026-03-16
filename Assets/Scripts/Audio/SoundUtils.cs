using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundUtils
{
    public static IEnumerator PlayRandomSoundsLoop(AudioSource audioSource, List<AudioClip> clips, (float x, float y) intervalRange, Func<bool> condition, float volume = 1f)
    {
        if (clips == null || clips.Count == 0) yield break;

        audioSource.volume = volume;
        while (condition())
        {
            AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(UnityEngine.Random.Range(intervalRange.x, intervalRange.y));
        }
    }

    public static void PlayARandomSound(AudioSource audioSource, List<AudioClip> clips, float volume = 1f)
    {
        if (clips == null || clips.Count == 0) return;

        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];
        audioSource.volume = volume;
        audioSource.clip = clip;

        audioSource.Play();
    }
}

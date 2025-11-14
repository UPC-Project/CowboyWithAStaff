using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundUtils
{
    public static IEnumerator PlayRandomSounds(AudioSource audioSource,List<AudioClip> clips, (float x, float y) intervalRange, Func<bool> condition, float volume = 1f)
    {
        if (clips == null || clips.Count == 0) yield break;

        while (condition())
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(intervalRange.x, intervalRange.y));
            AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Count)];
            audioSource.volume = volume;
            audioSource.clip = clip;

            audioSource.Play();
        }
    }
}

using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        AudioClip clipToPlay = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];

        if (s.loop)
        {
            musicSource.clip = clipToPlay;
            musicSource.volume = s.volume;
            musicSource.pitch = s.pitch;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            sfxSource.pitch = s.pitch;
            sfxSource.PlayOneShot(clipToPlay, s.volume);
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
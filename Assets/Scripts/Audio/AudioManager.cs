using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Lists")]
    public Sound[] sounds;
    public Sound[] musicTracks;

    [SerializeField] private AudioSource musicSource;
    public AudioSource sfxSource;

    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        musicSource.loop = true;

        DontDestroyOnLoad(this.gameObject); // Persist across scenes
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicTracks, track => track.name == name);

        if (s == null) return;

        AudioClip clipToPlay = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];

        musicSource.clip = clipToPlay;
        musicSource.volume = s.volume;
        musicSource.pitch = s.pitch;
        musicSource.Play();
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null) return;

        AudioClip clipToPlay = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];

        sfxSource.pitch = s.pitch;
        sfxSource.PlayOneShot(clipToPlay, s.volume);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.loop)
        {
            musicSource.Stop();
        }
    }


}
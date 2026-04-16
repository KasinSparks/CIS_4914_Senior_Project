using System;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSound : MonoBehaviour
{
    public enum Sounds
    {
        None,
        Forest,
        Desert,
        Plains,
        Path,
    }

    public AudioClip forest;
    public AudioClip desert;
    public AudioClip plains;
    public AudioClip path;

    public static bool should_be_playing = false;
    public static bool is_playing = false;
    public static BackgroundSound.Sounds current_sound = BackgroundSound.Sounds.None;

    // START: Code inspired from https://stackoverflow.com/questions/27911324/play-continuous-music-when-swapping-between-multiple-scene-in-unity3d
    [SerializeField] private AudioSource audio_source;

    private static BackgroundSound _instance;

    public static BackgroundSound instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindFirstObjectByType<BackgroundSound>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }
    // END

    private static void CheckForInstance()
    {
        if (_instance == null)
        {
            Debug.LogError("For the Background sound to work, you must start with the StartScreen Scene first.");
        }
    }

    public static void Play(Sounds sound)
    {
        CheckForInstance();

        if (current_sound == sound)
        {
            return;
        }
        
        current_sound = sound;

        switch (sound)
        {
            case Sounds.None:
                return;
            case Sounds.Forest:
                _instance.audio_source.clip = _instance.forest;
                break;
            case Sounds.Desert:
                _instance.audio_source.clip = _instance.desert;
                break;
            case Sounds.Plains:
                _instance.audio_source.clip = _instance.plains;
                break;
            case Sounds.Path:
                _instance.audio_source.clip = _instance.path;
                break;
        }

        should_be_playing = true;
        _instance.audio_source.Play();
        is_playing = true;
    }

    public static void Pause()
    {
        CheckForInstance();
        should_be_playing = false;
        _instance.audio_source.Pause();
        is_playing = false;
        current_sound = Sounds.None;
    }

    public static void Stop()
    {
        CheckForInstance();
        should_be_playing = false;
        _instance.audio_source.Stop();
        is_playing = false;
        current_sound = Sounds.None;
    }
}

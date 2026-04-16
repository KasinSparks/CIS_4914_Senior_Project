using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static bool should_be_playing = true;
    public static bool is_playing = false;

    // START: Code inspired from https://stackoverflow.com/questions/27911324/play-continuous-music-when-swapping-between-multiple-scene-in-unity3d
    [SerializeField] private AudioSource m_AudioSource;

    private static BackgroundMusic _instance;

    public static BackgroundMusic instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindFirstObjectByType<BackgroundMusic>();

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

            if (should_be_playing && !is_playing)
            {
                Play();
            }
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

    public static void Play()
    {
        should_be_playing = true;
        _instance.m_AudioSource.Play();
        is_playing = true;
    }

    public static void Pause()
    {
        should_be_playing = false;
        _instance.m_AudioSource.Pause();
        is_playing = false;
    }
}

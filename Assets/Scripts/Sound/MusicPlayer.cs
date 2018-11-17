using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour {

    public static MusicPlayer Instance;

    private static AudioSource audioSource;
    private AudioClip music;

    public static float MusicVolume
    {
        get
        {
            return PlayerPrefsHelper.GetFloat("MusicVolume", 1);
        }
        set
        {
            PlayerPrefsHelper.SetFloat("MusicVolume", value);
            audioSource.volume = value;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
            music = (AudioClip)Resources.Load("Music/music");
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource.clip = music;
        audioSource.volume = MusicVolume;
        audioSource.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundPlayer {

    public static GameObject playSound;

    private static bool init;
    private static List<AudioClip> AllSounds = new List<AudioClip>();

    public static float SoundVolume
    {
        get
        {
            return PlayerPrefsHelper.GetFloat("SoundVolume", 1);
        }
        set
        {
            PlayerPrefsHelper.SetFloat("SoundVolume", value);
        }
    }

    static void InitSound()
    {
        if (!init)
        {
            //Here we put a folder name and a file name in Resources folder
            playSound = (GameObject) Resources.Load("SoundPrefab/PlaySound");
            Object[] allObjects = Resources.LoadAll("Sounds");
            for (int i = 0; i < allObjects.Length; i++)
            {
                AllSounds.Add((AudioClip)allObjects[i]);
            }
            init = true;
        }
    }

    public static void Play(string clipName, float volume)
    {
        InitSound();

        for (int i = 0; i < AllSounds.Count; i++)
        {
            if(clipName == AllSounds[i].name)
            {
                GameObject spawnedSound = GameObject.Instantiate(playSound);
                AudioSource objSource = spawnedSound.GetComponent<AudioSource>();
                objSource.clip = AllSounds[i];
                objSource.volume = volume * SoundVolume;
                objSource.Play();   
                GameObject.Destroy(spawnedSound, objSource.clip.length);
                break;
            }
        }
    }    
}

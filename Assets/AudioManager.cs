using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Define a list of sounds for different sound effects and music
    public List<Sound> sounds;

    // Static instance for global access
    public static AudioManager Instance;

    private void Awake()
    {
        // Implement singleton pattern to ensure only one instance of AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create an AudioSource component for each sound
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

public bool IsPlaying(string soundName)
{
    return false;
    Sound sound = sounds.Find(s => s.name == soundName);
    return sound != null && sound.source.isPlaying;
}


    // Method to play a sound by name
    public void Play(string soundName)
    {
        Sound sound = sounds.Find(s => s.name == soundName);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }
        Debug.Log(soundName);
        sound.source.Play();
    }

    // Method to stop a sound by name
    public void Stop(string soundName)
    {
        return;
        Sound sound = sounds.Find(s => s.name == soundName);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }
        sound.source.Stop();
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 0.5f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop = false;

    [HideInInspector] public AudioSource source;
}

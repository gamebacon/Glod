using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Range(0, 1)] public float masterVolume = 1.0f;

    // Define a list of sounds for different sound effects and music
    public List<Sound> sounds;

    // Static instance for global access
    public static AudioManager Instance;

    // The range of pitch variation (minimum and maximum offsets for pitch)
    public float minPitchOffset = 0.9f; // Minimum pitch offset (slower)
    public float maxPitchOffset = 1.1f; // Maximum pitch offset (faster)

    public float minVolumeOffset = 0.9f; // Minimum volume offset (quieter)
    public float maxVolumeOffset = 1.1f; // Maximum volume offset (louder)

    private void Awake()
    {
        // Implement singleton pattern to ensure only one instance of AudioManager
        if (Instance == null)
        {
            Instance = this;
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
            sound.source.pitch = sound.pitch; // Initial pitch from the sound
            sound.source.loop = sound.loop;
        }
    }

    public bool IsPlaying(SoundType type)
    {
        Sound sound = sounds.Find(s => s.type == type);
        return sound != null && sound.source.isPlaying;
    }

    // Method to play a sound by name with random pitch offset
    public void Play(SoundType type)
    {
        Sound sound = sounds.Find(s => s.type == type);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + type + " not found!");
            return;
        } 

        // Apply a random pitch offset relative to the original pitch
        sound.source.pitch = sound.pitch * Random.Range(minPitchOffset, maxPitchOffset);

        // Apply a random volume offset relative to the original volume
        sound.source.volume = (sound.volume * Random.Range(minVolumeOffset, maxVolumeOffset)) * masterVolume;

        // Play the sound
        sound.source.Play();
    }

    // Method to stop a sound by name
    public void Stop(SoundType type)
    {
        Sound sound = sounds.Find(s => s.type == type);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + type + " not found!");
            return;
        }
        sound.source.Stop();
    }
}

[System.Serializable]
public class Sound
{
    [SerializeField] private string name;
    public SoundType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 0.5f;  // The original volume for this sound
    [Range(0.1f, 3f)] public float pitch = 1f;   // The original pitch for this sound
    public bool loop = false;

    [HideInInspector] public AudioSource source;  // The AudioSource that will play this sound

    public Sound() 
    { 
        this.name = this.type.ToString();
    }
}

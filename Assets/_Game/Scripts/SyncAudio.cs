using UnityEngine;

public class SyncAudio : MonoBehaviour
{
    public enum AudioType { Music, SFX }
    public AudioType type;

    private AudioSource audioSource;
    private float baseVolume = 1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        baseVolume = audioSource.volume;
    }

    private void OnEnable()
    {
        if (type == AudioType.Music)
        {
            SettingsManager.OnMusicVolumeChanged += UpdateVolume;
            UpdateVolume(PlayerPrefs.GetFloat("MusicVolume", 0.8f));
        }
        else
        {
            SettingsManager.OnSFXVolumeChanged += UpdateVolume;
            UpdateVolume(PlayerPrefs.GetFloat("SFXVolume", 0.8f));
        }
    }

    private void OnDisable()
    {
        if (type == AudioType.Music)
        {
            SettingsManager.OnMusicVolumeChanged -= UpdateVolume;
        }
        else
        {
            SettingsManager.OnSFXVolumeChanged -= UpdateVolume;
        }
    }

    private void UpdateVolume(float vol)
    {
        audioSource.volume = baseVolume * vol;
    }
}
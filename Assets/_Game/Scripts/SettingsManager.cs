using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public static System.Action<float> OnMusicVolumeChanged;
    public static System.Action<float> OnSFXVolumeChanged;
    public static System.Action<float> OnSensitivityChanged;
    public static System.Action<bool> OnInvertLookChanged;

    [Header("UI Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider sensitivitySlider;

    [Header("UI Toggles")]
    [SerializeField] private TMP_Text fullscreenText;
    [SerializeField] private TMP_Text invertLookText;
    [SerializeField] private TMP_Text qualityText;

    private readonly string[] qualityLevels = { "Low", "Medium", "High" };
    private int currentQualityIndex = 2;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        LoadAndApplySettings();
    }

    public void LoadAndApplySettings()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;
        OnMusicVolumeChanged?.Invoke(musicVol);
        OnSFXVolumeChanged?.Invoke(sfxVol);

        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 2.0f);
        bool invertY = PlayerPrefs.GetInt("InvertLook", 0) == 1;
        sensitivitySlider.value = sensitivity;

        if (invertY)
        {
            invertLookText.text = "ON";
        }
        else
        {
            invertLookText.text = "OFF";
        }

        OnSensitivityChanged?.Invoke(sensitivity);
        OnInvertLookChanged?.Invoke(invertY);

        int defaultFullscreenValue;
        if (Screen.fullScreen)
        {
            defaultFullscreenValue = 1;
        }
        else
        {
            defaultFullscreenValue = 0;
        }

        bool isFull = PlayerPrefs.GetInt("Fullscreen", defaultFullscreenValue) == 1;
        Screen.fullScreen = isFull;

        if (isFull)
        {
            fullscreenText.text = "ON";
        }
        else
        {
            fullscreenText.text = "OFF";
        }

        currentQualityIndex = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        currentQualityIndex = Mathf.Clamp(currentQualityIndex, 0, qualityLevels.Length - 1);
        QualitySettings.SetQualityLevel(currentQualityIndex);
        qualityText.text = qualityLevels[currentQualityIndex];
    }

    public void SetMusicVolume(float val)
    {
        PlayerPrefs.SetFloat("MusicVolume", val);
        OnMusicVolumeChanged?.Invoke(val);
    }

    public void SetSFXVolume(float val)
    {
        PlayerPrefs.SetFloat("SFXVolume", val);
        OnSFXVolumeChanged?.Invoke(val);
    }

    public void SetSensitivity(float val)
    {
        PlayerPrefs.SetFloat("Sensitivity", val);
        OnSensitivityChanged?.Invoke(val);
    }

    public void ToggleInvertLook()
    {
        bool current = PlayerPrefs.GetInt("InvertLook", 0) == 1;
        bool next = !current;

        int nextInt;
        if (next)
        {
            nextInt = 1;
        }
        else
        {
            nextInt = 0;
        }
        PlayerPrefs.SetInt("InvertLook", nextInt);

        if (next)
        {
            invertLookText.text = "ON";
        }
        else
        {
            invertLookText.text = "OFF";
        }

        OnInvertLookChanged?.Invoke(next);
    }

    public void ToggleFullscreen()
    {
        bool next = !Screen.fullScreen;
        Screen.fullScreen = next;

        int nextInt;
        if (next)
        {
            nextInt = 1;
        }
        else
        {
            nextInt = 0;
        }
        PlayerPrefs.SetInt("Fullscreen", nextInt);

        if (next)
        {
            fullscreenText.text = "ON";
        }
        else
        {
            fullscreenText.text = "OFF";
        }
    }

    public void CycleQuality()
    {
        currentQualityIndex = (currentQualityIndex + 1) % qualityLevels.Length;
        QualitySettings.SetQualityLevel(currentQualityIndex);
        PlayerPrefs.SetInt("QualityLevel", currentQualityIndex);
        qualityText.text = qualityLevels[currentQualityIndex];
    }
}
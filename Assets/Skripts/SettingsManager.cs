using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public const string VOLUME_KEY = "MasterVolume";
    public const string ROTATION_SPEED_KEY = "RotationSpeed";

    public static event Action<float> OnRotationSpeedChanged;

    private static SettingsManager _instance;

    public static float CurrentVolume { get; private set; } = 80f;
    public static float CurrentRotationSpeed { get; private set; } = 100f;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Загружаем настройки сразу при создании
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Принудительно применяем настройки при старте
        ApplyVolumeSettings(CurrentVolume);
        ApplyRotationSpeedSettings(CurrentRotationSpeed);
    }

    private void LoadSettings()
    {
        CurrentVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 80f);
        CurrentRotationSpeed = PlayerPrefs.GetFloat(ROTATION_SPEED_KEY, 100f);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(VOLUME_KEY, CurrentVolume);
        PlayerPrefs.SetFloat(ROTATION_SPEED_KEY, CurrentRotationSpeed);
        PlayerPrefs.Save();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeSlidersInScene();
    }

    private void InitializeSlidersInScene()
    {
        CustomSliderController[] sliders = FindObjectsOfType<CustomSliderController>();

        foreach (var slider in sliders)
        {
            if (slider.gameObject.name == "VolumeSlider")
            {
                slider.SetValue(CurrentVolume);
                slider.OnValueChanged = ApplyVolumeSettings;
            }
            else if (slider.gameObject.name == "SpeedSlider")
            {
                slider.SetValue(CurrentRotationSpeed);
                slider.OnValueChanged = ApplyRotationSpeedSettings;
            }
        }
    }

    public void ApplyVolumeSettings(float value)
    {
        CurrentVolume = value;
        AudioListener.volume = value / 100f; // Применяем сразу
        SaveSettings();
    }

    public void ApplyRotationSpeedSettings(float value)
    {
        CurrentRotationSpeed = value;
        OnRotationSpeedChanged?.Invoke(value); // Уведомляем подписчиков
        SaveSettings();
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SaveSettings();
        }
    }
}
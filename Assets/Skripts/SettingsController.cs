using UnityEngine;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("Volume Control")]
    public CustomSliderController volumeSlider;

    [Header("Rotation Speed Control")]
    public CustomSliderController rotationSpeedSlider;

    private const string VOLUME_KEY = "MasterVolume";
    private const string ROTATION_SPEED_KEY = "RotationSpeed";

    void Start()
    {
        // Добавим проверку на null
        if (volumeSlider == null)
            Debug.LogError("Volume Slider is not assigned in SettingsController!");
        else
        {
            float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 80f);
            volumeSlider.SetValue(savedVolume);
            ApplyVolumeSettings(savedVolume);
        }

        if (rotationSpeedSlider == null)
            Debug.LogError("Rotation Speed Slider is not assigned in SettingsController!");
        else
        {
            float savedSpeed = PlayerPrefs.GetFloat(ROTATION_SPEED_KEY, 100f);
            rotationSpeedSlider.SetValue(savedSpeed);
            ApplyRotationSpeedSettings(savedSpeed);
        }
    }

    void Update()
    {
        if (volumeSlider != null && volumeSlider.IsDragging)
            ApplyVolumeSettings(volumeSlider.currentValue);

        if (rotationSpeedSlider != null && rotationSpeedSlider.IsDragging)
            ApplyRotationSpeedSettings(rotationSpeedSlider.currentValue);
    }

    private void ApplyVolumeSettings(float value)
    {
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        AudioListener.volume = value / 100f;
    }

    private void ApplyRotationSpeedSettings(float value)
    {
        PlayerPrefs.SetFloat(ROTATION_SPEED_KEY, value);
        PlayerPrefs.Save(); // Гарантируем немедленное сохранение
    }

    void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}
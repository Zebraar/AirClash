using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider goalsSlider;
    [SerializeField] private Text goalsText;
    [SerializeField] private Slider fpsSlider;
    [SerializeField] private Text fpsText;
    public float volumeLevel = 0.5f;

    void Start() {
        volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }

    public void OnVolumeSliderChanged() {
        PlayerPrefs.SetFloat("MusicVolume", volumeSlider.value);
        AudioListener.volume = volumeSlider.value;
    }
    public void OnGoalsSliderChanged()
    {
        PlayerPrefs.SetInt("Goals", Convert.ToInt32(goalsSlider.value));
        goalsText.text = Convert.ToString(Convert.ToInt32(goalsSlider.value));
    }
    public void OnFpsSliderChanged()
    {
        PlayerPrefs.SetInt("FPS", Convert.ToInt32(fpsSlider.value));
        fpsText.text = Convert.ToString(Convert.ToInt32(fpsSlider.value));
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS");
    }

    public void ShowTelegram()
    {
        Application.OpenURL("https://t.me/airclash_dev");
    }

    public void ShowGitHub()
    {
        Application.OpenURL("https://github.com/Zebraar/AirClash");
    }
}

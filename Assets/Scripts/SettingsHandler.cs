using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider goalsSlider;
    [SerializeField] private Text goalsText;
    [SerializeField] private Slider fpsSlider;
    [SerializeField] private Text fpsText;
    [SerializeField] private Toggle particleToggle;
    public float volumeLevel = 0.5f;

    public void Start() {
        QualitySettings.vSyncCount = 0;
        if(PlayerPrefs.GetInt("FPS") == 0) PlayerPrefs.SetInt("FPS", 60);
        else Application.targetFrameRate = PlayerPrefs.GetInt("FPS");
        if(PlayerPrefs.GetInt("MusicVolume") == 0) PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        else Application.targetFrameRate = PlayerPrefs.GetInt("MusicVolume");
        if(PlayerPrefs.GetInt("Goals") == 0) PlayerPrefs.SetInt("Goals", 5);
        else Application.targetFrameRate = PlayerPrefs.GetInt("Goals");
        volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        goalsSlider.value = PlayerPrefs.GetInt("Goals");
        fpsSlider.value = PlayerPrefs.GetInt("FPS");
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            if(PlayerPrefs.GetInt("Particle") == 0) particleToggle.isOn = false;
            else particleToggle.isOn = true;
        }
        
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

    public void OnParticleToggleChanged()
    {
        PlayerPrefs.SetInt("Particle", particleToggle.isOn ? 1 : 0);
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

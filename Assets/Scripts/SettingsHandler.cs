using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class IsAnimToggleEvent : UnityEvent<bool> { }
[System.Serializable]
public class IsFpsCounterEvent : UnityEvent<bool> { }
public class SettingsHandler : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider fpsSlider;

    [Header("Texts")]
    [SerializeField] private Text fpsText;

    [Header("Toggles")]
    [SerializeField] private Toggle animBgToggle;
    [SerializeField] private Toggle trailToggle;
    [SerializeField] private Toggle puckTrailToggle;
    [SerializeField] private Toggle fpsCounterToggle;
    [SerializeField] private Toggle bgMusicInGameToggle;

    [Header("Other")]
    [SerializeField] private IsAnimToggleEvent isAnimToggleEvent;
    [SerializeField] private IsFpsCounterEvent isFpsCounterEvent;

    public void Awake() 
    {
        QualitySettings.vSyncCount = 0;
        if(PlayerPrefs.GetInt("FPS", 0) == 0) PlayerPrefs.SetInt("FPS", 60);
        else Application.targetFrameRate = PlayerPrefs.GetInt("FPS");
        volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        fpsSlider.value = PlayerPrefs.GetInt("FPS");
        trailToggle.isOn = PlayerPrefs.GetInt("Trail", 1) != 0;
        animBgToggle.isOn = PlayerPrefs.GetInt("isAnimBg", 1) != 0;
        puckTrailToggle.isOn = PlayerPrefs.GetInt("PuckTrail", 1) != 0;
        fpsCounterToggle.isOn = PlayerPrefs.GetInt("FpsCounter", 0) != 0;
        bgMusicInGameToggle.isOn = PlayerPrefs.GetInt("BgMusicInGame", 1) != 0;
    }

    public void OnVolumeSliderChanged() {
        PlayerPrefs.SetFloat("MusicVolume", volumeSlider.value);
        AudioListener.volume = volumeSlider.value;
    }
    public void OnFpsSliderChanged()
    {
        PlayerPrefs.SetInt("FPS", Convert.ToInt32(fpsSlider.value));
        fpsText.text = Convert.ToString(Convert.ToInt32(fpsSlider.value));
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS");
    }

    public void OnTrailToggleChanged()
    {
        PlayerPrefs.SetInt("Trail", trailToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnAnimBgToggleChanged()
    {
        PlayerPrefs.SetInt("isAnimBg", animBgToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
        isAnimToggleEvent.Invoke(animBgToggle.isOn);
    }

    public void OnPuckTrailToggleChanged()
    {
        PlayerPrefs.SetInt("PuckTrail", puckTrailToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnFPSCounterToggleChanged()
    {
        PlayerPrefs.SetInt("FpsCounter", fpsCounterToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
        isFpsCounterEvent.Invoke(fpsCounterToggle.isOn);
    }

    public void OnBgMusicInGameToggleChanged()
    {
        PlayerPrefs.SetInt("BgMusicInGame", bgMusicInGameToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ShowTelegram()
    {
        Application.OpenURL("https://t.me/airclash_dev");
    }

    public void ShowGitHub()
    {
        Application.OpenURL("https://github.com/ZebrarsGames/AirClash");
    }

    public void ShowWebSite()
    {
        Application.OpenURL("https://zebrarsgames.github.io/AirClash/");
    }
}

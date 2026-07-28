using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Audio;
using TMPro;

[System.Serializable]
public class IsAnimToggleEvent : UnityEvent<bool> { }
[System.Serializable]
public class IsFpsCounterEvent : UnityEvent<bool> { }
public class SettingsHandler : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider fpsSlider;
    [SerializeField] private Slider bgMusicSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI fpsText;

    [Header("Toggles")]
    [SerializeField] private Toggle animBgToggle;
    [SerializeField] private Toggle trailToggle;
    [SerializeField] private Toggle puckTrailToggle;
    [SerializeField] private Toggle fpsCounterToggle;
    [SerializeField] private Toggle bgMusicInGameToggle;

    [Header("Other")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private IsAnimToggleEvent isAnimToggleEvent;
    [SerializeField] private IsFpsCounterEvent isFpsCounterEvent;

    private const string mixerParameterNameMaster = "Master";
    private const string mixerParameterNameSFX = "SFX";
    private const string mixerParameterNameBgMusic = "BgMusic";

    void Start() 
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 60);
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        float bgMusicVol = PlayerPrefs.GetFloat("BgMusicVolume", 1.0f);

        audioMixer.SetFloat(mixerParameterNameMaster, Mathf.Log10(masterVol) * 20);
        audioMixer.SetFloat(mixerParameterNameSFX, Mathf.Log10(sfxVol) * 20);
        audioMixer.SetFloat(mixerParameterNameBgMusic, Mathf.Log10(bgMusicVol) * 20);

        masterVolumeSlider.value = masterVol;
        bgMusicSlider.value = bgMusicVol;
        sfxVolumeSlider.value = sfxVol;
        fpsSlider.value = PlayerPrefs.GetInt("FPS", 60);
        trailToggle.isOn = PlayerPrefs.GetInt("Trail", 1) != 0;
        animBgToggle.isOn = PlayerPrefs.GetInt("isAnimBg", 1) != 0;
        puckTrailToggle.isOn = PlayerPrefs.GetInt("PuckTrail", 1) != 0;
        fpsCounterToggle.isOn = PlayerPrefs.GetInt("FpsCounter", 0) != 0;
        bgMusicInGameToggle.isOn = PlayerPrefs.GetInt("BgMusicInGame", 1) != 0;
    }

    public void OnVolumeSliderChanged() {
        float sliderValue = masterVolumeSlider.value;
        float dbValue = Mathf.Log10(sliderValue) * 20;
        audioMixer.SetFloat(mixerParameterNameMaster, dbValue);
    }
    public void OnBgMusicVolumeSliderChanged() {
        float sliderValue = bgMusicSlider.value;
        float dbValue = Mathf.Log10(sliderValue) * 20;
        audioMixer.SetFloat(mixerParameterNameBgMusic, dbValue);
    }
    public void OnSoundEffectsSliderChanged() {
        float sliderValue = sfxVolumeSlider.value;
        float dbValue = Mathf.Log10(sliderValue) * 20;
        audioMixer.SetFloat(mixerParameterNameSFX, dbValue);
    }
    public void OnFpsSliderChanged()
    {
        fpsText.text = fpsSlider.value.ToString();
        Application.targetFrameRate = Convert.ToInt32(fpsSlider.value);
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

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("FPS", Convert.ToInt32(fpsSlider.value));
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("BgMusicVolume", bgMusicSlider.value);
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.Save();
    }
}

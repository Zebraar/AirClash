using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Audio;

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
    [SerializeField] private Text fpsText;

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

    public void Awake() 
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 60);
        float masterVol = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
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
        float dbValue = Mathf.Log10(masterVolumeSlider.value) * 20;
        audioMixer.SetFloat(mixerParameterNameMaster, dbValue);

        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        PlayerPrefs.Save();
    }
    public void OnBgMusicVolumeSliderChanged() {
        float sliderValue = bgMusicSlider.value;
        float dbValue = Mathf.Log10(bgMusicSlider.value) * 20;
        audioMixer.SetFloat(mixerParameterNameBgMusic, dbValue);

        PlayerPrefs.SetFloat("BgMusicVolume", sliderValue);
        PlayerPrefs.Save();
    }
    public void OnSoundEffectsSliderChanged() {
        float sliderValue = sfxVolumeSlider.value;
        float dbValue = Mathf.Log10(sfxVolumeSlider.value) * 20;
        audioMixer.SetFloat(mixerParameterNameSFX, dbValue);

        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        PlayerPrefs.Save();
    }
    public void OnFpsSliderChanged()
    {
        PlayerPrefs.SetInt("FPS", Convert.ToInt32(fpsSlider.value));
        fpsText.text = Convert.ToString(Convert.ToInt32(fpsSlider.value));
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
}

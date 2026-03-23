using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    public float volumeLevel = 0.5f;

    void Start() {
        volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }

    public void OnSliderChanged() {
        PlayerPrefs.SetFloat("MusicVolume", volumeSlider.value);
        AudioListener.volume = volumeSlider.value;
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

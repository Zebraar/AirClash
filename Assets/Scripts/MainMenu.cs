using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject onlinePanel;
    [SerializeField] private GameObject botPanel;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuMusic;

    void Start()
    {
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.Play();
    }


    public void PlayBots(string difficulty)
    {
        switch (difficulty)
        {
            case "VeryEasy":
                PlayerPrefs.SetInt("Difficulty", 1);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Easy":
                PlayerPrefs.SetInt("Difficulty", 2);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Medium":
                PlayerPrefs.SetInt("Difficulty", 5);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Hard":
                PlayerPrefs.SetInt("Difficulty", 7);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Extreme":
                PlayerPrefs.SetInt("Difficulty", 10);
                SceneManager.LoadScene("BotsGame");
                break;
            default:
                PlayerPrefs.SetInt("Difficulty", 2);
                break;
        }
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        onlinePanel.SetActive(false);
        botPanel.SetActive(false);
    }

    public void OpenOnline()
    {
        settingsPanel.SetActive(false);
        onlinePanel.SetActive(true);
        botPanel.SetActive(false);
    }

    public void OpenBot()
    {
        settingsPanel.SetActive(false);
        onlinePanel.SetActive(false);
        botPanel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        settingsPanel.SetActive(false);
        onlinePanel.SetActive(false);
        botPanel.SetActive(false);
    }

    public void VeryEasyMode()
    {
        PlayBots("VeryEasy");
    }
    public void Easy()
    {
        PlayBots("Easy");
    }
    public void Normal()
    {
        PlayBots("Medium");
    }
    public void Hard()
    {
        PlayBots("Hard");
    }
    public void Extreme()
    {
        PlayBots("Extreme");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }


}

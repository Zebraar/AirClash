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
                PlayerPrefs.SetFloat("Difficulty", 2f);
                PlayerPrefs.SetFloat("Range1", -0.5f);
                PlayerPrefs.SetFloat("Range2", 0.5f);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Easy":
                PlayerPrefs.SetFloat("Difficulty", 3f);
                PlayerPrefs.SetFloat("Range1", -0.4f);
                PlayerPrefs.SetFloat("Range2", 0.4f);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Medium":
                PlayerPrefs.SetFloat("Difficulty", 6f);
                PlayerPrefs.SetFloat("Range1", -0.2f);
                PlayerPrefs.SetFloat("Range2", 0.2f);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Hard":
                PlayerPrefs.SetFloat("Difficulty", 15f);
                PlayerPrefs.SetFloat("Range1", -0.1f);
                PlayerPrefs.SetFloat("Range2", 0.1f);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Extreme":
                PlayerPrefs.SetFloat("Difficulty", 20f);
                PlayerPrefs.SetFloat("Range1", 0f);
                PlayerPrefs.SetFloat("Range2", 0f);
                SceneManager.LoadScene("BotsGame");
                break;
            default:
                PlayerPrefs.SetFloat("Difficulty", 2f);
                PlayerPrefs.SetFloat("Range1", -2f);
                PlayerPrefs.SetFloat("Range2", 2f);
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

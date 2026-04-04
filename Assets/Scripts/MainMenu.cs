using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject onlinePanel;
    [SerializeField] private GameObject botPanel;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private Text mainMenuText;
    [SerializeField] private Text moneyText;
    [SerializeField] private MoneyHandler moneyHandler;
    public float rotationSpeed = 10f;
    private RectTransform rectTransform;

    void Start()
    {
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.Play();
        rectTransform = mainMenuText.GetComponent<RectTransform>();
        moneyText.text = "Деньги " + Convert.ToString(PlayerPrefs.GetInt("Money"));
        QualitySettings.vSyncCount = 0;
        if(PlayerPrefs.GetInt("FPS") == 0) Application.targetFrameRate = 60;
        else Application.targetFrameRate = PlayerPrefs.GetInt("FPS");
    }

    void Update()
    {
        rectTransform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        float currentAngle = rectTransform.localEulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360;

        if (currentAngle >= 6 || currentAngle <= -6)
        {
            rotationSpeed *= -1;
        
            float clampedAngle = Mathf.Clamp(currentAngle, -12, 12);
            rectTransform.localEulerAngles = new Vector3(0, 0, clampedAngle);
        }
    }


    public void PlayBots(string difficulty)
    {
        switch (difficulty)
        {
            case "VeryEasy":
                PlayerPrefs.SetFloat("Difficulty", 3.1415926535f);
                PlayerPrefs.SetInt("HowMoneyAdd", 3);
                PlayerPrefs.SetInt("HowMoneyRemove", 1);
                PlayerPrefs.Save();
                SceneManager.LoadScene("BotsGame");
                break;
            case "Easy":
                PlayerPrefs.SetFloat("Difficulty", 7.5f);
                PlayerPrefs.SetInt("HowMoneyAdd", 4);
                PlayerPrefs.SetInt("HowMoneyRemove", 1);
                PlayerPrefs.Save();
                SceneManager.LoadScene("BotsGame");
                break;
            case "Medium":
                PlayerPrefs.SetFloat("Difficulty", 13.5f);
                PlayerPrefs.SetInt("HowMoneyAdd", 6);
                PlayerPrefs.SetInt("HowMoneyRemove", 3);
                PlayerPrefs.Save();
                SceneManager.LoadScene("BotsGame");
                break;
            case "Hard":
                PlayerPrefs.SetFloat("Difficulty", 25f);
                PlayerPrefs.SetInt("HowMoneyAdd", 10);
                PlayerPrefs.SetInt("HowMoneyRemove", 5);
                PlayerPrefs.Save();
                SceneManager.LoadScene("BotsGame");
                break;
            case "Extreme":
                PlayerPrefs.SetFloat("Difficulty", 50f);
                PlayerPrefs.SetInt("HowMoneyAdd", 20);
                PlayerPrefs.SetInt("HowMoneyRemove", 15);
                PlayerPrefs.Save();
                SceneManager.LoadScene("BotsGame");
                break;
            default:
                PlayerPrefs.SetFloat("Difficulty", 2f);
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

    public void OpenShop()
    {
        SceneManager.LoadScene("ShopScene");
    }

    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}

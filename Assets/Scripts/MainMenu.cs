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
    [SerializeField] private GameObject mentionsPanel;
    [SerializeField] private GameObject achievementsPanel;
    [SerializeField] private GameObject gamemodesPanel;
    [SerializeField] private GameObject userGamemodePanel;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuMusic;
    [Header("UI Elements")]
    [SerializeField] private Text mainMenuText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Slider goalsSlider;
    [SerializeField] private Text goalsText;
    [Header("Scripts")]
    [SerializeField] private MoneyHandler moneyHandler;
    [SerializeField] private CoinMover coinMover;
    [Header("Floats")]
    public float rotationSpeed = 10f;
    private RectTransform rectTransform;
    private string toScene;

    void Start()
    {
        Debug.Log("Версия игры до обновления Unity");
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.Play();
        rectTransform = mainMenuText.GetComponent<RectTransform>();
        moneyText.text = "Деньги " + Convert.ToString(PlayerPrefs.GetInt("Money"));
        if(PlayerPrefs.GetInt("isAfterGame") == 0)
        {
            PlayerPrefs.SetInt("HowMoneyAdds", 0);
            PlayerPrefs.SetInt("isAfterGame", 0);
            PlayerPrefs.Save();    
        } else
        {
            AddMoney(PlayerPrefs.GetInt("HowMoneyAdds"));
            PlayerPrefs.SetInt("isAfterGame", 0);
            PlayerPrefs.Save();
        }
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
                PlayerPrefs.SetInt("HowMoneyAdd", 1);
                PlayerPrefs.SetInt("HowMoneyAddAsLose", 1);
                PlayerPrefs.SetFloat("BotOffsetX", 1f);
                PlayerPrefs.SetFloat("BotOffsetY", 0.8f);
                break;
            case "Easy":
                PlayerPrefs.SetFloat("Difficulty", 7.5f);
                PlayerPrefs.SetInt("HowMoneyAdd", 3);
                PlayerPrefs.SetInt("HowMoneyAddAsLose", 2);
                PlayerPrefs.SetFloat("BotOffsetX", 0.7f);
                PlayerPrefs.SetFloat("BotOffsetY", 0.7f);
                break;
            case "Medium":
                PlayerPrefs.SetFloat("Difficulty", 13.5f);
                PlayerPrefs.SetInt("HowMoneyAdd", 4);
                PlayerPrefs.SetInt("HowMoneyAddAsLose", 2);
                PlayerPrefs.SetFloat("BotOffsetX", 0.4f);
                PlayerPrefs.SetFloat("BotOffsetY", 0.5f);
                break;
            case "Hard":
                PlayerPrefs.SetFloat("Difficulty", 25f);
                PlayerPrefs.SetInt("HowMoneyAdd", 7);
                PlayerPrefs.SetInt("HowMoneyAddAsLose", 2);
                PlayerPrefs.SetFloat("BotOffsetX", 0.3f);
                PlayerPrefs.SetFloat("BotOffsetY", 0.2f);
                break;
            case "Extreme":
                PlayerPrefs.SetFloat("Difficulty", 50f);
                PlayerPrefs.SetInt("HowMoneyAdd", 15);
                PlayerPrefs.SetInt("HowMoneyAddAsLose", 1);
                PlayerPrefs.SetFloat("BotOffsetX", 0.15f);
                PlayerPrefs.SetFloat("BotOffsetY", 0.1f);
                break;
            default:
                PlayerPrefs.SetFloat("Difficulty", 2f);
                break;
        }
        PlayerPrefs.Save();
        toScene = "BotsGame";
        CloseAllPanels();
        gamemodesPanel.SetActive(true);
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
        mentionsPanel.SetActive(false);
        achievementsPanel.SetActive(false);
        gamemodesPanel.SetActive(false);
        userGamemodePanel.SetActive(false);
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
        toScene = "GameScene";
        CloseAllPanels();
        gamemodesPanel.SetActive(true);
    }

    public void OpenShop()
    {
        SceneManager.LoadScene("ShopScene");
    }

    public void OpenMentions()
    {
        mentionsPanel.SetActive(true);
    }
    public void OpenAchievements()
    {
        achievementsPanel.SetActive(true);
    }
    public void OpenUserGamemode()
    {
        CloseAllPanels();
        userGamemodePanel.SetActive(true);
        if(PlayerPrefs.GetInt("Goals") == 0) PlayerPrefs.SetInt("Goals", 5);
        else PlayerPrefs.SetInt("Goals", PlayerPrefs.GetInt("Goals"));
        goalsSlider.value = PlayerPrefs.GetInt("Goals");
        goalsText.text = Convert.ToString(PlayerPrefs.GetInt("Goals"));
    }

    public void AddMoney(int amount)
    {
        coinMover.AddCoins(new Vector3(0, 0, 0), amount);
    }
    public void SetGamemode(int howManyGoals)
    {
        PlayerPrefs.SetInt("Goals", howManyGoals);
        PlayerPrefs.Save();
        SceneManager.LoadScene(toScene);
    }
    public void SetUserGamemode()
    {
        PlayerPrefs.SetInt("Goals", Convert.ToInt32(goalsSlider.value));
        PlayerPrefs.Save();
        SceneManager.LoadScene(toScene);
    }
    public void OnGoalsSliderChanged()
    {
        goalsText.text = Convert.ToString(Convert.ToInt32(goalsSlider.value));
    }
}

using System;
using System.Collections;
using DG.Tweening;
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
    [SerializeField] private GameObject xpPanel;
    [SerializeField] private GameObject questPanel;
    [SerializeField] private GameObject dailyQuestPanel;
    [SerializeField] private GameObject profilePanel;
    [SerializeField] private GameObject editProfilePanel;
    [SerializeField] private GameObject cloudPanel;
    [SerializeField] private GameObject modificatorsPanel;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuMusic;
    [Header("UI Elements")]
    [SerializeField] private Text mainMenuText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Slider goalsSlider;
    [SerializeField] private Text goalsText;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Text speedText;
    [SerializeField] private GameObject userGamemodeBtn;
    [SerializeField] private GameObject speedPanel;
    [Header("Scripts")]
    [SerializeField] private MoneyHandler moneyHandler;
    [SerializeField] private CoinMover coinMover;
    [SerializeField] private XpHandler xpHandler;
    [SerializeField] private QuestsHandler questsHandler; 
    [SerializeField] private DailyQuestHandler dailyQuestHandler;
    [SerializeField] private SaveManager saveManager;
    [Header("Floats")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float maxAngle = 6f;  
    private RectTransform rectTransform;
    private string toScene;
    private string lastOpenMenu;

    void Awake()
    {
        if(PlayerPrefs.GetInt("IsFirstTimePlayed", 1) == 1)
        {
            saveManager.SaveDefaultData();
            PlayerPrefs.SetInt("IsFirstTimePlayed", 0);
            PlayerPrefs.Save();
        } 
    }
    void Start()
    {
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.time = PlayerPrefs.GetFloat("MainMenuMusicTime", 0);
        audioSource.Play();
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 60);
        rectTransform = mainMenuText.GetComponent<RectTransform>();
        moneyText.text = "Деньги " + moneyHandler.GetMoney(); 
        saveManager.SaveData();
        if(PlayerPrefs.GetInt("isAfterGame", 0) == 0)
        {
            PlayerPrefs.SetInt("HowMoneyAdds", 0);
            PlayerPrefs.SetInt("HowXpAdds", 0);
            PlayerPrefs.SetInt("isAfterGame", 0);
            PlayerPrefs.Save();    
        } else
        {
            AddMoney(PlayerPrefs.GetInt("HowMoneyAdds"));
            UpdateQuests(PlayerPrefs.GetInt("HowMoneyAdds"));
            PlayerPrefs.SetInt("isAfterGame", 0);
            PlayerPrefs.SetInt("HowMoneyAdds", 0);
            PlayerPrefs.SetInt("HowXpAdds", 0);
            PlayerPrefs.Save();
        }
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * rotationSpeed) * maxAngle;
        rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public void PlayBots(string difficulty)
    {
        switch(difficulty)
        {
            case "VeryEasy":
                PlayerPrefs.SetFloat("Difficulty", 5f);
                PlayerPrefs.SetInt("HowMoneyAdd", 2);
                PlayerPrefs.SetInt("HowMoneyAddAsLose", 1);
                PlayerPrefs.SetInt("HowManyAddXp", 3);
                PlayerPrefs.SetFloat("BotOffsetX", 1f);
                PlayerPrefs.SetFloat("BotOffsetY", 0.8f);
                break;
            case "Easy":
                PlayerPrefs.SetFloat("Difficulty", 7.5f);
                PlayerPrefs.SetInt("HowMoneyAdd", 3);
                PlayerPrefs.SetInt("HowMoneyAddAsLose", 2);
                PlayerPrefs.SetInt("HowManyAddXp", 7);
                PlayerPrefs.SetFloat("BotOffsetX", 0.7f);
                PlayerPrefs.SetFloat("BotOffsetY", 0.7f);
                break;
            case "Medium":
                PlayerPrefs.SetFloat("Difficulty", 12f);
                PlayerPrefs.SetInt("HowMoneyAdd", 5);
                PlayerPrefs.SetInt("HowMoneyAddAsLose", 2);
                PlayerPrefs.SetInt("HowManyAddXp", 10);
                PlayerPrefs.SetFloat("BotOffsetX", 0.4f);
                PlayerPrefs.SetFloat("BotOffsetY", 0.5f);
                break;
            case "Hard":
                PlayerPrefs.SetFloat("Difficulty", 17.5f);
                PlayerPrefs.SetInt("HowMoneyAdd", 10);
                PlayerPrefs.SetInt("HowMoneyAddAsLose", 2);
                PlayerPrefs.SetInt("HowManyAddXp", 20);
                PlayerPrefs.SetFloat("BotOffsetX", 0.3f);
                PlayerPrefs.SetFloat("BotOffsetY", 0.2f);
                break;
            case "Extreme":
                PlayerPrefs.SetFloat("Difficulty", 27.5f);
                PlayerPrefs.SetInt("HowMoneyAdd", 20);
                PlayerPrefs.SetInt("HowMoneyAddAsLose", 1);
                PlayerPrefs.SetInt("HowManyAddXp", 30);
                PlayerPrefs.SetFloat("BotOffsetX", 0.15f);
                PlayerPrefs.SetFloat("BotOffsetY", 0.1f);
                break;
            default:
                PlayerPrefs.SetFloat("Difficulty", 11.5f);
                break;
        }
        PlayerPrefs.Save();
        toScene = "BotsGame";
        CloseAllPanels();
        gamemodesPanel.SetActive(true);
        OnGamemodePanel();
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
        xpPanel.SetActive(false);
        questPanel.SetActive(false);
        dailyQuestPanel.SetActive(false);
        profilePanel.SetActive(false);
        editProfilePanel.SetActive(false);
        cloudPanel.SetActive(false);
        modificatorsPanel.SetActive(false);
    }
    public void ClosePanel(GameObject panel)
    {
        var rect = panel.GetComponent<RectTransform>();
        rect.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() => 
            {
                panel.SetActive(false);
                rect.localScale = Vector3.one;
            });
    }
    public void OpenPanel(GameObject panel)
    {
        var rect = panel.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
        panel.SetActive(true);
        rect.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.3f).SetEase(Ease.OutBack);
    }
    public void OnBotBtn()
    {
        toScene = "BotsGame";
    }
    public void OnGamemodePanel()
    {
        lastOpenMenu = "GamemodePanel";
        if(toScene == "GameScene")
        {
            userGamemodeBtn.SetActive(true);
        } else if(toScene == "BotsGame")
        {
            userGamemodeBtn.SetActive(false);
        }
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

    public void StartGame()
    {
        toScene = "GameScene";
    }

    public void OpenShop()
    {
        PlayerPrefs.SetFloat("MainMenuMusicTime", audioSource.time);
        PlayerPrefs.Save();
        SceneManager.LoadScene("ShopScene");
    }
    public void SwitchToQuestPanel()
    {
        dailyQuestPanel.SetActive(false);
        questPanel.SetActive(true);
    }
    public void SwitchToEditProfilePanel()
    {
        profilePanel.SetActive(false);
        editProfilePanel.SetActive(true);
    }
    public void SwithcToProfilePanel()
    {
        editProfilePanel.SetActive(false);
        cloudPanel.SetActive(false);
        profilePanel.SetActive(true);
    }
    public void SwithcToCloudPanel()
    {
        profilePanel.SetActive(false);
        cloudPanel.SetActive(true);
    }
    public void SwithcToModificatorsPanel()
    {
        CloseAllPanels();
        modificatorsPanel.SetActive(true);
    }

    public void SwitchToGameModesPanel()
    {
        CloseAllPanels();
        if(lastOpenMenu == "GamemodePanel")
        {
            gamemodesPanel.SetActive(true);
            OnGamemodePanel();
        } else if(lastOpenMenu == "UserGameModePanel")
        {
            OpenUserGamemode();
        }
    }
    public void OpenDailyQuestPanel()
    {
        questPanel.SetActive(false);
        dailyQuestPanel.SetActive(true);
    }
    public void OpenUserGamemode()
    {
        CloseAllPanels();
        userGamemodePanel.SetActive(true);
        lastOpenMenu = "UserGameModePanel";
        if(toScene == "BotsGame")
        {
            goalsSlider.value = PlayerPrefs.GetInt("Goals", 4);
            speedSlider.value = PlayerPrefs.GetFloat("Difficulty", 7.5f);
            speedPanel.SetActive(true);
            float value = speedSlider.value;
            speedText.text = $"{value:F1}";
            goalsText.text = PlayerPrefs.GetInt("Goals", 4).ToString();
        } else if(toScene == "GameScene")
        {
            goalsSlider.value = PlayerPrefs.GetInt("Goals", 4);
            speedSlider.value = PlayerPrefs.GetFloat("Difficulty", 7.5f);
            speedPanel.SetActive(false);
        }
    }

    public void OnGoalsSliderChanged()
    {
        goalsText.text = Convert.ToInt32(goalsSlider.value).ToString();
    }

    public void OnSpeedSliderChanged()
    {
        float value = speedSlider.value;
        speedText.text = $"{value:F1}";
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
        PlayerPrefs.SetFloat("Difficulty", speedSlider.value);
        PlayerPrefs.Save();
        SceneManager.LoadScene(toScene);
    }
    public void LoadToScene()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene(toScene);
    }
    private void UpdateQuests(int amount)
    {
        questsHandler.UpdateQuestProgress("money10", amount);
        questsHandler.UpdateQuestProgress("money50", amount);
        questsHandler.UpdateQuestProgress("money100", amount);
        questsHandler.UpdateQuestProgress("money200", amount);
        questsHandler.UpdateQuestProgress("money300", amount);
        questsHandler.UpdateQuestProgress("money500", amount);
        dailyQuestHandler.UpdateQuestProgress("daily_money50", amount);
        dailyQuestHandler.UpdateQuestProgress("money70", amount);
        dailyQuestHandler.UpdateQuestProgress("daily_money100", amount);
    }
}

using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class CloudUIScr : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;

    [Header("Texts")]
    [SerializeField] private Text statusText;

    [Header("Buttons")]
    [SerializeField] private Button[] buttons;

    [Header("Panels")]
    [SerializeField] private GameObject surePanel;
    [SerializeField] private GameObject cloudPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sureSound;

    [Header("Scripts")]
    [SerializeField] private QuestsHandler questsHandler;
    [SerializeField] private DailyQuestHandler dailyQuestHandler;
    [SerializeField] private AchievementsHandler achievementsHandler;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private MoneyHandler moneyHandler;
    [SerializeField] private XpHandler xpHandler;
    [SerializeField] private FirebaseManager firebaseManager;

    void Start()
    {
        usernameInput.text = saveManager.GetData().NickName;
    }

    public void OnClickLoginOrRegister()
    {
        firebaseManager.AccountAuth(usernameInput.text, passwordInput.text);
    }

    public void OnClickSave()
    {
        firebaseManager.SaveProgress(usernameInput.text, passwordInput.text);
    }

    public void OnClickLoad()
    {
        firebaseManager.LoadProgress(usernameInput.text, passwordInput.text);
    }

    public void OnInputField()
    {
        cloudPanel.GetComponent<RectTransform>().DOLocalMoveY(228, 0.3f).SetEase(Ease.OutSine);
    }

    public void OnInputFieldEnd()
    {
        cloudPanel.GetComponent<RectTransform>().DOLocalMoveY(0, 0.3f).SetEase(Ease.OutSine);
    }

    public void SetStatusText(string status)
    {
        statusText.text = "Статус: " + status;
    }

    public void SetActiveBtns(bool isActive)
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = !isActive;
        }
    }

    public void SetPlayerData(PlayerData playerData)
    {
        if(playerData != null)
        {
            SetMoneyQuests(playerData.TotalMoney);
            SetXpQuests(playerData.XP);
            SetGoalQuests(playerData.Goals);
            PlayerPrefs.SetString("Nick", usernameInput.text);
            PlayerPrefs.SetInt("TotalGoals", playerData.Goals);
            PlayerPrefs.SetString("CurrentSkin", playerData.CurrentSkinName);
            moneyHandler.SetMoney(playerData.Money);
            moneyHandler.SetTotalMoney(playerData.TotalMoney);
            xpHandler.SetLevel(playerData.XpLevel);
            xpHandler.SetTotalXp(playerData.TotalXP);
            xpHandler.SetXp(playerData.XP);
            xpHandler.SetXpToNextLevel(playerData.XpToNextLevel);
            PlaytimeTracker.Instance.SetSecondsPlaytime(playerData.Playtime);

            int achievementsCount = achievementsHandler.GetCountOfAchievements();
            for(int i = 0; i < achievementsCount; i++)
            {
                string id = achievementsHandler.GetStringId(i);
                achievementsHandler.SetProgress(id, playerData.AchievementsProgress[i]);
            }

            string[] parts = playerData.AllBuySkins;
            for(int i = 0; i < parts.Length; i++)
            {
                PlayerPrefs.SetInt(parts[i], 1);
            }

            PlayerPrefs.Save();
            saveManager.SaveData();
        } else
        {
            saveManager.SaveDefaultData();
        }
    }

    public void ShowSurePanel()
    {
        audioSource.PlayOneShot(sureSound);
        var rect = surePanel.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
        surePanel.SetActive(true);
        rect.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void NoSurePanel()
    {
        var rect = surePanel.GetComponent<RectTransform>();
        rect.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => surePanel.SetActive(false));
        rect.localScale = Vector3.one;
    }

    public void YesSurePanel()
    {
        firebaseManager.DeleteAccount(usernameInput.text, passwordInput.text);
        var rect = surePanel.GetComponent<RectTransform>();
        rect.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => surePanel.SetActive(false));
        rect.localScale = Vector3.one;
    }

    private void SetMoneyQuests(int amount)
    {
        questsHandler.SetQuestProgress("money10", amount);
        questsHandler.SetQuestProgress("money50", amount);
        questsHandler.SetQuestProgress("money100", amount);
        questsHandler.SetQuestProgress("money200", amount);
        questsHandler.SetQuestProgress("money300", amount);
        questsHandler.SetQuestProgress("money500", amount);
        dailyQuestHandler.UpdateQuestProgress("daily_money50", amount);
        dailyQuestHandler.UpdateQuestProgress("money70", amount);
        dailyQuestHandler.UpdateQuestProgress("daily_money100", amount);
    }

    private void SetXpQuests(int amount)
    {
        questsHandler.SetQuestProgress("xp100", amount);
        questsHandler.SetQuestProgress("xp200", amount);
        questsHandler.SetQuestProgress("xp400", amount);
        questsHandler.SetQuestProgress("xp500", amount);
        questsHandler.SetQuestProgress("xp700", amount);
        questsHandler.SetQuestProgress("xp1000", amount);
        dailyQuestHandler.UpdateQuestProgress("xp50", amount);
    }

    private void SetGoalQuests(int amount)
    {
        questsHandler.SetQuestProgress("goal10", amount);
        questsHandler.SetQuestProgress("goal50", amount);
        questsHandler.SetQuestProgress("goal100", amount);
        questsHandler.SetQuestProgress("goal200", amount);
        questsHandler.SetQuestProgress("goal300", amount);
        questsHandler.SetQuestProgress("goal500", amount);
        dailyQuestHandler.UpdateQuestProgress("goal20", amount);
    }
}

using UnityEngine;
using System.IO;
using System;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private XpHandler xpHandler;
    [SerializeField] private AchievementsHandler achievementsHandler;
    [SerializeField] private MoneyHandler moneyHandler;

    public void SaveData()
    {
        PlayerData playerData = new PlayerData();
        playerData.Money = moneyHandler.GetMoney();
        playerData.XP = xpHandler.GetXP();
        playerData.XpLevel = xpHandler.GetLevel();
        playerData.XpToNextLevel = xpHandler.GetXpToNextLevel();
        playerData.Goals = PlayerPrefs.GetInt("TotalGoals", 0);
        playerData.NickName = PlayerPrefs.GetString("Nick", GetData().NickName);
        playerData.CurrentSkinName = PlayerPrefs.GetString("CurrentSkin", "DefSkin");
        playerData.Playtime = PlaytimeTracker.Instance.GetSecondsPlaytime();
        playerData.TotalMoney = moneyHandler.GetTotalMoney();
        playerData.TotalXP = xpHandler.GetTotalXP();

        int achievementsCount = achievementsHandler.GetCountOfAchievements();

        playerData.AchievementsIds = new string[achievementsCount];
        playerData.AchievementsProgress = new int[achievementsCount];

        for(int i = 0; i < achievementsCount; i++)
        {
            string id = achievementsHandler.GetStringId(i);
            playerData.AchievementsIds[i] = id;
            playerData.AchievementsProgress[i] = achievementsHandler.GetProgress(id);
        }

        string skins = PlayerPrefs.GetString("AllBuySkins", "DefSkin");
        string[] parts = skins.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        playerData.AllBuySkins = parts;

        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
        PlayerPrefs.Save();
        Debug.Log("Data Saved!");
    }

    public PlayerData GetData()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.json"))
        {
            Debug.Log("No save file");
            return new PlayerData();
        }
        string json = File.ReadAllText(Application.persistentDataPath + "/save.json");
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
        return playerData;
    }

    public void DeleteData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.json"))
        {
            File.Delete(Application.persistentDataPath + "/save.json");
            File.Delete(Path.Combine(Application.persistentDataPath, "avatar.png"));
            Debug.Log("Файл сохранения успешно удален по пути: " + Application.persistentDataPath + "/save.json");
        }
        else
        {
            Debug.LogWarning("Файл сохранения не найден");
        }
    }

    public void SaveDefaultData()
    {
        PlayerData playerData = new PlayerData();
        playerData.Money = PlayerPrefs.GetInt("Money", 0);
        playerData.XP = PlayerPrefs.GetInt("CurrentXp", 0);
        playerData.XpLevel = PlayerPrefs.GetInt("XpLevel", 1);;
        playerData.XpToNextLevel = PlayerPrefs.GetInt("XpToNextLevel", 100);;
        playerData.Goals = 0;
        playerData.NickName = "Ник";
        playerData.CurrentSkinName = PlayerPrefs.GetString("CurrentSkin", "DefSkin");
        playerData.Playtime = 0;
        playerData.TotalMoney = PlayerPrefs.GetInt("Money", 0);
        playerData.TotalXP = PlayerPrefs.GetInt("CurrentXp", 0);

        int achievementsCount = achievementsHandler.GetCountOfAchievements();

        playerData.AchievementsIds = new string[achievementsCount];
        playerData.AchievementsProgress = new int[achievementsCount];

        for(int i = 0; i < achievementsCount; i++)
        {
            string id = achievementsHandler.GetStringId(i);
            PlayerPrefs.SetInt(id + "_unlocked", 0); 
            playerData.AchievementsIds[i] = id;
            playerData.AchievementsProgress[i] = 0;
        }

        string skins = "DefSkin";
        string[] parts = skins.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        playerData.AllBuySkins = parts;

        PlayerPrefs.Save();
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
        Debug.Log("Default data Saved!");
    }

    public PlayerData GetDefaultData()
    {
        PlayerData playerData = new PlayerData();
        playerData.Money = PlayerPrefs.GetInt("Money", 0);
        playerData.XP = PlayerPrefs.GetInt("CurrentXp", 0);
        playerData.XpLevel = PlayerPrefs.GetInt("XpLevel", 1);;
        playerData.XpToNextLevel = PlayerPrefs.GetInt("XpToNextLevel", 100);;
        playerData.Goals = 0;
        playerData.NickName = "Ник";
        playerData.CurrentSkinName = PlayerPrefs.GetString("CurrentSkin", "DefSkin");
        playerData.Playtime = 0;
        playerData.TotalMoney = PlayerPrefs.GetInt("Money", 0);
        playerData.TotalXP = PlayerPrefs.GetInt("CurrentXp", 0);

        int achievementsCount = achievementsHandler.GetCountOfAchievements();

        playerData.AchievementsIds = new string[achievementsCount];
        playerData.AchievementsProgress = new int[achievementsCount];

        for(int i = 0; i < achievementsCount; i++)
        {
            string id = achievementsHandler.GetStringId(i);
            PlayerPrefs.SetInt(id + "_unlocked", 0); 
            playerData.AchievementsIds[i] = id;
            playerData.AchievementsProgress[i] = 0;
        }

        string skins = "DefSkin";
        string[] parts = skins.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        playerData.AllBuySkins = parts;

        PlayerPrefs.Save();
        return playerData;
    }
}

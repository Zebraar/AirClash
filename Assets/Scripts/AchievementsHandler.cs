using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class AchievementsHandler : MonoBehaviour
{
    
    public class Achievement
    {
        public string Title;
        public bool IsUnlocked;
        public int Progress;
        public int Target;
    }

    private Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();

    void Start()
    {
        achievements.Add("a_start_has_been_made", new Achievement { Title = "Начало положено", Target = 1 });
        achievements.Add("begginer", new Achievement { Title = "Новичок", Target = 10 });
        achievements.Add("amateur", new Achievement {Title = "Любитель", Target = 30});
        LoadAchievements();
    }

    public void UpdateProgress(string id, int amount)
    {
        if (achievements.TryGetValue(id, out Achievement ach))
        {
            if (ach.IsUnlocked) return;

            ach.Progress += amount;

            PlayerPrefs.SetInt(id, ach.Progress);
        
            if (ach.Progress >= ach.Target)
            {
                ach.IsUnlocked = true;
                PlayerPrefs.SetInt(id + "_unlocked", 1); 
                Debug.Log($"Достижение получено: {ach.Title}");
            }
        
            PlayerPrefs.Save();
        }
    }

    public void LoadAchievements()
    {
        foreach (var pair in achievements)
        {
            string id = pair.Key;
            Achievement ach = pair.Value;

            if (PlayerPrefs.GetInt(id + "_unlocked", 0) == 1)
            {
                ach.IsUnlocked = true;
                ach.Progress = ach.Target;
                Debug.Log("Достижение " + ach.Title + " разблокировано");
            }
            else
            {
                ach.Progress = PlayerPrefs.GetInt(id, 0);
            }
        }
    }

    public bool GetUnlocked(string id)
    {
        if(achievements.TryGetValue(id, out Achievement ach))
        {
            return ach.IsUnlocked;
        } else return false;
    }
    public int GetTarget(string id)
    {
        if(achievements.TryGetValue(id, out Achievement ach))
        {
            return ach.Target;
        } else return 0;
    }
    public int GetProgress(string id)
    {
        if(achievements.TryGetValue(id, out Achievement ach))
        {
            return ach.Progress;
        } else return 0;
    }
}

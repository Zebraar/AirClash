using System.Collections.Generic;
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
        // Ачивки на голы
        achievements.Add("a_start_has_been_made", new Achievement { Title = "Начало положено", Target = 1 });
        achievements.Add("begginer", new Achievement { Title = "Новичок", Target = 10 });
        achievements.Add("amateur", new Achievement {Title = "Любитель", Target = 30});
        achievements.Add("professional", new Achievement {Title = "Профессионал", Target = 70});
        achievements.Add("master", new Achievement {Title = "Мастер", Target = 100});
        achievements.Add("world_champion", new Achievement {Title = "Всемирный чемпион", Target = 400});
        achievements.Add("best_in_the_galaxy", new Achievement {Title = "Лучший в галактике", Target = 1000});
        achievements.Add("best_in_the_universe", new Achievement {Title = "Лучший во вселенной", Target = 10000});
        // Ачивки на победы над ботами
        achievements.Add("light_warm-up", new Achievement { Title = "Лёгкая разминка", Target = 1});
        achievements.Add("warm-up", new Achievement { Title = "Разминка", Target = 1});
        achievements.Add("training", new Achievement { Title = "Тренировка", Target = 1});
        achievements.Add("fight", new Achievement { Title = "Бой", Target = 1});
        achievements.Add("competitions", new Achievement { Title = "Соревнования", Target = 1});
        // Ачивки от подписчиков
        achievements.Add("ten", new Achievement {Title = "Десятка", Target = 10});
        achievements.Add("seriously", new Achievement {Title = "Серьёзно?", Target = 1});
        achievements.Add("large_wardrobe", new Achievement {Title = "Большой гардероб", Target = 7});
        achievements.Add("own_goal", new Achievement {Title = "Автогол", Target = 2});
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

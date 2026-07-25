using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementsHandler : MonoBehaviour
{
    [SerializeField] private AnimationsHandler animationsHandler;
    
    public class Achievement
    {
        public string Title;
        public bool IsUnlocked;
        public int Progress;
        public int Target;
        public int Award;
    }

    private Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();
    public int GetCountOfAchievements() => achievements.Count;

    void Awake()
    {
        // Ачивки на голы
        achievements.Add("a_start_has_been_made", new Achievement { Title = "Начало положено", Target = 1, Award = 5});
        achievements.Add("begginer", new Achievement { Title = "Новичок", Target = 10, Award = 10});
        achievements.Add("amateur", new Achievement {Title = "Любитель", Target = 30, Award = 20});
        achievements.Add("professional", new Achievement {Title = "Профессионал", Target = 70, Award = 50});
        achievements.Add("master", new Achievement {Title = "Мастер", Target = 100, Award = 80});
        achievements.Add("world_champion", new Achievement {Title = "Всемирный чемпион", Target = 400, Award = 200});
        achievements.Add("best_in_the_galaxy", new Achievement {Title = "Лучший в галактике", Target = 1000, Award = 400});
        achievements.Add("best_in_the_universe", new Achievement {Title = "Лучший во вселенной", Target = 10000, Award = 2000});
        // Ачивки на победы над ботами
        achievements.Add("warm-up", new Achievement { Title = "Разминка", Target = 1, Award = 10});
        achievements.Add("training", new Achievement { Title = "Тренировка", Target = 1, Award = 20});
        achievements.Add("fight", new Achievement { Title = "Бой", Target = 1, Award = 50});
        achievements.Add("competitions", new Achievement { Title = "Соревнования", Target = 1, Award = 100});
        // Ачивки от подписчиков
        achievements.Add("ten", new Achievement {Title = "Десятка", Target = 10, Award = 25});
        achievements.Add("seriously", new Achievement {Title = "Серьёзно?", Target = 1, Award = 5});
        achievements.Add("large_wardrobe", new Achievement {Title = "Большой гардероб", Target = 25, Award = 600});
        achievements.Add("own_goal", new Achievement {Title = "Автогол", Target = 2, Award = 15});
        // Ачивки для рулетки
        achievements.Add("ludoman", new Achievement {Title = "Лудоман", Target = 15, Award = 35});
        achievements.Add("lucky", new Achievement {Title = "Везунчик", Target = 1, Award = 150});
        achievements.Add("six_seven", new Achievement {Title = "Сикс севен", Target = 3, Award = 50});
        // Ачивки для XP
        achievements.Add("first_steps", new Achievement {Title = "Первые шаги", Target = 2, Award = 30});
        achievements.Add("regular_player", new Achievement {Title = "Постоянный игрок", Target = 5, Award = 70});
        achievements.Add("thunderstorm_game", new Achievement {Title = "Гроза игры", Target = 7, Award = 100});
        achievements.Add("game_legend", new Achievement {Title = "Легенда игры", Target = 10, Award = 300});
        // Ачивки для квестов
        achievements.Add("coin_master", new Achievement {Title = "Мастер монет", Target = 6, Award = 200});
        achievements.Add("master_of_goals", new Achievement {Title = "Мастер голов", Target = 6, Award = 200});
        achievements.Add("master_xp", new Achievement {Title = "Мастер XP", Target = 6, Award = 200});
        achievements.Add("daily", new Achievement {Title = "Ежедневка", Target = 3, Award = 40});
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
                PlayerPrefs.SetInt("HowXpAdds", PlayerPrefs.GetInt("HowXpAdds") + ach.Award);
                PlayerPrefs.SetInt(id + "_unlocked", 1); 
                animationsHandler.ShowAchievement(ach.Title);
            }
        
            PlayerPrefs.Save();
        }
    }

    public void SetProgress(string id, int amount)
    {
        if (achievements.TryGetValue(id, out Achievement ach))
        {
            if (ach.IsUnlocked) return;

            ach.Progress = amount;

            PlayerPrefs.SetInt(id, ach.Progress);
        
            if (ach.Progress >= ach.Target)
            {
                ach.IsUnlocked = true;
                PlayerPrefs.SetInt("HowXpAdds", PlayerPrefs.GetInt("HowXpAdds") + ach.Award);
                PlayerPrefs.SetInt(id + "_unlocked", 1); 
                animationsHandler.ShowAchievement(ach.Title);
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
    public string GetStringId(int id)
    {
        if(achievements.Keys.ElementAt(id) != null)
        {
            return achievements.Keys.ElementAt(id);
        } else return "";
    }
}

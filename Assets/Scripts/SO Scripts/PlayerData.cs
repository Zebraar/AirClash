using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string NickName;
    public int Money;
    public int TotalMoney;
    public int XP;
    public int TotalXP;
    public int XpLevel;
    public int XpToNextLevel;
    public int Goals;
    public string CurrentSkinName;
    public float Playtime;
    public string[] AchievementsIds;
    public string[] AllBuySkins;
    public int[] AchievementsProgress;
    public string avatarBase64; 
}

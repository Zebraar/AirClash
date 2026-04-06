using UnityEngine;
using UnityEngine.UI;

public class AchievementItem : MonoBehaviour
{
    [SerializeField] private AchievementsHandler achievementsHandler;
    public string Title;
    private bool IsUnlocked;
    private int Progress;
    private int Target;
    [SerializeField] private Text progressText;
    [SerializeField] private GameObject unlockedCheckMark;
    void Start()
    {
        achievementsHandler.LoadAchievements();
        IsUnlocked = achievementsHandler.GetUnlocked(Title);
        Progress = achievementsHandler.GetProgress(Title);
        Target = achievementsHandler.GetTarget(Title);
        progressText.text = Progress.ToString() + "/" + Target.ToString();
        if(Progress == Target) unlockedCheckMark.SetActive(true);
        else unlockedCheckMark.SetActive(false);
    }
}

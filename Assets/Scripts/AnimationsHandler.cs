using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimationsHandler : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private Text achievementText;
    [Header("Floats")]
    public float achievementSpeed;
    public Vector3 endDotToMoveAchievementPanel;
    private Vector3 startPosAchievementPanel;
    [Header("Other")]
    private RectTransform achievementPanelTransform;

    void Awake()
    {
        achievementPanelTransform = achievementPanel.GetComponent<RectTransform>(); 
        startPosAchievementPanel = achievementPanelTransform.anchoredPosition;
    }
    public void ShowAchievement(string achievementTitle)
    {
        achievementPanel.SetActive(true);
        achievementText.text = achievementTitle;
        StartCoroutine(AchievementCoroutine());
    }
    IEnumerator AchievementCoroutine()
    {
        achievementPanelTransform.DOKill();

        achievementPanelTransform.DOAnchorPos(endDotToMoveAchievementPanel, 2.0f);

        yield return new WaitForSeconds(3f);

        achievementPanelTransform.DOAnchorPos(startPosAchievementPanel, 2.0f);

        yield return new WaitForSeconds(2f);
        achievementPanel.SetActive(false);
    }
}

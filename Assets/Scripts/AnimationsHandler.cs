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
    public Vector2 endDotToMoveAchievementPanel;
    private Vector2 startPosAchievementPanel;
    [Header("Other")]
    private RectTransform achievementPanelTransform;
    private Coroutine currentCoroutine;

    void Awake()
    {
        achievementPanelTransform = achievementPanel.GetComponent<RectTransform>(); 
        startPosAchievementPanel = achievementPanelTransform.anchoredPosition;
    }

    public void ShowAchievement(string achievementTitle)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        
        achievementPanel.SetActive(true);
        achievementText.text = achievementTitle;
        currentCoroutine = StartCoroutine(AchievementCoroutine());
    }

    IEnumerator AchievementCoroutine()
    {
        achievementPanelTransform.DOKill();

        achievementPanelTransform.DOAnchorPos(endDotToMoveAchievementPanel, 2.0f)
            .SetLink(achievementPanel);

        yield return new WaitForSeconds(3f);

        achievementPanelTransform.DOAnchorPos(startPosAchievementPanel, 2.0f)
            .SetLink(achievementPanel);

        yield return new WaitForSeconds(2f);
        
        achievementPanel.SetActive(false);
    }
}
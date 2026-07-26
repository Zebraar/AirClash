using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class XpUiScr : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider xpSlider;
    // [SerializeField] private Text awardForNextLevel;
    [SerializeField] private Text currentXpText;
    [SerializeField] private Text currentLvlText;
    [SerializeField] private Text nextLvlText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject panel;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] levelUpSounds;
    
    [Header("Scripts")]
    [SerializeField] private XpHandler xpHandler;

    private bool isAnim;
    public bool GetIsAnim() => isAnim;
    void Start()
    {
        if(SceneManager.GetActiveScene().name.Equals("BotsGame"))
        {
            SetOldProgress(xpHandler.GetOldXPProgress());
        } else if(SceneManager.GetActiveScene().name.Equals("GameScene"))
        {
            SetProgress(xpHandler.GetXPProgress());
        } else
        {
            SetProgress(xpHandler.GetXPProgress());
            // if(xpHandler.GetSkinAwardForNextLevel() != null)
            // {
            //     awardForNextLevel.text = "Награда за следующий уровень: " + xpHandler.GetMoneyAwardForNextLevel().Award + " монет и скин " + xpHandler.GetSkinAwardForNextLevel().GuiSkinName ;
            // } else
            // {
            //     awardForNextLevel.text = "Награда за следующий уровень: " + xpHandler.GetMoneyAwardForNextLevel().Award + " монет";
            // }
        }
    }

    public void SetProgress(float progress)
    {
        xpSlider.value = progress;
        currentXpText.text = xpHandler.GetXP().ToString() + " / " + xpHandler.GetXpToNextLevel().ToString() + " XP";
        currentLvlText.text = xpHandler.GetLevel().ToString();
        nextLvlText.text = (xpHandler.GetLevel() + 1).ToString();
    }
    public void SetProgress(float progress, int currentXP)
    {
        xpSlider.value = progress;
        currentXpText.text = currentXP.ToString() + " / " + xpHandler.GetXpToNextLevel().ToString() + " XP";
        currentLvlText.text = xpHandler.GetLevel().ToString();
        nextLvlText.text = (xpHandler.GetLevel() + 1).ToString();
    }
    public void SetOldProgress(float progress)
    {
        xpSlider.value = progress;
        currentXpText.text = xpHandler.GetOldXP().ToString() + " / " + xpHandler.GetXpToNextLevel().ToString() + " XP";
        currentLvlText.text = xpHandler.GetLevel().ToString();
        nextLvlText.text = (xpHandler.GetLevel() + 1).ToString();
    }

    public void LevelUpAnimStart()
    {
        StartCoroutine(LevelUpAnim());
    }

    IEnumerator LevelUpAnim()
    {
        isAnim = true;
        yield return new WaitForSeconds(1.1f);
        int rand = UnityEngine.Random.Range(0, levelUpSounds.Length);
        audioSource.PlayOneShot(levelUpSounds[rand]);
        panel.transform.DOScale(1.5f, 0.4f).OnComplete(() => panel.transform.DOScale(1.0f, 0.2f));
        canvasGroup.DOFade(1.0f, 0.4f).OnComplete(() => canvasGroup.DOFade(0f, 0.2f));
        yield return new WaitForSeconds(0.2f);
        SetProgress(xpHandler.GetXPProgress());
        isAnim = false;
    }
}


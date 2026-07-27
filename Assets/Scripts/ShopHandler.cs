using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopHandler : MonoBehaviour
{
    [Header("Economy and Progress")]
    [SerializeField] private MoneyHandler moneyHandler;
    [SerializeField] private Text moneyText;
    [SerializeField] private AchievementsHandler achievementsHandler;

    [Header("Shop and Skins")]
    [SerializeField] private SkinItem[] allSkins;
    [SerializeField] private GameObject surePanel;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSourceBgMusic;
    [SerializeField] private AudioClip shopMusic;
    [SerializeField] private AudioClip buySound;
    [SerializeField] private AudioClip cancelSound;
    [SerializeField] private AudioClip warningSound;

    [Header("Other")]
    [SerializeField] private SaveManager saveManager;


    void Start()
    {
        moneyText.text = "Деньги " + Convert.ToString(moneyHandler.GetMoney());
        audioSourceBgMusic.clip = shopMusic;
        audioSourceBgMusic.loop = true;
        audioSourceBgMusic.time = PlayerPrefs.GetFloat("ShopMusicTime", 0);
        audioSourceBgMusic.Play();
        string currentSkin = PlayerPrefs.GetString("CurrentSkin", "DefSkin");
        EquipSkin(currentSkin);
    }
    public void CloseShop()
    {
        PlayerPrefs.SetFloat("ShopMusicTime", audioSourceBgMusic.time);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }

    public bool BuySkin(string skinName, int skinCoast)
    {
        if(moneyHandler.GetMoney() >= skinCoast && PlayerPrefs.GetInt(skinName) == 0)
        {
            achievementsHandler.UpdateProgress("large_wardrobe", 1);
            audioSource.PlayOneShot(buySound);
            moneyHandler.RemoveMoney(skinCoast);
            PlayerPrefs.SetString("CurrentSkin", skinName);
            PlayerPrefs.SetInt(skinName, 1);
            PlayerPrefs.Save();
            moneyText.text = "Деньги " + Convert.ToString(moneyHandler.GetMoney());
            return true;
        } else
        {
            audioSource.PlayOneShot(cancelSound);
            return false;
        } 
    }

    public void EquipSkin(string skinName)
    {
        PlayerPrefs.SetString("CurrentSkin", skinName);
        PlayerPrefs.Save();

        foreach (var skin in allSkins)
        {
            skin.equipArrow.gameObject.SetActive(skin.skinName == skinName);
        }
    }

    public void PlayCancelSound()
    {
        audioSource.PlayOneShot(cancelSound);
    }


    public void RemoveAllMoney()
    {
        moneyHandler.RemoveMoney(moneyHandler.GetMoney());
        moneyText.text = "Деньги " + Convert.ToString(moneyHandler.GetMoney());
    }

    public void ShowSurePanel()
    {
        audioSource.PlayOneShot(warningSound);
        var rect = surePanel.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
        surePanel.SetActive(true);
        rect.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f).SetEase(Ease.OutBack);
    }
    public void HideSurePanel()
    {
        StartCoroutine(AnimateSurePanel());
    }
    IEnumerator AnimateSurePanel()
    {
        var rect = surePanel.GetComponent<RectTransform>();
        rect.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        yield return new WaitForSeconds(0.35f);
        surePanel.SetActive(false);
    }
    public void DeletePlayerPrefs()
    {
        int fps = PlayerPrefs.GetInt("FPS");
        float musicVoulme = PlayerPrefs.GetFloat("MusicVolume");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("FPS", fps);
        Application.targetFrameRate = fps;
        PlayerPrefs.SetFloat("MusicVolume", musicVoulme);
        saveManager.DeleteData();
        PlayerPrefs.Save();
    }

    public void PlusMoney(int money)
    {
        moneyHandler.AddMoney(money);
        moneyText.text = "Деньги " + Convert.ToString(moneyHandler.GetMoney());
    }

}

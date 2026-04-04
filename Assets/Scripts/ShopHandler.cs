using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopHandler : MonoBehaviour
{
    [SerializeField] private MoneyHandler moneyHandler;
    [SerializeField] private Text moneyText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip cancelSound;
    [SerializeField] private AudioClip buySound;
    [SerializeField] private SkinItem[] allSkins; 

    void Start()
    {
        moneyText.text = "Деньги " + Convert.ToString(moneyHandler.GetMoney());
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.Play();
        string currentSkin = PlayerPrefs.GetString("CurrentSkin", "DefSkin");
        EquipSkin(currentSkin);
    }
    public void CloseShop()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public bool BuySkin(string skinName, int skinCoast)
    {
        if(moneyHandler.GetMoney() >= skinCoast && PlayerPrefs.GetInt(skinName) == 0)
        {
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

    public void PlusMoney()
    {
        moneyHandler.AddMoney(100);
        moneyText.text = "Деньги " + Convert.ToString(moneyHandler.GetMoney());
    }

}

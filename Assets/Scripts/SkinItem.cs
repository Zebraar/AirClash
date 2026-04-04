using UnityEngine;

public class SkinItem : MonoBehaviour
{
    public string skinName;
    public int skinPrice;
    public ShopHandler shop;
    public bool isBuy;
    public UnityEngine.UI.Image checkmark;
    public UnityEngine.UI.Image equipArrow;

    void Start()
    {
        if(PlayerPrefs.GetInt(skinName) == 1)
        {
            isBuy = true;
            checkmark.gameObject.SetActive(true);
        }
    }

    public void OnClickBuy() 
    {
        if(!isBuy)
        {
            isBuy = shop.BuySkin(skinName, skinPrice);      
            if(isBuy) 
            {
                checkmark.gameObject.SetActive(true);
                shop.EquipSkin(skinName);
            }
        } 
        else
        {
            shop.EquipSkin(skinName);
        }
    }  

}

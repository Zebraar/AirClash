using UnityEngine;

public class SkinItem : MonoBehaviour
{
    public string skinName;
    public int skinPrice;
    public ShopHandler shop;
    public bool isBuy;
    public UnityEngine.UI.Image checkmark;

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
            shop.BuySkin(skinName, skinPrice);
            isBuy = shop.BuySkin(skinName, skinPrice);
            if(isBuy) checkmark.gameObject.SetActive(true);
        } else
        {
            shop.EquipSkin(skinName);
        }
    }
}

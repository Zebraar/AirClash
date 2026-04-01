using UnityEngine;

public class SkinItem : MonoBehaviour
{
    public string skinName;
    public int skinPrice;
    public ShopHandler shop;
    public bool isBuy;

    public void OnClickBuy() 
    {
        if(!isBuy)
        {
            shop.BuySkin(skinName, skinPrice);
            isBuy = shop.BuySkin(skinName, skinPrice);
        } else
        {
            shop.EquipSkin(skinName);
        }
    }
}

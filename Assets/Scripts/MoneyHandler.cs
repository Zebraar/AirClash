using UnityEngine;

public class MoneyHandler : MonoBehaviour
{
    private int money;

    void Awake()
    {
        money = PlayerPrefs.GetInt("Money", 0);
    }

    public void AddMoney(int amount)
    {
        money += amount;
        Save();
    }

    public void RemoveMoney(int amount)
    {
        money -= amount;
        money = Mathf.Max(0, money);
        Save();
    }

    public int GetMoney()
    {
        return money;
    }

    private void Save()
    {
        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.Save();
    }
}
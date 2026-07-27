using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ModificatorsHandler : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text moneyMultiplyText;
    [SerializeField] private Toggle[] modificatorsToggles;

    public void SetModificators()
    {
        List<string> modificatorsStrings = new List<string>();
        for(int i = 0; i < modificatorsToggles.Length; i++)
        {
            if(modificatorsToggles[i].isOn)
            {
                modificatorsStrings.Add(modificatorsToggles[i].GetComponent<ModidficatorToggleItem>().ModificatorName);
            }
        }
        string result = string.Join(",", modificatorsStrings);
        if(result.Equals(""))
        {
            PlayerPrefs.SetString("CurrentModificators", "None");
        } else
        {
            PlayerPrefs.SetString("CurrentModificators", result);
        }
        PlayerPrefs.Save();
    }
    public void OnValueChanged()
    {
        float modificatorsMultiply = 1;
        for(int i = 0; i < modificatorsToggles.Length; i++)
        {
            if(modificatorsToggles[i].isOn)
            {
                modificatorsMultiply += modificatorsToggles[i].GetComponent<ModidficatorToggleItem>().ModificatorXMoney;
            }
        }
        moneyMultiplyText.text = "Деньги " + modificatorsMultiply + "x";
    }
}

using UnityEngine;
using Mirror;
using TMPro;

public class EOSMenuUI : MonoBehaviour
{
    public NetworkManager networkManager;
    public TMP_InputField idInputField;
    public TextMeshProUGUI myIdText;

    private void Start()
    {
        InvokeRepeating(nameof(UpdateMyID), 1f, 1f);
    }

    private void UpdateMyID()
    {
        var eos = networkManager.GetComponent<EpicTransport.EOSSDKComponent>();
        if(eos != null && !string.IsNullOrEmpty(EpicTransport.EOSSDKComponent.LocalUserProductIdString))
        {
            myIdText.text = "Мой EOS ID: " + EpicTransport.EOSSDKComponent.LocalUserProductIdString;
            CancelInvoke(nameof(UpdateMyID));
        }
    }

    public void StartHostGame()
    {

        if(!EpicTransport.EOSSDKComponent.Initialized) 
        {
            Debug.LogWarning("[EOS] SDK status is not Initialized yet! Please wait...");
            return;
        }

        networkManager.StartHost();
    }

    public void JoinClientGame()
    {
        if (!string.IsNullOrEmpty(idInputField.text))
        {
            networkManager.networkAddress = idInputField.text.Trim();
            networkManager.StartClient();
        }
    }
}
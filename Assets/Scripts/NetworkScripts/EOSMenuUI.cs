using System.Collections;
using UnityEngine;
using Mirror;
using TMPro;
using EpicTransport;
using DG.Tweening;

public class EOSMenuUI : MonoBehaviour
{
    public NetworkManager networkManager;
    public TMP_InputField idInputField;
    public TextMeshProUGUI myIdText;
    public RoomManager roomManager;
    public GameObject uiPanel;

    private string currentEosId = string.Empty;

    private void Start()
    {
        uiPanel.SetActive(true);
        myIdText.text = "Авторизация в Epic Games...";
        StartCoroutine(WaitForEOSLoginRoutine());
    }

    private IEnumerator WaitForEOSLoginRoutine()
    {
        while(!EOSSDKComponent.Initialized)
        {
            yield return new WaitForSeconds(0.2f);
        }

        currentEosId = EOSSDKComponent.LocalUserProductIdString;

        if(string.IsNullOrEmpty(currentEosId) && EOSSDKComponent.LocalUserProductId != null)
        {
            currentEosId = EOSSDKComponent.LocalUserProductId.ToString();
        }

        myIdText.text = "Мой EOS ID: " + currentEosId;
        Debug.Log($"EOS ID: {currentEosId}");
    }

    public void StartHostGame()
    {
        if(string.IsNullOrEmpty(currentEosId))
        {
            currentEosId = EOSSDKComponent.LocalUserProductIdString;
        }

        if(string.IsNullOrEmpty(currentEosId) || !EOSSDKComponent.Initialized)
        {
            myIdText.text = "Подождите, идёт авторизация EOS...";
            Debug.LogWarning("Попытка создать комнату до завершения авторизации Epic!");
            return;
        }

        myIdText.text = "Создание комнаты...";

        roomManager.CreateRoom(currentEosId, 
        (roomId) =>
        {
            myIdText.text = "Код комнаты: " + roomId;
            if(networkManager is MyNetworkManager customManager)
            {
                customManager.SetCurrentRoomCode(roomId);
            }
            Debug.Log($"Комната успешно создана на сервере! Код: {roomId}");
            var group = uiPanel.GetComponent<CanvasGroup>();
            group.DOFade(0, 1f).OnComplete(() => uiPanel.SetActive(false));
            
            networkManager.StartHost();
        },
        (errorText) =>
        {
            myIdText.text = "Ошибка создания: " + errorText;
            Debug.LogError($"Ошибка сервера Firebase: {errorText}");
        });
    }

    public void JoinClientGame()
    {
        string inputCode = idInputField.text.Trim().ToUpper();

        if(string.IsNullOrEmpty(inputCode))
        {
            myIdText.text = "Введите код комнаты!";
            return;
        }

        myIdText.text = "Поиск комнаты...";

        roomManager.JoinRoom(inputCode, 
        (roomEosId) =>
        {
            myIdText.text = "Подключение к " + inputCode + "...";
            Debug.Log($"Успешно получен EOS ID хоста: {roomEosId}");
            var group = uiPanel.GetComponent<CanvasGroup>();
            group.DOFade(0, 0.5f);
            
            networkManager.networkAddress = roomEosId.Trim();
            networkManager.StartClient();
        },
        (errorText) =>
        {
            myIdText.text = "Комната не найдена: " + errorText;
            Debug.LogError($"Ошибка при поиске комнаты: {errorText}");
        });
    }
}
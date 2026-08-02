using UnityEngine;
using Mirror;
using TMPro;

public class EOSMenuUI : MonoBehaviour
{
    public NetworkManager networkManager;
    public TMP_InputField idInputField; // Ссылка на UI поле ввода (куда вставлять ID хоста)
    public TextMeshProUGUI myIdText;          // Текст для отображения личного ID игрока

    private void Start()
    {
        // Каждую секунду проверяем, получил ли EOS наш Product ID, и выводим его на экран
        InvokeRepeating(nameof(UpdateMyID), 1f, 1f);
    }

    private void UpdateMyID()
    {
        // Ищем компонент EOS на объекте NetworkManager
        var eos = networkManager.GetComponent<EpicTransport.EOSSDKComponent>();
        if (eos != null && !string.IsNullOrEmpty(EpicTransport.EOSSDKComponent.LocalUserProductIdString))
        {
            myIdText.text = "Мой EOS ID: " + EpicTransport.EOSSDKComponent.LocalUserProductIdString;
            CancelInvoke(nameof(UpdateMyID)); // Останавливаем проверку, когда ID найден
        }
    }

    // Повесьте этот метод на кнопку "Создать игру" (Host)
    public void StartHostGame()
    {
        networkManager.StartHost();
    }

    // Повесьте этот метод на кнопку "Подключиться" (Client)
    public void JoinClientGame()
    {
        if (!string.IsNullOrEmpty(idInputField.text))
        {
            // Самое главное: заменяем адрес подключения на ID из поля ввода
            networkManager.networkAddress = idInputField.text.Trim();
            networkManager.StartClient();
        }
    }
}
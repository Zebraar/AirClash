using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class DisconnectUIManager : MonoBehaviour
{
    public static DisconnectUIManager Instance;

    [SerializeField] private GameObject timerCanvas;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject disconnectText;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        NetworkClient.OnDisconnectedEvent += OnClientDisconnectedFromServer;
    }

    private void OnDisable()
    {
        NetworkClient.OnDisconnectedEvent -= OnClientDisconnectedFromServer;
    }

    private void OnClientDisconnectedFromServer()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowDisconnectUI()
    {
        if(timerCanvas != null) timerCanvas.SetActive(true);
        if(timerPanel != null) timerPanel.SetActive(false);
        
        if(disconnectText != null)
        {
            RectTransform rect = disconnectText.GetComponent<RectTransform>();
            rect.localScale = Vector3.zero;
            disconnectText.SetActive(true);
            rect.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        }
    }
}
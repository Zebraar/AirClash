using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DisconnectUIManager : MonoBehaviour
{
    [SerializeField] private GameObject timerCanvas;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject disconnectText;

    private void OnEnable()
    {
        MyNetworkManager.OnClientDisconnected += HandleDisconnect;
    }

    private void OnDisable()
    {
        MyNetworkManager.OnClientDisconnected -= HandleDisconnect;
    }

    private void HandleDisconnect()
    {
        ShowDisconnectUI();
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
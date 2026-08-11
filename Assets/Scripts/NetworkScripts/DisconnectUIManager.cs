using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;
using Mirror;

public class DisconnectUIManager : MonoBehaviour
{
    [SerializeField] private GameObject timerCanvas;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject disconnectText;

    private Coroutine disconnectCoroutine;

    private void OnEnable()
    {
        MyNetworkManager.OnOpponentDisconnected += HandleDisconnect;
    }

    private void OnDisable()
    {
        MyNetworkManager.OnOpponentDisconnected -= HandleDisconnect;
    }

    private void HandleDisconnect()
    {
        if(disconnectCoroutine == null)
        {
            disconnectCoroutine = StartCoroutine(ShowDisconnectUI());
        }
    }

    private IEnumerator ShowDisconnectUI()
    {
        if(timerCanvas != null) timerCanvas.SetActive(true);
        if(timerPanel != null) timerPanel.SetActive(false);

        if(disconnectText != null)
        {
            RectTransform rect = disconnectText.GetComponent<RectTransform>();
            
            rect.DOKill(); 
            rect.localScale = Vector3.zero;
            disconnectText.SetActive(true);
            
            rect.DOScale(Vector3.one, 0.4f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true); 
        }

        yield return new WaitForSecondsRealtime(3.5f);

        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if(NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void OnDestroy()
    {
        if(disconnectText != null)
        {
            disconnectText.transform.DOKill();
        }
    }
}
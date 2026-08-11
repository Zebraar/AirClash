using System.Collections;
using UnityEngine;
using TMPro;
using Mirror;

public class PingDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI pingText;

    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.5f;

    private Coroutine pingCoroutine;
    private bool isOfflineDisplayed;
    private const string OfflineText = "Ping: Offline";

    private void OnEnable()
    {
        pingCoroutine = StartCoroutine(UpdatePingRoutine());
    }

    private void OnDisable()
    {
        if(pingCoroutine != null)
        {
            StopCoroutine(pingCoroutine);
            pingCoroutine = null;
        }
    }

    private IEnumerator UpdatePingRoutine()
    {
        var delay = new WaitForSeconds(updateInterval);

        while(true)
        {
            if(!NetworkClient.active)
            {
                if (!isOfflineDisplayed)
                {
                    pingText.text = OfflineText;
                    isOfflineDisplayed = true;
                }
            }
            else
            {
                isOfflineDisplayed = false;

                int pingMs = Mathf.RoundToInt((float)(NetworkTime.rtt * 1000));

                pingText.SetText("Ping: {0}", pingMs);
            }

            yield return delay;
        }
    }
}
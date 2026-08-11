using System.Collections;
using UnityEngine;
using TMPro;
using Mirror;

public class PingDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI pingText;
    [SerializeField] private TextMeshProUGUI warningText; 

    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.5f;

    [Header("Ping Thresholds (ms)")]
    [SerializeField] private int highPingThreshold = 300;
    [SerializeField] private int criticalPingThreshold = 800;

    [Header("Colors")]
    [SerializeField] private Color goodColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;

    private Coroutine pingCoroutine;
    private bool isOfflineDisplayed;
    private const string OfflineText = "Ping: Offline";
    private const string ConnectionUnstableText = "Нестабильное соединение!";

    private void OnEnable()
    {
        if(warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }

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
                if(!isOfflineDisplayed)
                {
                    pingText.text = OfflineText;
                    pingText.color = criticalColor;

                    if(warningText != null)
                    {
                        warningText.gameObject.SetActive(false);
                    }

                    isOfflineDisplayed = true;
                }
            }
            else
            {
                isOfflineDisplayed = false;

                int pingMs = Mathf.RoundToInt((float)(NetworkTime.rtt * 1000));

                pingText.SetText("Ping: {0}", pingMs);

                UpdatePingVisuals(pingMs);
            }

            yield return delay;
        }
    }

    private void UpdatePingVisuals(int pingMs)
    {
        if(pingMs >= criticalPingThreshold)
        {
            pingText.color = criticalColor;

            if(warningText != null)
            {
                warningText.text = ConnectionUnstableText;
                warningText.color = criticalColor;
                warningText.gameObject.SetActive(true);
            }
        }
        else if(pingMs >= highPingThreshold)
        {
            pingText.color = warningColor;

            if(warningText != null)
            {
                warningText.gameObject.SetActive(false);
            }
        }
        else
        {
            pingText.color = goodColor;

            if(warningText != null)
            {
                warningText.gameObject.SetActive(false);
            }
        }
    }
}
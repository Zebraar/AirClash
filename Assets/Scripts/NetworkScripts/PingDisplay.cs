using UnityEngine;
using TMPro;
using Mirror;

public class PingDisplay : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI pingText;
    [Header("Floats")]
    [SerializeField] private float updateInterval = 0.5f;

    private float timer;

    void Update()
    {
        if(!NetworkClient.active)
        {
            pingText.text = "Ping: Offline";
            return;
        }

        timer += Time.deltaTime;

        if(timer >= updateInterval)
        {
            double ping = NetworkTime.rtt * 1000;

            pingText.text = $"Ping: {System.Math.Round(ping)} ms";

            timer = 0f;
        }
    }
}
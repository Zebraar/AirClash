using UnityEngine;
using UnityEngine.UI;

public class TimerScr : MonoBehaviour
{
    
    public Text TimerText;
    public GameObject TimerCanvas;
    public int TimeLeft = 4;
    public bool TimerOn = false;

    [SerializeField] private Button restartBtn;


    public void Goal()
    {
        TimerCanvas.gameObject.SetActive(true);
        TimerText.text = "GOAL";
        TimerOn = true;
        restartBtn.interactable = false;
        Invoke("TimerStart", 1f);
    }

    public void TimerStart()
    {
        TimerCanvas.gameObject.SetActive(true);
        TimerText.text = "4";
        TimeLeft = 4;
        TimerOn = true;
        restartBtn.interactable = false;
        InvokeRepeating("Timer", 0f, 1f);
    }

    private void Timer()
    {
        TimeLeft--;
        TimerText.text = TimeLeft.ToString();

        if (TimeLeft <= 0)
        {
            CancelInvoke("Timer");
            TimerCanvas.gameObject.SetActive(false);
            TimerOn = false;
            restartBtn.interactable = true;
        }
    }
}

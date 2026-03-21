using UnityEngine;
using UnityEngine.UI;

public class GoalHandler : MonoBehaviour
{
    [SerializeField] private Text scoreText1;
    [SerializeField] private Text scoreText2;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    private Vector2 player1startPos;
    private Vector2 player2startPos;
    private Vector2 puckStartPos;
    public GameObject puck;
    private int score1 = 0;
    private int score2 = 0;
    [SerializeField] private TimerScr timer;

    void Awake()
    {
        player1startPos = player1.transform.position;
        player2startPos = player2.transform.position;
        puckStartPos = puck.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GoalTrigger1"))
        {
            score1++;
            scoreText1.text = score1.ToString();  
            ResetPosition();
            timer.TimerStart();
        }
        else if (collision.gameObject.CompareTag("GoalTrigger2"))
        {
            score2++;
            scoreText2.text = score2.ToString(); 
            ResetPosition();
            timer.TimerStart();
        }
    }

    public void ResetPosition()
    {
        if (puck != null)
        {
            puck.transform.position = puckStartPos;
            player1.transform.position = player1startPos;
            player1.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            player2.transform.position = player2startPos;
            player2.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            Rigidbody2D rb = puck.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    public void RestartGame()
    {
        score1 = 0;
        score2 = 0;
        scoreText1.text = "0";
        scoreText2.text = "0";
        ResetPosition();
        timer.TimerStart();
    }
}

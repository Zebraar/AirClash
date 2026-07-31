using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoalHandlerNetwork : NetworkBehaviour
{
    public static GoalHandlerNetwork Instance;

    [Header("UI Elements")]
    public Text scoreText1;
    public Text scoreText2;
    [SerializeField] private TextMeshProUGUI winOrLoseText;
    [SerializeField] private GameObject goalTextCanvas;
    [SerializeField] private GameObject endSreenPanel;

    [Header("Players & Puck")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    public GameObject puck;

    [Header("Positions")]
    private Vector2 player1startPos;
    private Vector2 player2startPos;
    private Vector2 puckStartPos;

    [Header("Game Logic & Scoring")]
    public int score1 = 0;
    public int score2 = 0;
    public int howManyGoals;
    [SerializeField] private TimerScr timer;
    [SerializeField] private EndScreen endScreen;

    [Header("Audio")]
    public AudioSource audioSourceSfx;
    public AudioSource audioSourceBgMusic;
    public AudioClip puckSound;
    public AudioClip StartGameSound;
    [SerializeField] private AudioClip[] gameMusics;

    void Awake()
    {
        Instance = this;
        player1startPos = player1.transform.position;
        player2startPos = player2.transform.position;
        puckStartPos = puck.transform.position;
        player1.GetComponentInChildren<Light2D>().intensity = 0;
        player2.GetComponentInChildren<Light2D>().intensity = 0;
    }

    void Start()
    {        
        timer.TimerStart();
        audioSourceSfx.PlayOneShot(StartGameSound);
        bool isMusic = PlayerPrefs.GetInt("BgMusicInGame", 1) != 0;
        if(isMusic)
        {
            int rand = UnityEngine.Random.Range(0, gameMusics.Length);
            audioSourceBgMusic.clip = gameMusics[rand];
            audioSourceBgMusic.loop = true;
            audioSourceBgMusic.time = 0;
            audioSourceBgMusic.Play();
        }
        howManyGoals = 4;
        puck.GetComponent<TrailRenderer>().enabled = PlayerPrefs.GetInt("PuckTrail", 1) != 0;
    }

    [Server] 
    public void ServerProcessGoal(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("GoalTrigger1"))
        {
            score1++;
        }
        else if(collision.gameObject.CompareTag("GoalTrigger2"))
        {
            score2++;
        }

        if(score1 >= howManyGoals)
        {
            RpcWinLose(1);
        } else if(score2 >= howManyGoals)
        {
            RpcWinLose(2);
        } else
        {
            RpcOnGoalScored(score1, score2);
            ServerResetPosition();
        }
    }

    [ClientRpc]
    private void RpcOnGoalScored(int newScore1, int newScore2)
    {
        Debug.Log($"Сервер сообщил: Забит гол! Текущий счет: {newScore1} {newScore2}");
        scoreText1.text = newScore1.ToString(); 
        scoreText2.text = newScore2.ToString(); 
        timer.Goal();
    }

    [ClientRpc]
    private void RpcWinLose(int playerIndex)
    {
        if(playerIndex == 1)
        {
            if(isClientOnly)
            {
                Win();
            } else
            {
               Lose();
            }
        } else if(playerIndex == 2)
        {
            if(isClientOnly)
            {
                Lose();
            } else
            {
                Win();
            }
        }
    }

    public void RegisterPlayer(GameObject player, string name)
    {
        if(name == "Player1")
        {
            player1 = player;
        }
        else if(name == "Player2")
        {
            player2 = player;
        }
    }

    public void OnPuckCollisionEnter2D(Collision2D collision) 
    {
        if(!(collision.gameObject.name.Equals("Player1") || collision.gameObject.name.Equals("Player2")))
        {
            audioSourceSfx.PlayOneShot(puckSound);
        }
    }

    private void ServerResetPosition()
    {
        if(puck != null)
        {
            TeleportAndReset(puck.GetComponent<Rigidbody2D>(), puckStartPos);
            TeleportAndReset(player1.GetComponent<Rigidbody2D>(), player1startPos);
            TeleportAndReset(player2.GetComponent<Rigidbody2D>(), player2startPos);
        }
    }

    private void TeleportAndReset(Rigidbody2D rb, Vector2 targetPos)
    {
        if(rb == null) return;

        if(rb.TryGetComponent<PlayersControllerNetwork>(out var controller))
        {
            if(rb.TryGetComponent<NetworkTransformBase>(out var netTransform))
            {
                netTransform.enabled = false;
                netTransform.Reset();
                
                controller.ResetTargetPosition(targetPos);
                
                netTransform.enabled = true;
            }
            else
            {
                controller.ResetTargetPosition(targetPos); 
            }
        }
        else 
        {
            if(rb.TryGetComponent<NetworkTransformBase>(out var netTransform))
            {
                netTransform.enabled = false;
                netTransform.Reset();
            }

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = targetPos;
            rb.transform.position = targetPos;

            if(netTransform != null)
            {
                netTransform.enabled = true;
            }
        }
    }

    public void RestartGame()
    {
        score1 = 0;
        score2 = 0;
        scoreText1.text = "0";
        scoreText2.text = "0";
        PlayerPrefs.SetInt("HowMoneyAdds", 0);
        PlayerPrefs.SetInt("HowXpAdds", 0);
        PlayerPrefs.Save();
        ServerResetPosition();
        audioSourceSfx.PlayOneShot(StartGameSound);
        timer.TimerStart();
    }

    public void Win()
    {
        goalTextCanvas.SetActive(true);
        var rect = endSreenPanel.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
        endSreenPanel.SetActive(true);
        rect.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.3f).SetEase(Ease.OutBack);
        winOrLoseText.text = "Победа!";
    }
    public void Lose()
    {
        goalTextCanvas.SetActive(true);
        var rect = endSreenPanel.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
        endSreenPanel.SetActive(true);
        rect.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.3f).SetEase(Ease.OutBack);
        winOrLoseText.text = "Поражение!";
    }
    public void LoadMainMenu()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
}

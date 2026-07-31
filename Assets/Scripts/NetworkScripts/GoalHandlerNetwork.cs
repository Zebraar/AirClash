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
    public TextMeshProUGUI scoreText1;
    public TextMeshProUGUI scoreText2;
    [SerializeField] private TextMeshProUGUI winOrLoseText;
    [SerializeField] private TextMeshProUGUI rematchButtonText;
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button rematchButton;
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
    private int score1 = 0;
    private int score2 = 0;
    public int howManyGoals;
    [SerializeField] private TimerScr timer;

    [Header("Audio")]
    public AudioSource audioSourceSfx;
    public AudioSource audioSourceBgMusic;
    public AudioClip puckSound;
    public AudioClip StartGameSound;
    [SerializeField] private AudioClip[] gameMusics;

    [SyncVar(hook = nameof(OnPlayer1RematchChanged))]
    private bool player1Ready = false;

    [SyncVar(hook = nameof(OnPlayer2RematchChanged))]
    private bool player2Ready = false;

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
        PlayerPrefs.SetInt("IsHostDisconnect", 1);
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

    public void OnRematchButtonClicked()
    {
        int playerNumber = isServer ? 1 : 2;
        CmdRequestRematch(playerNumber);
    }

    [Command(requiresAuthority = false)]
    private void CmdRequestRematch(int playerNumber)
    {
        if(playerNumber == 1) player1Ready = true;
        if(playerNumber == 2) player2Ready = true;

        if(player1Ready && player2Ready)
        {
            player1Ready = false;
            player2Ready = false;
            
            score1 = 0;
            score2 = 0;
            
            ServerResetPosition(); 

            RpcRestartGame();
        }
    }

    private void OnPlayer1RematchChanged(bool oldVal, bool newVal)
    {
        UpdateRematchUI();
    }

    private void OnPlayer2RematchChanged(bool oldVal, bool newVal)
    {
        UpdateRematchUI();
    }

    private void UpdateRematchUI()
    {
        bool iAmServer = isServer;
        
        if(iAmServer)
        {
            if(player1Ready && !player2Ready)
            {
                rematchButtonText.text = "Ожидание соперника...";
                mainMenuBtn.interactable = false;
                rematchButton.interactable = false;
            } 
            else if(!player1Ready && player2Ready) rematchButtonText.text = "Соперник хочет реванш!";
            else rematchButtonText.text = "Реванш";
        }
        else
        {
            if(player2Ready && !player1Ready)
            {
                rematchButtonText.text = "Ожидание соперника...";
                mainMenuBtn.interactable = false;
                rematchButton.interactable = false;
            }
            else if(!player2Ready && player1Ready) rematchButtonText.text = "Соперник хочет реванш!";
            else rematchButtonText.text = "Реванш";
        }
    }

    [ClientRpc]
    private void RpcRestartGame()
    {
        scoreText1.text = "0";
        scoreText2.text = "0";
        
        if(audioSourceSfx && StartGameSound)
            audioSourceSfx.PlayOneShot(StartGameSound);
            
        timer.TimerStart();
        
        rematchButtonText.text = "Реванш"; 
        mainMenuBtn.interactable = true;
        rematchButton.interactable = true;
        endSreenPanel.SetActive(false);
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
        PlayerPrefs.SetInt("IsHostDisconnect", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
}

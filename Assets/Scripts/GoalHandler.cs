using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoalHandler : MonoBehaviour
{
    public Text scoreText1;
    public Text scoreText2;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    private Vector2 player1startPos;
    private Vector2 player2startPos;
    private Vector2 puckStartPos;
    public GameObject puck;
    public int score1 = 0;
    public int score2 = 0;
    [SerializeField] private TimerScr timer;
    public AudioSource audioSource;
    public AudioClip puckSound;
    public AudioClip StartGameSound;
    [SerializeField] private BotsAI botsAI;
    private int howManyGoals;
    [SerializeField] private Text goalText;
    [SerializeField] private GameObject goalTextCanvas;
    public GameObject particlePrefab;
    private bool isParticlesOn;
    public MoneyHandler moneyHandler;
    [SerializeField] private int howMoneyAdd;
    [SerializeField] private int howMoneyRemove;
    private Color wallParticleColor;

    void Awake()
    {
        player1startPos = player1.transform.position;
        player2startPos = player2.transform.position;
        puckStartPos = puck.transform.position;
        var ps = particlePrefab.GetComponent<ParticleSystem>();
        var psMain = ps.main;
        if(PlayerPrefs.GetInt("Particle") == 0) isParticlesOn = false;
        else isParticlesOn = true;
    }

    void Start()
    {
        timer.TimerStart();
        audioSource.PlayOneShot(StartGameSound);
        howManyGoals = PlayerPrefs.GetInt("Goals");
        howMoneyAdd = PlayerPrefs.GetInt("HowMoneyAdd") * Mathf.Max(1, howManyGoals / 2);
        howMoneyRemove = PlayerPrefs.GetInt("HowMoneyRemove");  
        if (!ColorUtility.TryParseHtmlString("#30C7FE", out wallParticleColor))
        {
            wallParticleColor = Color.white; // Цвет по умолчанию, если Hex неверный
        } 
    }

    public void OnGoalTrigger(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("GoalTrigger1"))
        {
            score1++;
            scoreText1.text = score1.ToString(); 
            if(score1 >= howManyGoals)
            {
                if(SceneManager.GetActiveScene().name == "BotsGame")
                {
                    moneyHandler.RemoveMoney(howMoneyRemove);
                    if(moneyHandler.GetMoney() <= 0) moneyHandler.RemoveMoney(moneyHandler.GetMoney());
                    PlayerPrefs.SetInt("Money", moneyHandler.GetMoney());
                    PlayerPrefs.Save();
                }
                goalTextCanvas.SetActive(true);
                goalText.text = "Игрок 2 выиграл!";
                Invoke("RestartGame", 3f);
            } else
            {
                if(SceneManager.GetActiveScene().name == "BotsGame")
                {
                    botsAI.EasyMode();
                    ResetPosition();
                    timer.Goal();
                } else
                {
                    ResetPosition();
                    timer.Goal();
                }
            }
        }
        else if (collision.gameObject.CompareTag("GoalTrigger2"))
        {
            score2++;
            scoreText2.text = score2.ToString(); 
            if(score2 >= howManyGoals)
            {
                if(SceneManager.GetActiveScene().name == "BotsGame")
                {
                    moneyHandler.AddMoney(howMoneyAdd);
                    PlayerPrefs.SetInt("Money", moneyHandler.GetMoney());
                    PlayerPrefs.Save();
                }
                goalTextCanvas.SetActive(true);
                goalText.text = "Игрок 1 выиграл!";    
                Invoke("RestartGame", 3f);
            } else
            {
                if(SceneManager.GetActiveScene().name == "BotsGame")
                {
                    botsAI.Fury();
                    ResetPosition();
                    timer.Goal();
                } else
                {
                    ResetPosition();
                    timer.Goal();
                }   
            }
        }
    }

    public void OnPuckCollisionEnter2D(Collision2D collision) 
    {
        if(isParticlesOn == true && collision.gameObject.CompareTag("Wall"))
        {
            var ps = particlePrefab.GetComponent<ParticleSystem>();
            var psMain = ps.main;
            ContactPoint2D contact = collision.contacts[0];
            Vector3 spawnPos = contact.point;
            spawnPos.z = -1f;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            psMain.startColor = wallParticleColor;     
            var newParticles = Instantiate(particlePrefab, contact.point, rotation);
            newParticles.GetComponent<ParticleSystem>().Play();
        }
        audioSource.PlayOneShot(puckSound);
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
        audioSource.PlayOneShot(StartGameSound);
        timer.TimerStart();
    }
}

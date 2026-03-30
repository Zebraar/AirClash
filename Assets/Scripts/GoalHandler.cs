using JetBrains.Annotations;
using Mono.Cecil.Cil;
using UnityEditor;
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
    private Rigidbody2D puckRb;
    public float maxSpeed = 20f;
    [SerializeField] private BotsAI botsAI;
    private int howManyGoals;
    [SerializeField] private Text goalText;
    [SerializeField] private GameObject goalTextCanvas;
    public GameObject particlePrefab;

    void Awake()
    {
        player1startPos = player1.transform.position;
        player2startPos = player2.transform.position;
        puckStartPos = puck.transform.position;
        puckRb = puck.GetComponent<Rigidbody2D>();
        var ps = particlePrefab.GetComponent<ParticleSystem>();
        var psMain = ps.main;
    }

    void Start()
    {
        timer.TimerStart();
        audioSource.PlayOneShot(StartGameSound);
        howManyGoals = PlayerPrefs.GetInt("Goals");
    }

    void FixedUpdate()
    {
        if (puckRb.linearVelocity.magnitude > maxSpeed)
        {
            puckRb.linearVelocity = Vector3.ClampMagnitude(puckRb.linearVelocity, maxSpeed);
        }
        if(puckRb.linearVelocityY < 0.1f && puckRb.linearVelocityY > -0.1f && !timer.TimerOn && puckRb.linearVelocityX != 0)
        {
            puckRb.linearVelocity = new Vector2(puckRb.linearVelocity.x, 0.1f * Mathf.Sign(puckRb.linearVelocity.y));
        }
        if(puckRb.linearVelocityX < 0.1f && puckRb.linearVelocityX > -0.1f && !timer.TimerOn && puckRb.linearVelocityY != 0)
        {
            puckRb.linearVelocity = new Vector2(puckRb.linearVelocity.y, 0.1f * Mathf.Sign(puckRb.linearVelocity.x));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("GoalTrigger1"))
        {
            score1++;
            scoreText1.text = score1.ToString(); 
            if(score2 >= howManyGoals)
            {
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

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        var ps = particlePrefab.GetComponent<ParticleSystem>();
        var psMain = ps.main;
        // Получаем точку контакта для точности
        ContactPoint2D contact = collision.contacts[0];
        Vector3 spawnPos = contact.point;
        spawnPos.z = -1f;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        psMain.startColor = collision.gameObject.GetComponent<SpriteRenderer>().color;
           
        // Создаем частицы в месте удара
        var newParticles = Instantiate(particlePrefab, contact.point, rotation);
        newParticles.GetComponent<ParticleSystem>().Play();

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

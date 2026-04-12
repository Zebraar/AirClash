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
    [SerializeField] private int howMoneyAddAsLose;
    [SerializeField] private AchievementsHandler achievementsHandler;
    private Color wallParticleColor;
    private string lastCollision;

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
        howMoneyAdd = PlayerPrefs.GetInt("HowMoneyAdd") * Mathf.Max(1, Convert.ToInt32(howManyGoals / 1.5f));
        howMoneyAddAsLose = PlayerPrefs.GetInt("HowMoneyAddAsLose");  
        if (!ColorUtility.TryParseHtmlString("#30C7FE", out wallParticleColor))
        {
            wallParticleColor = Color.white;
        } 
    }

    public void OnGoalTrigger(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GoalTrigger1"))
        {
            if(lastCollision == "Player1") achievementsHandler.UpdateProgress("own_goal", 1);
            score1++;
            scoreText1.text = score1.ToString(); 
            if(score1 >= howManyGoals)
            {
                if(SceneManager.GetActiveScene().name == "BotsGame") Lose();
                else Win();
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
                if(score2 == 10) achievementsHandler.UpdateProgress("ten", 10);
                Win();
            } else
            {
                if(SceneManager.GetActiveScene().name == "BotsGame")
                {
                    UpdateAchievements();
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
        lastCollision = collision.gameObject.name;
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
        PlayerPrefs.SetInt("HowMoneyAdds", 0);
        PlayerPrefs.Save();
        ResetPosition();
        audioSource.PlayOneShot(StartGameSound);
        timer.TimerStart();
    }

    public void Win()
    {
        if(SceneManager.GetActiveScene().name == "BotsGame")
        {
            UpdateAchievements();
            switch(PlayerPrefs.GetFloat("Difficulty"))
            {
                case 3.1415926535f:
                    achievementsHandler.UpdateProgress("light_warm-up", 1);
                    break;
                case 7.5f:
                    achievementsHandler.UpdateProgress("warm-up", 1);
                    break;
                case 13.5f:
                    achievementsHandler.UpdateProgress("training", 1);
                    break;
                case 25f:
                    achievementsHandler.UpdateProgress("fight", 1);
                    break;
                case 50f:
                    achievementsHandler.UpdateProgress("competitions", 1);
                    break;
            }  
            PlayerPrefs.SetInt("Money", moneyHandler.GetMoney());
            PlayerPrefs.SetInt("HowMoneyAdds", PlayerPrefs.GetInt("HowMoneyAdds") + howMoneyAdd);
            PlayerPrefs.SetInt("isAfterGame", 1);
            PlayerPrefs.Save();
        }
        goalTextCanvas.SetActive(true);
        if(score1 >= howManyGoals) goalText.text = "Игрок 1 выиграл!";  
        else if(score2 >= howManyGoals) goalText.text = "Игрок 2 выиграл!";  
        Invoke("LoadMainMenu", 4f);
    }
    public void Lose()
    {
        if(SceneManager.GetActiveScene().name == "BotsGame")
        {
            if(PlayerPrefs.GetFloat("Difficulty") == 7.5f) achievementsHandler.UpdateProgress("seriously", 1);
            PlayerPrefs.SetInt("Money", moneyHandler.GetMoney());
            PlayerPrefs.SetInt("HowMoneyAdds", howMoneyAddAsLose);
            PlayerPrefs.SetInt("isAfterGame", 1);
            PlayerPrefs.Save();
        }
        goalTextCanvas.SetActive(true);
        if(score1 >= howManyGoals) goalText.text = "Игрок 1 выиграл!";  
        else if(score2 >= howManyGoals) goalText.text = "Игрок 2 выиграл!";  
        Invoke("LoadMainMenu", 4f);
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void UpdateAchievements()
    {
        achievementsHandler.UpdateProgress("a_start_has_been_made", 1);
        achievementsHandler.UpdateProgress("begginer", 1);
        achievementsHandler.UpdateProgress("amateur", 1);
        achievementsHandler.UpdateProgress("professional", 1);
        achievementsHandler.UpdateProgress("master", 1);
        achievementsHandler.UpdateProgress("world_champion", 1);
        achievementsHandler.UpdateProgress("best_in_the_galaxy", 1);
        achievementsHandler.UpdateProgress("best_in_the_universe", 1);
    }
}

using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoalHandler : MonoBehaviour
{
    [Header("UI Elements")]
    public Text scoreText1;
    public Text scoreText2;
    [SerializeField] private Text goalText;
    [SerializeField] private GameObject goalTextCanvas;
    [SerializeField] private GameObject endSreenPanel;

    [Header("Players & Puck")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    public GameObject puck;
    [SerializeField] private BotsAI botsAI;

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
    private string lastCollision;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip puckSound;
    public AudioClip StartGameSound;
    [SerializeField] private AudioClip[] gameMusics;

    [Header("Effects")]
    public GameObject particlePrefab;
    private bool isParticlesOn;
    private Color wallParticleColor;

    [Header("Economy & Achievements")]
    public MoneyHandler moneyHandler;
    public int howMoneyAdd;
    private int howMoneyAddAsLose;
    [SerializeField] private AchievementsHandler achievementsHandler;

    [Header("Xp Logic")]
    [SerializeField] private XpHandler xpHandler;
    private int howManyXpAddAsWin;
    private int howManyXpAddForGoal;
    private int howManyXpAddAsLose;
    private int totalXpEarned;

    [Header("Quests")]
    [SerializeField] private DailyQuestHandler dailyQuestHandler;
    [SerializeField] private QuestsHandler questsHandler;


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
        totalXpEarned = 0;
        xpHandler.ResetOldXp();   
        timer.TimerStart();
        audioSource.PlayOneShot(StartGameSound);
        bool isMusic = PlayerPrefs.GetInt("BgMusicInGame", 1) != 0;
        if(isMusic)
        {
            int rand = UnityEngine.Random.Range(0, gameMusics.Length);
            audioSource.clip = gameMusics[rand];
            audioSource.loop = true;
            audioSource.time = 0;
            audioSource.Play();
        }
        howManyGoals = PlayerPrefs.GetInt("Goals");
        howMoneyAdd = Convert.ToInt32(PlayerPrefs.GetFloat("Difficulty") / 3 * Mathf.Max(1, howManyGoals));
        howManyXpAddForGoal = Convert.ToInt32(PlayerPrefs.GetFloat("Difficulty") / 2);
        howManyXpAddAsWin = howManyXpAddForGoal * Mathf.Max(1, Convert.ToInt32(howManyGoals / 1.5f));
        howManyXpAddAsLose = 1;
        howMoneyAddAsLose = 1;  
        endSreenPanel.SetActive(false);
        if(!ColorUtility.TryParseHtmlString("#30C7FE", out wallParticleColor))
        {
            wallParticleColor = Color.white;
        } 
        puck.GetComponent<TrailRenderer>().enabled = PlayerPrefs.GetInt("PuckTrail", 1) != 0;
    }

    public void OnGoalTrigger(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GoalTrigger1"))
        {
            if(lastCollision == "Player1" && SceneManager.GetActiveScene().name.Equals("BotsGame")) achievementsHandler.UpdateProgress("own_goal", 1);
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
                        botsAI.UpdateBotSpeed(score1, score2);
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
                    if(score2 >= 10) achievementsHandler.UpdateProgress("ten", 10);
                    UpdateGoalQuests();
                }
                Win();
            } else
            {
                if(SceneManager.GetActiveScene().name == "BotsGame")
                {
                    if(score2 >= 10) achievementsHandler.UpdateProgress("ten", 10);
                    totalXpEarned += howManyXpAddForGoal;
                    UpdateGoalQuests();
                    UpdateAchievements();
                    botsAI.UpdateBotSpeed(score1, score2);
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
        if(!(collision.gameObject.name.Equals("Player1") || collision.gameObject.name.Equals("Player2")))
        {
            audioSource.PlayOneShot(puckSound);
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
        PlayerPrefs.SetInt("HowMoneyAdds", 0);
        PlayerPrefs.SetInt("HowXpAdds", 0);
        PlayerPrefs.Save();
        ResetPosition();
        audioSource.PlayOneShot(StartGameSound);
        timer.TimerStart();
    }

    public void Win()
    {
        if(SceneManager.GetActiveScene().name == "BotsGame")
        {
            int xpBefore = xpHandler.GetXP();   
            int actuallyEarned = howManyXpAddAsWin + PlayerPrefs.GetInt("HowXpAdds");
            UpdateXpQuests(actuallyEarned);
            xpHandler.AddXp(howManyXpAddAsWin + PlayerPrefs.GetInt("HowXpAdds"));
            UpdateAchievements();
            UpdateWinQuests();
            endScreen.StartEndScreen(actuallyEarned, xpBefore); 
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
                    dailyQuestHandler.UpdateQuestProgress("win_normal_bot", 1);
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
        } else
        {
            int xpBefore = xpHandler.GetXP();
            endScreen.StartEndScreen(0, xpBefore);
        }
        goalTextCanvas.SetActive(true);
        var rect = endSreenPanel.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
        endSreenPanel.SetActive(true);
        rect.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.3f).SetEase(Ease.OutBack);
    }
    public void Lose()
    {
        if(SceneManager.GetActiveScene().name == "BotsGame")
        {
            int xpBefore = xpHandler.GetXP();
            xpHandler.AddXp(howManyXpAddAsLose + PlayerPrefs.GetInt("HowXpAdds"));
            int xpAfter = xpHandler.GetXP();
            
            int actuallyEarned = xpAfter - xpBefore;
            UpdateXpQuests(actuallyEarned);
            endScreen.StartEndScreen(actuallyEarned, xpBefore); 
            if(PlayerPrefs.GetFloat("Difficulty") == 7.5f) achievementsHandler.UpdateProgress("seriously", 1);
            PlayerPrefs.SetInt("Money", moneyHandler.GetMoney());
            PlayerPrefs.SetInt("HowMoneyAdds", PlayerPrefs.GetInt("HowMoneyAdds") + howMoneyAddAsLose);
            PlayerPrefs.SetInt("isAfterGame", 1);
            PlayerPrefs.Save();
        }
        goalTextCanvas.SetActive(true);
        var rect = endSreenPanel.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
        endSreenPanel.SetActive(true);
        rect.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.3f).SetEase(Ease.OutBack);
    }
    public void LoadMainMenu()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
    public void UpdateAchievements()
    {
        PlayerPrefs.SetInt("TotalGoals", PlayerPrefs.GetInt("TotalGoals", 0) + 1);
        PlayerPrefs.Save();
        achievementsHandler.UpdateProgress("a_start_has_been_made", 1);
        achievementsHandler.UpdateProgress("begginer", 1);
        achievementsHandler.UpdateProgress("amateur", 1);
        achievementsHandler.UpdateProgress("professional", 1);
        achievementsHandler.UpdateProgress("master", 1);
        achievementsHandler.UpdateProgress("world_champion", 1);
        achievementsHandler.UpdateProgress("best_in_the_galaxy", 1);
        achievementsHandler.UpdateProgress("best_in_the_universe", 1);
    }
    private void UpdateWinQuests()
    {
        dailyQuestHandler.UpdateQuestProgress("win_1_matches", 1);
        dailyQuestHandler.UpdateQuestProgress("win_3_matches", 1);
        dailyQuestHandler.UpdateQuestProgress("win_5_matches", 1);
        dailyQuestHandler.UpdateQuestProgress("win_7_matches", 1);
        dailyQuestHandler.UpdateQuestProgress("win_10_matches", 1);
    }
    private void UpdateGoalQuests()
    {
        questsHandler.UpdateQuestProgress("goal10", 1);
        questsHandler.UpdateQuestProgress("goal50", 1);
        questsHandler.UpdateQuestProgress("goal100", 1);
        questsHandler.UpdateQuestProgress("goal200", 1);
        questsHandler.UpdateQuestProgress("goal300", 1);
        questsHandler.UpdateQuestProgress("goal500", 1);
        dailyQuestHandler.UpdateQuestProgress("goal20", 1);
    }
    private void UpdateXpQuests(int amount)
    {
        questsHandler.UpdateQuestProgress("xp100", amount);
        questsHandler.UpdateQuestProgress("xp200", amount);
        questsHandler.UpdateQuestProgress("xp400", amount);
        questsHandler.UpdateQuestProgress("xp500", amount);
        questsHandler.UpdateQuestProgress("xp700", amount);
        questsHandler.UpdateQuestProgress("xp1000", amount);
        dailyQuestHandler.UpdateQuestProgress("xp50", amount);
    }
}

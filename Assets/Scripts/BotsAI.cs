using UnityEngine;

public class BotsAI : MonoBehaviour
{

    public GameObject bot;
    public Transform puck;
    public float moveSpeed;
    public float minX, maxX, minY, maxY;
    private Rigidbody2D botRb;
    [SerializeField] private TimerScr timer;
    [SerializeField] private Vector2 botStartPos;
    [SerializeField] private GoalHandler goalHandler;
    private int score1, score2 = 0;
    private float baseSpeed;
    [SerializeField] private Vector3 PuckKoof;

    void Start()
    {
        botRb = bot.GetComponent<Rigidbody2D>();
        botRb.linearVelocity = Vector2.zero;
        moveSpeed = PlayerPrefs.GetFloat("Difficulty");
        baseSpeed = moveSpeed;
    }

    void FixedUpdate()
    {
        if(timer.TimerOn) return;

        Vector2 targetDestination;
        float currentSpeed = moveSpeed;

        if(puck.position.x > 0 && puck.position.x < 3)
        {
            Vector2 puckPos = puck.position;
            puckPos.x *= -1.0f;
            puckPos.x -= PlayerPrefs.GetFloat("BotOffsetX");
            if(UnityEngine.Random.Range(0, 1) == 0) puckPos.y += PlayerPrefs.GetInt("BotOffsetY");
            else puckPos.y -= PlayerPrefs.GetInt("BotOffsetY");
            targetDestination = puckPos;          
            currentSpeed = moveSpeed / 2;
        } else if(puck.position.x > 3)
        {
            targetDestination = botStartPos;
            currentSpeed = moveSpeed / 3;
        } else if(puck.position.x < 0 && (puck.position.y > 4 || puck.position.y < -4))
        {
            targetDestination = botStartPos;
            currentSpeed = moveSpeed / 4;
        } else if(puck.position.x < -6 && (puck.position.y > 3.5f || puck.position.y < -3.5f))
        {
            targetDestination = botStartPos;
            currentSpeed = moveSpeed / 4;
        } else if(puck.position.x < -6)
        {
            targetDestination = puck.position;
            currentSpeed = moveSpeed * 1.5f;
        } else if(puck.position.x < -4)
        {
            if(puck.position.y > 0)
            {
                PuckKoof.y += 0.5f;
                targetDestination = puck.position - PuckKoof;
                currentSpeed = moveSpeed;
                PuckKoof.y -= 0.5f;
            } else if(puck.position.y < 0)
            {
                PuckKoof.y -= 0.5f;
                targetDestination = puck.position - PuckKoof;
                currentSpeed = moveSpeed;
                PuckKoof.y += 0.5f;
            } else
            {
                targetDestination = puck.position;
                currentSpeed = moveSpeed;
            }
        } else
        {
            if(puck.position.y > 0)
            {
                PuckKoof.y += 0.5f;
                targetDestination = puck.position - PuckKoof;
                currentSpeed = moveSpeed;
                PuckKoof.y -= 0.5f;
            } else if(puck.position.y < 0)
            {
                PuckKoof.y -= 0.5f;
                targetDestination = puck.position - PuckKoof;
                currentSpeed = moveSpeed;
                PuckKoof.y += 0.5f;
            } else
            {
                targetDestination = puck.position;
                currentSpeed = moveSpeed;
            }
            
        }
        Vector2 newPos = Vector2.MoveTowards(botRb.position, targetDestination, currentSpeed * Time.fixedDeltaTime);
    
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        botRb.MovePosition(newPos);
        
    }

    public void Fury()
    {
        score1 = goalHandler.score1;
        score2 = goalHandler.score2;

        if(score2 - score1 >= 3 && score2 - score1 < 5)
        {
            moveSpeed = baseSpeed * 1.5f;
        } else if(score2 - score1 >= 5 && score2 - score1 < 7)
        {
            moveSpeed = baseSpeed * 2f;
        } else if(score2 - score1 >= 7)
        {
            moveSpeed = baseSpeed * 3f;
        }
    }
    public void EasyMode()
    {
        score1 = goalHandler.score1;
        score2 = goalHandler.score2;

        if(score1 - score2 >= 3 && score1 - score2 < 5)
        {
            moveSpeed = baseSpeed / 1.5f;
        } else if(score1 - score2 >= 5 && score1 - score2 < 7)
        {
            moveSpeed = baseSpeed / 2f;
        } else if(score1 - score2 >= 7)
        {
            moveSpeed = baseSpeed / 3f;
        }
    }

}

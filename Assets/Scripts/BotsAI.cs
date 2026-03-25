using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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

    void Start()
    {
        botRb = bot.GetComponent<Rigidbody2D>();
        botRb.linearVelocity = Vector2.zero;
        moveSpeed = PlayerPrefs.GetInt("Difficulty");
    }

    void FixedUpdate()
    {
        if(timer.TimerOn) return;

        Vector2 targetDestination;
        float currentSpeed = moveSpeed;

        if(puck.position.x > 0)
        {
            targetDestination = new Vector2(puck.position.x * -1, puck.position.y);
            currentSpeed = moveSpeed / 2;
        } else if(puck.position.x < 0 && puck.position.y > 4 || puck.position.y < -4)
        {
            targetDestination = botStartPos;
            currentSpeed = moveSpeed / 4;
        } else
        {
            targetDestination = puck.position;
            currentSpeed = moveSpeed;
        }

        Vector2 newPos = Vector2.MoveTowards(botRb.position, targetDestination, currentSpeed * Time.fixedDeltaTime);
    
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        botRb.MovePosition(newPos);
        
    }

    public void Fury()
    {
        score1 = int.Parse(goalHandler.scoreText1.text);
        score2 = int.Parse(goalHandler.scoreText2.text);

        if(score1 - score2 >= 3)
        {
            moveSpeed *= 1.5f;
        } else if(score1 - score2 >= 5)
        {
            moveSpeed *= 2f;
        } else if(score1 - score2 >= 7)
        {
            moveSpeed *= 3f;
        }
    }
    public void EasyMode()
    {
        score1 = int.Parse(goalHandler.scoreText1.text);
        score2 = int.Parse(goalHandler.scoreText2.text);

        if(score2 - score1 >= 3)
        {
            moveSpeed /= 1.5f;
        } else if(score2 - score1 >= 5)
        {
            moveSpeed /= 2f;
        } else if(score2 - score1 >= 7)
        {
            moveSpeed /= 3f;
        }
    }

}

using System;
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

    void Start()
    {
        botRb = bot.GetComponent<Rigidbody2D>();
        botRb.linearVelocity = Vector2.zero;
    }

    void FixedUpdate()
    {
        if(timer.TimerOn) return;
        if(puck.position.x > 0)
        {
            Vector2 targetPos = Vector2.MoveTowards(botRb.position, puck.position, moveSpeed/2 * Time.fixedDeltaTime);
            targetPos.x = Mathf.Clamp(puck.position.x * -1, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
            botRb.MovePosition(targetPos);
        } else if(puck.position.x < 0 && puck.position.y > 4 || puck.position.y < -4)
        {
            Vector2 targetPos = Vector2.MoveTowards(botRb.position, botStartPos, moveSpeed/4 * Time.fixedDeltaTime);
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
            botRb.MovePosition(targetPos);
        } else
        {
            Vector2 targetPos = Vector2.MoveTowards(botRb.position, puck.position, moveSpeed * Time.fixedDeltaTime);
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
            botRb.MovePosition(targetPos);
        }
        
    }

}

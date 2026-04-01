using Unity.VisualScripting;
using UnityEngine;

public class PuckScr : MonoBehaviour
{
    [SerializeField] private GoalHandler goalHandler;
    private Rigidbody2D puckRb;
    public float maxSpeed = 20f;
    [SerializeField] private TimerScr timer;

    void Awake()
    {
        puckRb = GetComponent<Rigidbody2D>();
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
    void OnTriggerEnter2D(Collider2D collision)
    {
        goalHandler.OnGoalTrigger(collision);
    }
    private void OnCollisionEnter2D(Collision2D other) {
        goalHandler.OnPuckCollisionEnter2D(other);
    }
}

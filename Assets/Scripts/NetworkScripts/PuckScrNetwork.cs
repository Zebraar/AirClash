using Mirror;
using UnityEngine;

public class PuckScrNetwork : NetworkBehaviour
{
    private Rigidbody2D puckRb;
    public float maxSpeed = 20f;
    [SerializeField] private TimerScr timer;
    private NetworkTransformBase netTransform;
    private bool isPredicting = false;
    private float predictionTimer = 0f;

    private void Awake()
    {
        puckRb = GetComponent<Rigidbody2D>();
        netTransform = GetComponent<NetworkTransformBase>();
    }

    void FixedUpdate()
    {
        if(puckRb.linearVelocity.magnitude > maxSpeed)
        {
            puckRb.linearVelocity = Vector3.ClampMagnitude(puckRb.linearVelocity, maxSpeed);
        }

        bool isTimerActive = timer != null && timer.TimerOn;

        if(puckRb.linearVelocityY < 0.1f && puckRb.linearVelocityY > -0.1f && !isTimerActive && puckRb.linearVelocityX != 0)
        {
            puckRb.linearVelocity = new Vector2(puckRb.linearVelocity.x, 0.1f * Mathf.Sign(puckRb.linearVelocity.y == 0 ? 1 : puckRb.linearVelocity.y));
        }

        if(puckRb.linearVelocityX < 0.1f && puckRb.linearVelocityX > -0.1f && !isTimerActive && puckRb.linearVelocityY != 0)
        {
            puckRb.linearVelocity = new Vector2(0.1f * Mathf.Sign(puckRb.linearVelocity.x == 0 ? 1 : puckRb.linearVelocity.x), puckRb.linearVelocity.y);
        }
    }

    public void ApplyLocalHitPrediction(Vector2 hitDirection, float hitSpeed)
    {
        if(isPredicting) return;

        if(netTransform != null)
        {
            netTransform.enabled = false;
        }

        isPredicting = true;
        predictionTimer = 0.08f;

        puckRb.linearVelocity = hitDirection * hitSpeed;
    }

    private void Update()
    {
        if(!isPredicting) return;

        predictionTimer -= Time.deltaTime;
        if (predictionTimer <= 0f)
        {
            isPredicting = false;

            if(netTransform != null)
            {
                netTransform.enabled = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isServer) return; 
        GoalHandlerNetwork.Instance.ServerProcessGoal(collision); 
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        GoalHandlerNetwork.Instance.OnPuckCollisionEnter2D(other); 
    }
}
using System.Collections;
using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayersControllerNetwork : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Movement")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    [SerializeField] private TimerScr timer;

    [Header("Audio")]
    public AudioSource audioSource;
    [SerializeField] private AudioClip puckSound;

    [Header("Internal variables")]
    private Rigidbody2D rb;
    private Camera cam; 
    private Vector3 offset;
    [SyncVar]
    private Vector2 targetPos;
    private bool isDragging = false;
    private Color particleColor;
    private GameObject particles;

    [SyncVar(hook = nameof(OnPlayerIndexChanged))]
    private int netPlayerIndex = 0;
    [SyncVar]
    private bool isMovementBlocked = false;
    [SyncVar(hook = nameof(OnSkinChanged))]
    private string netSkinName = "";
    [SyncVar(hook = nameof(OnNickChanged))]
    private string netNickName = "";
    [SyncVar]
    private Vector2 netLinearVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        if(audioSource == null) audioSource = GameObject.Find("SoundManagerSfx").GetComponent<AudioSource>();
    }
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 60);

        targetPos = rb.position;

        if(timer == null) timer = FindAnyObjectByType<TimerScr>();
        float volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        AudioListener.volume = volume;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        if(NetworkServer.connections.Count <= 1)
        {
            netPlayerIndex = 1;
        }
        else
        {
            netPlayerIndex = 2;
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        string skinKey = PlayerPrefs.GetString("CurrentSkin", "DefSkin");
        CmdRequestSkin(skinKey);
        string nickKey = PlayerPrefs.GetString("Nick", "Ник");
        CmdRequestNick(nickKey);
    }

    private void OnPlayerIndexChanged(int oldIndex, int newIndex)
    {
        if(newIndex == 1)
        {
            gameObject.name = "Player1";
            minX = 0.5f;
            maxX = 6.65f;
            SetPlayerPosition(5.186f);
        }
        else if(newIndex == 2)
        {
            gameObject.name = "Player2";
            minX = -6.65f;
            maxX = -0.5f;
            SetPlayerPosition(-5.186f);
        }

        if(GoalHandlerNetwork.Instance != null)
        {
            GoalHandlerNetwork.Instance.RegisterPlayer(gameObject, gameObject.name);
        }

        if(!string.IsNullOrEmpty(netSkinName))
        {
            SkinData currentSkin = Resources.Load<SkinData>(netSkinName);
            ApplySkin(currentSkin);
        }
    }

    private void SetPlayerPosition(float xPos)
    {
        Vector3 newPos = new Vector3(xPos, transform.position.y, transform.position.z);
        transform.position = newPos;
        if(rb != null) 
        {
            rb.position = newPos;
            rb.linearVelocity = Vector2.zero;
        }
    }

    [Command]
    private void CmdRequestSkin(string skinName)
    {
        netSkinName = skinName;
    }

    private void OnSkinChanged(string oldSkin, string newSkin)
    {
        if(string.IsNullOrEmpty(newSkin)) return;

        if(netPlayerIndex == 0) return;
        
        SkinData currentSkin = Resources.Load<SkinData>(newSkin);
        ApplySkin(currentSkin);
    }

    [Command]
    private void CmdRequestNick(string nickName)
    {
        netNickName = nickName;
    }

    private void OnNickChanged(string oldSkin, string nickName)
    {
        if(string.IsNullOrEmpty(nickName)) return;

        if(netPlayerIndex == 0) return;
        if(netPlayerIndex == 1)
        {
            GameObject.Find("Player1NickTextTMP").GetComponent<TextMeshProUGUI>().text = nickName;
        } else if(netPlayerIndex == 2)
        {
            GameObject.Find("Player2NickTextTMP").GetComponent<TextMeshProUGUI>().text = nickName;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!isLocalPlayer) return; 

        Vector3 mousePos = cam.ScreenToWorldPoint(eventData.position);
        offset = (Vector2)transform.position - (Vector2)mousePos;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(timer != null && timer.TimerOn) return; 
        if(isMovementBlocked) return; 
        if(!isLocalPlayer || !isDragging) return;

        Vector3 mousePos = cam.ScreenToWorldPoint(eventData.position);
        Vector2 calculatedPos = new Vector2(mousePos.x + offset.x, mousePos.y + offset.y);

        calculatedPos.x = Mathf.Clamp(calculatedPos.x, minX, maxX);
        calculatedPos.y = Mathf.Clamp(calculatedPos.y, minY, maxY);

        targetPos = calculatedPos;

        CmdUpdatePosition(calculatedPos);
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            MoveRigidbodyPhysically(targetPos);
        }
        else if (isServer)
        {
            MoveRigidbodyPhysically(targetPos);
        }
        else
        {
            // Для прочих клиентов - интерполяция
            Vector2 nextPos = Vector2.Lerp(rb.position, targetPos, Time.fixedDeltaTime * 30f);
            MoveRigidbodyPhysically(nextPos);
        }
    }

    private void MoveRigidbodyPhysically(Vector2 target)
    {
        Vector2 desiredVelocity = (target - rb.position) / Time.fixedDeltaTime;

        rb.linearVelocity = Vector2.ClampMagnitude(desiredVelocity, 200f);
    }

    [Command]
    private void CmdUpdatePosition(Vector2 newPos)
    {
        if(isMovementBlocked) return; 
        targetPos = newPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!isLocalPlayer) return;
        isDragging = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isServer && collision.gameObject.name.Equals("Puck") && audioSource != null)
        {
            RpcPlayPuckSound();
        }
    }

    [ClientRpc]
    private void RpcPlayPuckSound()
    {
        if (audioSource != null && puckSound != null)
        {
            audioSource.PlayOneShot(puckSound);
        }
    }

    public void ApplySkin(SkinData skin)
    {
        if(skin == null || netPlayerIndex == 0) return;

        if(netPlayerIndex == 2) 
        {
            SkinData pl2Skin = Resources.Load<SkinData>(skin.name + "Pl2");
            if(pl2Skin != null) skin = pl2Skin;
        }
        GetComponent<SpriteRenderer>().sprite = skin.sprite;
        puckSound = skin.sound;
        if(skin.particles != null) 
        { 
            if(GetComponentInChildren<ParticleSystem>()) Destroy(GetComponentInChildren<ParticleSystem>().gameObject);

            GameObject newParticles = Instantiate(skin.particles, GetComponent<Transform>());
            newParticles.gameObject.SetActive(true);

            var ps = newParticles.GetComponent<ParticleSystem>();
            var psMain = ps.main;
            if(netPlayerIndex == 2 && ColorUtility.TryParseHtmlString("#ff6a6a", out particleColor))
            {
                psMain.startColor = particleColor;
            } else if(netPlayerIndex == 1 && ColorUtility.TryParseHtmlString("#9abaf5", out particleColor))
            {
                psMain.startColor = particleColor;
            }
            ps.Play(); 
        }
        if(skin.trail != null)
        {
            if(PlayerPrefs.GetInt("Trail", 1) == 1)
            {
                var trailPreset = skin.trail.GetComponent<TrailRenderer>();
                var newTrail = gameObject.GetComponent<TrailRenderer>();
                    
                if(newTrail != null && trailPreset != null)
                {
                    newTrail.sharedMaterial = trailPreset.sharedMaterial;
                    newTrail.time = trailPreset.time;
                    newTrail.startWidth = trailPreset.startWidth;
                    newTrail.endWidth = trailPreset.endWidth;
                    newTrail.numCornerVertices = trailPreset.numCornerVertices;
                    newTrail.numCapVertices = trailPreset.numCapVertices;
                    newTrail.alignment = trailPreset.alignment;
                    newTrail.textureMode = trailPreset.textureMode;
                    Gradient newGradient = new Gradient();
                    newGradient.SetKeys(trailPreset.colorGradient.colorKeys, trailPreset.colorGradient.alphaKeys);
                    newTrail.colorGradient = newGradient;
                    newTrail.enabled = true;
                }
            } 
            else if(gameObject.GetComponent<TrailRenderer>()) 
            {
                gameObject.GetComponent<TrailRenderer>().enabled = false;
            }
        } 
        else
        {
            if(gameObject.GetComponent<TrailRenderer>()) 
            {
                if(netPlayerIndex == 2)
                {
                    var trailRenderer = GetComponent<TrailRenderer>();
                    Color startColor;
                    Color endColor;
                    if(ColorUtility.TryParseHtmlString("#FE0000", out startColor))
                    {
                        trailRenderer.startColor = startColor;
                    }
                    if(ColorUtility.TryParseHtmlString("#DA0000", out endColor))
                    {
                        trailRenderer.endColor = endColor;
                    }
                } 
                GetComponent<TrailRenderer>().enabled = PlayerPrefs.GetInt("Trail", 1) == 1;
            }
        }
    }

    public void ResetTargetPosition(Vector2 newStartPos)
    {
        if(!isServer) return; 
        isMovementBlocked = true; 
        targetPos = newStartPos;
        if(rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = newStartPos;
            rb.transform.position = newStartPos;
        }
        
        if(netIdentity != null)
        {
            RpcForceTeleportClient(newStartPos);
        }

        if(gameObject.activeInHierarchy)
        {
            StartCoroutine(UnblockMovement());
        }
        else
        {
            isMovementBlocked = false; 
        }
    }

    [ClientRpc]
    private void RpcForceTeleportClient(Vector2 newStartPos)
    {
        if(isLocalPlayer)
        {
            isDragging = false; 
        }

        targetPos = newStartPos;

        if(rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = newStartPos;
            rb.transform.position = newStartPos;
        }

        if(TryGetComponent<NetworkTransformBase>(out var netTransform))
        {
            netTransform.Reset();
        }
        
    }

    [Server]
    IEnumerator UnblockMovement()
    {
        yield return new WaitForSecondsRealtime(1f);
        isMovementBlocked = false;
    }
}

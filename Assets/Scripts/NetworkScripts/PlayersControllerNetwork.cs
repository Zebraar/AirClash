using System.Collections;
using DG.Tweening;
using Mirror;
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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
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
        RpcApplySkinForAll(skinName);
    }

    [ClientRpc]
    private void RpcApplySkinForAll(string skinName)
    {
        SkinData currentSkin = Resources.Load<SkinData>(skinName);
        ApplySkin(currentSkin);
    }


    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 60);

        targetPos = rb.position;

        if(timer == null) timer = FindAnyObjectByType<TimerScr>();
        float volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        AudioListener.volume = volume;
        SkinData currentSkin = Resources.Load<SkinData>(PlayerPrefs.GetString("CurrentSkin", "DefSkin"));
        ApplySkin(currentSkin);
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

        CmdUpdatePosition(calculatedPos);
    }

    private void FixedUpdate()
    {
        if(isServer || isLocalPlayer || targetPos != Vector2.zero)
        {
            rb.MovePosition(targetPos);
        }
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

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.name.Equals("Puck") && audioSource != null)
        {
            audioSource.PlayOneShot(puckSound);
        }
    }

    public void ApplySkin(SkinData skin)
    {
        if(gameObject.name.Equals("Player1")) {
            skin = Resources.Load<SkinData>(PlayerPrefs.GetString("CurrentSkin"));
        } 
        else if(gameObject.name.Equals("Player2")) {
            skin = Resources.Load<SkinData>(PlayerPrefs.GetString("CurrentSkin") + "Pl2");
        }
        GetComponent<SpriteRenderer>().sprite = skin.sprite;
        puckSound = skin.sound;
        if(skin.particles != null) { 
            if(GetComponentInChildren<ParticleSystem>()) Destroy(GetComponentInChildren<ParticleSystem>().gameObject);
            particles = skin.particles;
            var ps = particles.GetComponent<ParticleSystem>();
            var psMain = ps.main;
            particles.gameObject.SetActive(true);
            if(skin.name == "GoldSkin")
            {
                psMain.startColor = Color.white;
            } else
            {
                if(gameObject.name.Equals("Player2") && ColorUtility.TryParseHtmlString("#ff6a6a", out particleColor))
                {
                    psMain.startColor = particleColor;
                } else if(gameObject.name.Equals("Player1") && ColorUtility.TryParseHtmlString("#9abaf5", out particleColor))
                {
                    psMain.startColor = particleColor;
                }
            }
            var newParticles = Instantiate(particles, GetComponent<Transform>());
            newParticles.gameObject.SetActive(true);
            newParticles.GetComponent<ParticleSystem>().Play(); 
        }
        if(skin.trail != null)
        {
            if(PlayerPrefs.GetInt("Trail", 1) == 1)
            {
                var trail = skin.trail.GetComponent<TrailRenderer>();
                var newTrail = gameObject.GetComponent<TrailRenderer>();
                newTrail.sharedMaterial = trail.sharedMaterial;
                newTrail.time = trail.time;
                newTrail.startWidth = trail.startWidth;
                newTrail.endWidth = trail.endWidth;
                newTrail.colorGradient = trail.colorGradient;
                newTrail.numCornerVertices = trail.numCornerVertices;
                newTrail.numCapVertices = trail.numCapVertices;
                newTrail.alignment = trail.alignment;
                newTrail.textureMode = trail.textureMode;
                gameObject.GetComponent<TrailRenderer>().enabled = true;
            } else gameObject.GetComponent<TrailRenderer>().enabled = false;
        } else
        {
            if(PlayerPrefs.GetInt("Trail", 1) == 1) GetComponent<TrailRenderer>().enabled = true;
            else GetComponent<TrailRenderer>().enabled = false;
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

    public override void OnStopServer()
    {
        PlayersControllerNetwork[] allPlayers = FindObjectsByType<PlayersControllerNetwork>();

        foreach(PlayersControllerNetwork player in allPlayers)
        {
            if(player != this)
            {
                player.TargetNotifyOpponentDisconnected(player.connectionToClient);
            }
        }

        base.OnStopServer();
    }

    [TargetRpc]
    public void TargetNotifyOpponentDisconnected(NetworkConnectionToClient target)
    {
        Debug.Log("Другой игрок отключился!");

        if(DisconnectUIManager.Instance != null)
        {
            DisconnectUIManager.Instance.ShowDisconnectUI();
        }
        
        StartCoroutine(LeaveToMainMenuRoutine(3f));
    }

    private IEnumerator LeaveToMainMenuRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if(NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}

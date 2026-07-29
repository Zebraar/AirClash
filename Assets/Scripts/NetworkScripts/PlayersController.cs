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
    private Vector2 targetPos;
    private bool isDragging = false;
    private Color particleColor;
    private GameObject particles;


    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 60);
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        targetPos = rb.position;
        float volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        AudioListener.volume = volume;
        if(PlayerPrefs.GetString("CurrentSkin") == "") PlayerPrefs.SetString("CurrentSkin", "DefSkin");
        SkinData currentSkin = Resources.Load<SkinData>(PlayerPrefs.GetString("CurrentSkin"));
        ApplySkin(currentSkin);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(eventData.position);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y);
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // if (!isDragging || timer.TimerOn) return;
        if(!isLocalPlayer) return;
        if (!isDragging) return;

        Vector3 mousePos = cam.ScreenToWorldPoint(eventData.position);
        targetPos = new Vector2(mousePos.x + offset.x, mousePos.y + offset.y);

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
    }

    private void FixedUpdate() {
        // if (isDragging && !timer.TimerOn)
        if(isDragging)
        {
            rb.MovePosition(targetPos);
        }
        // else if(timer.TimerOn)
        // {
        //     rb.linearVelocity = Vector2.zero;
        //     rb.angularVelocity = 0f;
        //     targetPos = rb.position;
        // }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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

}

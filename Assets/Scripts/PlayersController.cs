using UnityEngine;
using UnityEngine.EventSystems;

public class PlayersController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 offset;
    private Rigidbody2D rb;
    private Camera cam;

    public float minX, maxX, minY, maxY;
    [SerializeField] private TimerScr timer;    private Vector2 targetPos;
    private bool isDragging = false;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        if(PlayerPrefs.GetInt("FPS") < 15) Application.targetFrameRate = 60;
        else Application.targetFrameRate = PlayerPrefs.GetInt("FPS");
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        targetPos = rb.position;
        float volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        AudioListener.volume = volume;
        if(PlayerPrefs.GetString("CurrentSkin") == "") PlayerPrefs.SetString("CurrentSkin", "DefSkin");
        if(gameObject.name.Equals("Player1")) GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(PlayerPrefs.GetString("CurrentSkin"));
        else if(gameObject.name.Equals("Player2")) GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(PlayerPrefs.GetString("CurrentSkin") + "Pl2");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(eventData.position);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y);
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || timer.TimerOn) return;

        Vector3 mousePos = cam.ScreenToWorldPoint(eventData.position);
        targetPos = new Vector2(mousePos.x + offset.x, mousePos.y + offset.y);

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
    }

    private void FixedUpdate() {
        if (isDragging && !timer.TimerOn)
        {
            rb.MovePosition(targetPos);
        }
        else if (timer.TimerOn)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            targetPos = rb.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

}

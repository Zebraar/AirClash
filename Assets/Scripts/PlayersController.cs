using UnityEngine;
using UnityEngine.EventSystems;

public class PlayersController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 offset;
    private Rigidbody2D rb;
    private Camera cam;

    public float minX, maxX, minY, maxY;
    [SerializeField] private TimerScr timer;
    private Vector2 targetPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        targetPos = rb.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(eventData.position);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(eventData.position);
        targetPos = new Vector2(mousePos.x + offset.x, mousePos.y + offset.y);

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
    }

    private void FixedUpdate() {
        if (!timer.TimerOn) {
            rb.MovePosition(targetPos);
        } else {
            rb.linearVelocity = Vector2.zero;
            targetPos = rb.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Объект отпущен");
    }

}

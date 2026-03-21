using UnityEngine;
using UnityEngine.EventSystems;

public class PlayersController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 offset;
    private Rigidbody2D rb;

    void Start()
{
    rb = GetComponent<Rigidbody2D>();
}

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2 targetPos = new Vector2(mousePos.x + offset.x, mousePos.y + offset.y);

        rb.MovePosition(targetPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Объект отпущен");
    }

}

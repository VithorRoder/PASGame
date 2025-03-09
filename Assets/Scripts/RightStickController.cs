using UnityEngine;
using UnityEngine.EventSystems;

public class RightStickController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 stickDirection;
    private bool isTouching;

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouching = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchPosition = eventData.position;
        Vector2 stickCenter = (Vector2)transform.position;
        stickDirection = (touchPosition - stickCenter).normalized;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouching = false;
        stickDirection = Vector2.zero;
    }

    public bool IsTouching()
    {
        return isTouching;
    }
}
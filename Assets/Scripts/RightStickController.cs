using UnityEngine;
using UnityEngine.EventSystems;

public class RightStickController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 stickDirection;
    private bool isTouching;
    private Vector2 startTouchPosition;
    private bool isUsingMouse;

    private void Start()
    {
        stickDirection = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouching = true;
        startTouchPosition = eventData.position;
        stickDirection = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentTouchPosition = eventData.position;
        stickDirection = (currentTouchPosition - startTouchPosition).normalized;
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

    private void Update()
    {
        if (Application.isMobilePlatform) return;

        if (Input.GetMouseButtonDown(1))
        {
            isUsingMouse = true;
            isTouching = true;
            startTouchPosition = Input.mousePosition;
        }

        if (isUsingMouse && Input.GetMouseButton(1))
        {
            Vector2 currentMousePosition = Input.mousePosition;
            stickDirection = (currentMousePosition - startTouchPosition).normalized;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isUsingMouse = false;
            isTouching = false;
            stickDirection = Vector2.zero;
        }
    }
}
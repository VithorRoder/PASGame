using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float radius = 0.005f;
    private float angle = 0f;
    private Vector2 initialOffset;

    void Start()
    {
        if (_img != null)
        {
            initialOffset = _img.uvRect.position;
        }
    }

    void Update()
    {
        if (_img == null) return;

        angle += speed * Time.deltaTime;

        float offsetX = Mathf.Cos(angle) * radius;
        float offsetY = Mathf.Sin(angle) * radius;

        _img.uvRect = new Rect(initialOffset + new Vector2(offsetX, offsetY), _img.uvRect.size);
    }
}
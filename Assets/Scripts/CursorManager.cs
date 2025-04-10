using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;

    void Start()
    {
        if (cursorTexture != null)
        {
            Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }
    }
}
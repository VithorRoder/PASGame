using UnityEngine;

public class DisableCanvas : MonoBehaviour
{
    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            GameObject canvas = GameObject.FindWithTag("Canvas");

            bool isMobile = IsMobilePlatform();

            if (canvas != null)
            {
                if (isMobile)
                {
                    Debug.Log("WebGL Mobile detectado, não desativando Canvas.");
                }
                else
                {
                    Debug.Log("WebGL Desktop detectado, desativando Canvas...");
                    canvas.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("Nenhum Canvas encontrado com a tag 'Canvas'.");
            }
        }
    }

    bool IsMobilePlatform()
    {
        if (Application.isMobilePlatform)
        {
            return true;
        }

        if (Application.platform == RuntimePlatform.WebGLPlayer && Screen.width < 800)
        {
            return true;
        }

        return false;
    }
}
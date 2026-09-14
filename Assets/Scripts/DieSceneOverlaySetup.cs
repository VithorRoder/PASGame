using UnityEngine;
using UnityEngine.EventSystems;

// Attach to a root object in DieScene (e.g. the Canvas).
// Die() loads DieScene additively on top of GameScene so the dead player can
// keep watching the match. If GameScene already has an EventSystem, this
// scene's own EventSystem would duplicate it, so we remove the extra one.
public class DieSceneOverlaySetup : MonoBehaviour
{
    void Awake()
    {
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (eventSystems.Length > 1)
        {
            foreach (EventSystem es in eventSystems)
            {
                if (es.gameObject.scene == gameObject.scene)
                {
                    Destroy(es.gameObject);
                }
            }
        }
    }
}

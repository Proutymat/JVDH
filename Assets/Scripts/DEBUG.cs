using UnityEngine;
using UnityEngine.EventSystems;

public class DebugSelection : MonoBehaviour
{
    void Update()
    {
        if (EventSystem.current != null)
            Debug.Log("Selected : " + EventSystem.current.currentSelectedGameObject);
    }
}
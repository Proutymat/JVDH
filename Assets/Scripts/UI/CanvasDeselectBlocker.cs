using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasDeselectBlocker : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData e)
    {
        // Ne rien faire = empêche la déselection
    }
}
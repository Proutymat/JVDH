using UnityEngine;
using UnityEngine.EventSystems;

public class SliderEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        VideoManager.Instance.OnSliderPointerDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        VideoManager.Instance.OnSliderPointerUp();
    }
}

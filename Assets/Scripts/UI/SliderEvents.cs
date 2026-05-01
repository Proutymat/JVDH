using UnityEngine;
using UnityEngine.EventSystems;

public class SliderEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private VideoPlayerControls m_videoPlayerControls;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        m_videoPlayerControls.OnSliderPointerDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_videoPlayerControls.OnSliderPointerUp();
    }
}

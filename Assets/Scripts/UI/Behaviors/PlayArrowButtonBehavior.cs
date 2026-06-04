using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayArrowButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Title("Set in inspector")]
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private ColorVariable m_normalColor;
    [SerializeField] private ColorVariable m_hoveredColor;
    [SerializeField] private RectTransform m_container;
    [SerializeField] private Image m_image;
    
    private ButtonState m_state;
    
    public void OnPointerEnter(PointerEventData e) => SetHighlight(true);
    public void OnPointerExit(PointerEventData e)  => SetHighlight(false);
    public void OnSelect(BaseEventData e)          => SetHighlight(true);
    public void OnDeselect(BaseEventData e)        => SetHighlight(false);
    
    
    private void SetState(ButtonState state)
    {
        m_state = state;
        
        if (state == ButtonState.Normal)
        {
            m_image.color = m_normalColor.Color;
            m_container.DOScale(1f, 0.2f);
            InputManager.Instance.SetSelected(gameObject);
        }
        else if (state == ButtonState.Hovered)
        {
            m_image.color = m_hoveredColor.Color;
            m_container.DOScale(1.2f, 0.2f);
        }
    }
    
    void SetHighlight(bool hovered)
    {
        if (hovered)
        {
            SetState(ButtonState.Hovered);
            m_audioSource.Play();
        }
        else
        {
            SetState(ButtonState.Normal);
        }
    }
}

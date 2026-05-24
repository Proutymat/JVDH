using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrowButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Title("Set in inspector")]
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private ColorVariable m_normalColor;
    [SerializeField] private ColorVariable m_hoveredColor;
    [SerializeField] private RectTransform m_container;
    [SerializeField] private Image m_image;
    
    private ButtonState m_state;
    
    private void SetState(ButtonState state)
    {
        m_state = state;
        
        if (state == ButtonState.Normal)
        {
            m_image.color = m_normalColor.Color;
            m_container.DOScale(1f, 0.2f);
        }
        else if (state == ButtonState.Hovered)
        {
            m_image.color = m_hoveredColor.Color;
            m_container.DOScale(1.2f, 0.2f);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetState(ButtonState.Hovered);
        m_audioSource.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetState(ButtonState.Normal);
    }
}

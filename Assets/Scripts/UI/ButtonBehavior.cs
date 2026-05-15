using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;

public class ButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Title("Parameters")] 
    [SerializeField] private float m_hoverAnimDuration;
    
    [Title("Set in inspector")]
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private ColorVariable m_normalColor;
    [SerializeField] private ColorVariable m_hoveredColor;
    [SerializeField] private TMP_Text m_text;
    [SerializeField] private RectTransform  m_buttonRect;
    [SerializeField] private RectTransform m_hoverBackgroundRect;
    

    private ButtonState m_state;


    private void Awake()
    {
        m_text.color = m_normalColor.Color;
        m_hoverBackgroundRect.transform.DOScaleX(0f, 0f);
    }

    private void SetState(ButtonState state)
    {
        m_state = state;
        
        if (state == ButtonState.Normal)
        {
            m_text.color = m_normalColor.Color;
            m_hoverBackgroundRect.transform.DOScaleX(0f, m_hoverAnimDuration);
        }
        else if (state == ButtonState.Hovered)
        {
            m_text.color = m_hoveredColor.Color;
            AlignBackground();
            m_hoverBackgroundRect.transform.DOScaleX(1f, m_hoverAnimDuration);
        }
    }
    
    private void AlignBackground()
    {
        Vector3 pos = m_hoverBackgroundRect.position;
        pos.y = m_buttonRect.position.y;
        m_hoverBackgroundRect.position = pos;
        m_hoverBackgroundRect.pivot = new Vector2(0f, 0.5f);
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

    public void ButtonClick()
    {
        SetState(ButtonState.Normal);
    }
}

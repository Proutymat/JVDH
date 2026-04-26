using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum ButtonState
    {
        Normal,
        Hovered,
        Clicked
    }
    
    [Header("Set in inspector")]
    [SerializeField] private ColorVariable m_normalColor;
    [SerializeField] private ColorVariable m_hoveredColor;
    [SerializeField] private TMP_Text m_text;
    [SerializeField] private GameObject m_buttonObject;

    private ButtonState m_state;


    private void Start()
    {
        SetState(ButtonState.Normal);
    }

    private void SetState(ButtonState state)
    {
        m_state = state;
        
        if (state == ButtonState.Normal)
        {
            m_text.color = m_normalColor.Color;
        }
        else if (state == ButtonState.Hovered)
        {
            m_text.color = m_hoveredColor.Color;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("COUCOU");
        SetState(ButtonState.Hovered);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetState(ButtonState.Normal);
    }
}

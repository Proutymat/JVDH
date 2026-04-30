using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    [Header("Set in inspector")]
    [SerializeField] private FontStyles baseStyle;
    [SerializeField] private TMP_Text m_text;
    
    private ButtonState m_state;
    
    
    private void SetState(ButtonState state)
    {
        m_state = state;
        
        if (state == ButtonState.Hovered)
            m_text.fontStyle = baseStyle | FontStyles.Underline;
        else
            m_text.fontStyle = baseStyle;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetState(ButtonState.Hovered);
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

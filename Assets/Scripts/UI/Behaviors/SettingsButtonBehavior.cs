using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    
    [Title("Set in inspector")]
    [SerializeField] private FontStyles baseStyle;
    [SerializeField] private TMP_Text m_text;
    
    private ButtonState m_state;
    
    
    public void OnPointerEnter(PointerEventData e) => SetHighlight(true);
    public void OnPointerExit(PointerEventData e)  => SetHighlight(false);
    public void OnSelect(BaseEventData e)          => SetHighlight(true);
    public void OnDeselect(BaseEventData e)        => SetHighlight(false);

    void SetHighlight(bool hovered)
    {
        if (hovered)
        {
            SetState(ButtonState.Hovered);
            if (EventSystem.current.currentSelectedGameObject != gameObject)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }
        else
        {
            SetState(ButtonState.Normal);
        }
    }
    
    
    private void SetState(ButtonState state)
    {
        m_state = state;
        
        if (state == ButtonState.Hovered)
            m_text.fontStyle = baseStyle | FontStyles.Underline;
        else
            m_text.fontStyle = baseStyle;
    }

    public void ButtonClick()
    {
        // Clic sound
    }

}

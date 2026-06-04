using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniaVideoBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Title("Set in inspector")] 
    [SerializeField] private RectTransform m_container;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private Button m_backButton;
    [SerializeField] private Button m_previewButton;
    
    public void OnPointerEnter(PointerEventData e) => SetHighlight(true);
    public void OnPointerExit(PointerEventData e)  => SetHighlight(false);
    public void OnSelect(BaseEventData e)          => SetHighlight(true);
    public void OnDeselect(BaseEventData e)        => SetHighlight(false);
    
    void SetHighlight(bool hovered)
    {
        if (hovered)
        {
            Button currentSelected = gameObject.GetComponent<Button>();
            
            // Update back button nav
            Navigation back = m_backButton.navigation;
            back.selectOnUp = currentSelected;
            m_backButton.navigation = back;
            
            // Update preview button nav
            Navigation preview = m_previewButton.navigation;
            preview.selectOnDown = currentSelected;
            m_previewButton.navigation = preview;
            
            m_container.DOScale(1.1f, 0.2f);
            m_audioSource.Play();
            InputManager.Instance.SetSelected(gameObject);
        }
        else
        {
            m_container.DOScale(1.0f, 0.2f);
        }
    }
}

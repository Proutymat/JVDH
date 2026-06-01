using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiniaVideoBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Title("Set in inspector")] 
    [SerializeField] private RectTransform m_container;
    [SerializeField] private AudioSource m_audioSource;
    
    public void OnPointerEnter(PointerEventData e) => SetHighlight(true);
    public void OnPointerExit(PointerEventData e)  => SetHighlight(false);
    public void OnSelect(BaseEventData e)          => SetHighlight(true);
    public void OnDeselect(BaseEventData e)        => SetHighlight(false);
    
    void SetHighlight(bool hovered)
    {
        if (hovered)
        {
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

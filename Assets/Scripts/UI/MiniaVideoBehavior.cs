using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiniaVideoBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Title("Set in inspector")] 
    [SerializeField] private RectTransform m_container;
    [SerializeField] private AudioSource m_audioSource;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_container.DOScale(1.1f, 0.2f);
        m_audioSource.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_container.DOScale(1.0f, 0.2f);
    }
}

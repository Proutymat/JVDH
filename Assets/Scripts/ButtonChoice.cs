using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonChoice : MonoBehaviour
{
    [Header("Button Ref")]
    [SerializeField] private TMP_Text textButtonRef;
    [SerializeField] private GameObject goButtonRef;
    
    [Space(10)]
    [Header("Settings")]
    [SerializeField] private string textButton;
    public VideoNodeSO videoNode;
    
    [Space(10)]
    [Header("Animation")]
    [SerializeField] private float scaleUnhovered = 1.0f;
    [SerializeField] private float scaleHovered = 1.2f;
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private Ease easeHovered = Ease.InOutFlash;
    
    private ButtonState m_state;
    

    private void OnValidate() => SetTextButton(textButton);

    public void InitButton(ChoiceSO choice)
    {
        if (!choice) return;

        videoNode = choice.nextNode;
        SetTextButton(choice.label);
        goButtonRef.transform.position = choice.screenPosition;
        goButtonRef.SetActive(true);
    }

    // TODO : Modify to manage specific text
    private void SetTextButton(string txt)
    {
        if (!textButtonRef) return;
        
        textButtonRef.text = txt;
    }
    
    private void SetState(ButtonState state)
    {
        m_state = state;

        if (!goButtonRef) return;
        goButtonRef.transform.DOScale(
            state == ButtonState.Normal ? scaleUnhovered : scaleHovered, 
            scaleDuration).SetEase(easeHovered);
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
        
        // Communicate with VideoTreePlayer
        VideoTreePlayer.instance.OnChoiceSelected(this);
    }
}

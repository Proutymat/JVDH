using UnityEngine;
using UnityEngine.Video;

public class VideoTreePlayer : MonoBehaviour
{
    public static VideoTreePlayer instance;
    
    [SerializeField] private VideoNodeSO startVideoNode;
    [SerializeField] private ButtonChoice[] buttonsGO;
    
    private VideoNodeSO _currentNode;

    private void Awake() => instance = this;
    
    public void StartVideoTree() => PlayNode(startVideoNode);

    private void PlayNode(VideoNodeSO node)
    {
        _currentNode = node;
        VideoManager.Instance.PlayVideoNode(node);
    }

    public void AppearChoiceButton()
    {
        for (var i = 0; i < _currentNode.choices.Count; i++)
            buttonsGO[i].InitButton(_currentNode.choices[i]);
    }

    public void OnChoiceSelected(ButtonChoice buttonChoice)
    {
        if (buttonChoice && buttonChoice.videoNode) 
            PlayNode(buttonChoice.videoNode);
    }
}

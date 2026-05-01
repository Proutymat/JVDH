using UnityEngine;
using UnityEngine.Video;

public class VideoTreePlayer : MonoBehaviour
{
    public static VideoTreePlayer instance;
    [SerializeField] private VideoNodeSO startVideoNode;
    
    private VideoNodeSO _currentNode;

    private void Awake() => instance = this;
    
    public void StartVideoTree() => PlayNode(startVideoNode);

    private void PlayNode(VideoNodeSO node)
    {
        _currentNode = node;
        VideoManager.Instance.PlayClip(node.videoClip);
    }

    public void OnChoiceSelected(int index)
    {
        var next = _currentNode.choices[index]?.nextNode;
        if (next != null) PlayNode(next);
    }
}

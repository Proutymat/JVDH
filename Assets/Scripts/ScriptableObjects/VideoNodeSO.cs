using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "VideoNode", menuName = "Video Decision Tree/Video Node")]
public class VideoNodeSO : ScriptableObject
{
    [Header("Content")]
    public VideoClip videoClip;
    [TextArea(2, 4)]
    public string description;

    [Header("Choices")]
    public List<ChoiceSO> choices = new List<ChoiceSO>();

    [Header("Settings")]
    public bool isEndNode = false;

    // Editor-only: position in the graph
    [HideInInspector] public Vector2 graphPosition;
}
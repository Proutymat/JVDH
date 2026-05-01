using UnityEngine;

[CreateAssetMenu(fileName = "Choice", menuName = "Video Decision Tree/Choice")]
public class ChoiceSO : ScriptableObject
{
    [Header("Display")]
    public string label = "Choix...";

    [Header("Navigation")]
    public VideoNodeSO nextNode;
}
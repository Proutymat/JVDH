using UnityEngine;

[CreateAssetMenu(fileName = "Choice", menuName = "Video Decision Tree/Choice")]
public class ChoiceSO : ScriptableObject
{
    [Header("Display")]
    public string label = "Choix...";
    public Vector3 screenPosition = Vector3.zero;

    [Header("Navigation")]
    public VideoNodeSO nextNode;
}
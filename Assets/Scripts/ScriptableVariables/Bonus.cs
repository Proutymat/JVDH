using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Bonus", menuName = "Scriptable Objects/Bonus")]
public class Bonus : ScriptableObject
{
    public int bonusIndex;
    public Sprite previewMiniature;
    public VideoClip videoClip;
    public LocalizedString  titleKey;
    public LocalizedString  descriptionKey;
    
}

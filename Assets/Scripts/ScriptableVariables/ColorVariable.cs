using UnityEngine;

[CreateAssetMenu(fileName = "Color", menuName = "Scriptable Objects/Color")]
public class ColorVariable : ScriptableObject
{
    [SerializeField] private Color m_color;

    public Color Color{ get => m_color; }
}

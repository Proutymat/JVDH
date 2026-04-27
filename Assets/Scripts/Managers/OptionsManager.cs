using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public enum Language
    {
        French,
        English
    }
    
    private float m_globalVolume;
    private bool m_subtitles;
    private Language m_language;
    private bool m_windowedMode;
    
    
    public void ClickButton()
    {
        GameManager.Instance.LoadOptionsMenu();
    }
}

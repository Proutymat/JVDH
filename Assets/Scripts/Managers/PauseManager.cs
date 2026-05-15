using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Set in Inspector")]
    [SerializeField] private CanvasGroup m_mainPausePanel;
    [SerializeField] private CanvasGroup m_settingsPausePanel;

    private void Start()
    {
        ShowInGameSettings(false);
    }

    private static void ShowCanvasGroup(bool show, CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = show ? 1 : 0;
        canvasGroup.blocksRaycasts = show;
        canvasGroup.interactable = show;
    }
    
    public void ShowInGameSettings(bool show)
    {
        if (show)
        {
            ShowCanvasGroup(true, m_settingsPausePanel);
            ShowCanvasGroup(false, m_mainPausePanel);
        }
        else
        {
            ShowCanvasGroup(false, m_settingsPausePanel);
            ShowCanvasGroup(true, m_mainPausePanel);
        }
            
    }
}

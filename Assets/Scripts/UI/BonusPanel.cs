using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.ActionResolvers;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using UnityEngine.Video;

public class BonusPanel : MonoBehaviour
{
    [Title("Set in Inspector")]
    [SerializeField] private CanvasGroup m_previewLockState;
    [SerializeField] private CanvasGroup m_previewPlayState;
    [SerializeField] private CanvasGroup m_previousButton;
    [SerializeField] private CanvasGroup m_nextButton;
    [SerializeField] private List<GameObject> m_bonusMiniatures;
    [SerializeField] private TMP_Text m_titleText;
    [SerializeField] private TMP_Text m_descriptionText;
    [SerializeField] private Image m_previewImage;
    [SerializeField] private Bonus m_firstBonus;
    [SerializeField] private InputActionReference m_escapeVideoAction;
    [SerializeField] private CanvasGroup m_closeVideoButton;
    [SerializeField] private VideoPlayerControls m_videoPlayerControls;
    
    private int m_currentPage;
    private Bonus m_currentBonus;
    private bool m_isPlayingVideo;
    
    private void Start()
    {
        m_isPlayingVideo = false;
        ResetUI();
    }
    

    public void ResetUI()
    {
        m_currentPage = 0;
        PanelManager.ShowCanvasGroup(false, m_previousButton);
        PanelManager.ShowCanvasGroup(true, m_nextButton);
        
        // Disable every page and set lock state
        for (int i = 0; i < m_bonusMiniatures.Count; i++)
        {
            m_bonusMiniatures[i].SetActive(false);
            m_bonusMiniatures[i].transform.GetChild(0).gameObject.SetActive(!SaveManager.Instance.Data.progression.success[i]);
        }
        
        // Enable first page
        for (int i = 0; i < 4; i++)
        {
            m_bonusMiniatures[i].SetActive(true);
        }

        UpdatePreview(m_firstBonus);
    }

    public void NextPage()
    {
        // Hide last page and show new page
        int i = 0;
        while (i < 8 && i + m_currentPage * 4 < m_bonusMiniatures.Count)
        {
            m_bonusMiniatures[i + m_currentPage * 4].SetActive(i > 3);
            i++;
        }
        
        m_currentPage++;
        
        // Show/hide arrow buttons
        PanelManager.ShowCanvasGroup(true, m_previousButton);
        if (m_currentPage == Math.Ceiling(m_bonusMiniatures.Count / 4f) - 1)
        {
            PanelManager.ShowCanvasGroup(false, m_nextButton);
        }
    }

    public void PreviousPage()
    {
        // Hide last page and show new page
        int i = 7;
        if (m_currentPage == Math.Ceiling(m_bonusMiniatures.Count / 4f) - 1)
        {
            i = m_bonusMiniatures.Count % 4 == 0 ? 7 : (m_bonusMiniatures.Count % 4) + 3 ;
        }
        
        m_currentPage--;
        
        while (i >= 0)
        {
            m_bonusMiniatures[i + m_currentPage * 4].SetActive(i < 4);
            i--;
        }

        // Show/hide arrow buttons
        PanelManager.ShowCanvasGroup(true, m_nextButton);
        if (m_currentPage == 0)
        {
            PanelManager.ShowCanvasGroup(false, m_previousButton);
        }
    }

    public void UpdatePreview(Bonus bonus)
    {
        m_currentBonus = bonus;
        
        PanelManager.ShowCanvasGroup(!SaveManager.Instance.Data.progression.success[bonus.bonusIndex], m_previewLockState);
        m_previewPlayState.alpha = SaveManager.Instance.Data.progression.success[bonus.bonusIndex] ? 1 : 0;
        
        // Update preview
        m_titleText.text = m_currentBonus.titleKey.GetLocalizedString();
        m_descriptionText.text = m_currentBonus.descriptionKey.GetLocalizedString();
        m_previewImage.sprite = m_currentBonus.previewMiniature;
    }

    public void OpenVideoPlayer()
    {
        m_isPlayingVideo = true;
        VideoManager.Instance.Stop();
        VideoManager.Instance.PlayClip(m_currentBonus.videoClip);
        SoundManager.Instance.StopMusic();
        GameManager.Instance.GameState = GameState.VideoPlayer;
        PanelManager.Instance.SetPanel(PanelState.Game, FadeStyle.FadeIn, null, null, VideoManager.Instance.UnPause);
        VideoManager.Instance.GetVideoPlayer.loopPointReached += CloseVideoPlayer;
        m_videoPlayerControls.ShowControls(false);
        m_closeVideoButton.alpha = 1;
    }

    public void CloseVideoPlayer(VideoPlayer _)
    {
        m_isPlayingVideo = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PanelManager.Instance.SetPanel(PanelState.Bonus);
        SoundManager.Instance.PlayMenuMusic(true);
        VideoManager.Instance.PlayMainMenuClip();
        VideoManager.Instance.UnPause();
        GameManager.Instance.GameState = GameState.MainMenu;
        VideoManager.Instance.GetVideoPlayer.loopPointReached -= CloseVideoPlayer;
        m_closeVideoButton.alpha = 0;
    }

    private void Update()
    {
        if (m_isPlayingVideo && m_escapeVideoAction.action.WasPerformedThisFrame())
        {
            CloseVideoPlayer(VideoManager.Instance.GetVideoPlayer);
        }
    }
}

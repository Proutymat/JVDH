using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class BonusPanel : MonoBehaviour
{
    [Title("Set in Inspector")]
    [SerializeField] private List<GameObject> m_bonusMiniatures;
    [SerializeField] private TMP_Text m_titleText;
    [SerializeField] private TMP_Text m_descriptionText;
    [SerializeField] private TMP_Text m_nameText;
    [SerializeField] private Bonus m_firstBonus;
    [SerializeField] private InputActionReference m_escapeVideoAction;
    [SerializeField] private CanvasGroup m_closeVideoButton;
    [SerializeField] private VideoPlayerControls m_videoPlayerControls;
    [Title("Preview minia", horizontalLine: false)]
    [SerializeField] private Image m_previewImage;
    [SerializeField] private CanvasGroup m_previewLockState;
    [SerializeField] private CanvasGroup m_previewPlayState;
    [SerializeField] private Button m_previewPlayButton;
    [SerializeField] private PreviewShake m_previewShake;
    [SerializeField] private InputActionReference m_upAction;
    [Title("Previous and Next buttons", horizontalLine: false)]
    [SerializeField] private CanvasGroup m_previousCanvas;
    [SerializeField] private CanvasGroup m_nextCanvas;
    [SerializeField] private Button m_previousButton;
    [SerializeField] private Button m_nextButton;
    
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
        m_currentBonus = m_firstBonus;
        PanelManager.ShowCanvasGroup(false, m_previousCanvas);
        PanelManager.ShowCanvasGroup(true, m_nextCanvas);
        
        // Disable every page and set lock state
        for (int i = 0; i < m_bonusMiniatures.Count; i++)
        {
            m_bonusMiniatures[i].SetActive(false); // Disable bonuses
            m_bonusMiniatures[i].transform.GetChild(0).gameObject.SetActive(!SaveManager.Instance.Data.progression.success[i]); // Set lock state
            m_bonusMiniatures[i].transform.GetChild(1).gameObject.SetActive(false); // Disable  selected border
        }
        
        m_bonusMiniatures[0].transform.GetChild(1).gameObject.SetActive(true); // Enable first bonus selected border
        
        // Enable first page
        for (int i = 0; i < 4; i++)
        {
            m_bonusMiniatures[i].SetActive(true);
        }

        UpdatePreview(m_firstBonus);

        
        // Previous button navigation
        Navigation previous = m_previousButton.navigation;
        previous.selectOnRight = m_bonusMiniatures[4].GetComponent<Button>();
        m_previousButton.navigation = previous;
        
        // Next button navigation
        Navigation next = m_nextButton.navigation;
        next.selectOnLeft = m_bonusMiniatures[3].GetComponent<Button>();
        m_nextButton.navigation = next;
    }

    public void NextPage()
    {
        // Hide last bonuses page and show new bonuses page
        int i = 0;
        while (i < 8 && i + m_currentPage * 4 < m_bonusMiniatures.Count)
        {
            m_bonusMiniatures[i + m_currentPage * 4].SetActive(i > 3);
            i++;
        }
        
        m_currentPage++;
        
        // Show/hide arrow buttons
        PanelManager.ShowCanvasGroup(true, m_previousCanvas);
        if (m_currentPage == Math.Ceiling(m_bonusMiniatures.Count / 4f) - 1)
        {
            PanelManager.ShowCanvasGroup(false, m_nextCanvas);
        }
        else
        {
            // Next button navigation
            Navigation next = m_nextButton.navigation;
            next.selectOnLeft = m_bonusMiniatures[3 + m_currentPage * 4].GetComponent<Button>();
            m_nextButton.navigation = next;
        }
        
        // Previous button navigation
        Navigation previous = m_previousButton.navigation;
        previous.selectOnRight = m_bonusMiniatures[0 + m_currentPage * 4].GetComponent<Button>();
        m_previousButton.navigation = previous;
        
        InputManager.Instance.SetSelected(m_bonusMiniatures[m_currentPage * 4]);
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
        PanelManager.ShowCanvasGroup(true, m_nextCanvas);
        if (m_currentPage == 0)
        {
            PanelManager.ShowCanvasGroup(false, m_previousCanvas);
        }
        else
        {
            // Previous button navigation
            Navigation previous = m_previousButton.navigation;
            previous.selectOnRight = m_bonusMiniatures[0 + m_currentPage * 4].GetComponent<Button>();
            m_previousButton.navigation = previous;
        }
        
        // Next button navigation
        Navigation next = m_nextButton.navigation;
        next.selectOnLeft = m_bonusMiniatures[3 + m_currentPage * 4].GetComponent<Button>();
        m_nextButton.navigation = next;
        
        InputManager.Instance.SetSelected(m_bonusMiniatures[3 + m_currentPage * 4]);
    }

    public void UpdatePreview(Bonus bonus)
    {
        m_bonusMiniatures[m_currentBonus.bonusIndex].transform.GetChild(1).gameObject.SetActive(false); // Disable old selected border
        
        m_currentBonus = bonus;
        
        m_bonusMiniatures[m_currentBonus.bonusIndex].transform.GetChild(1).gameObject.SetActive(true); // Enable current selected border
        
        PanelManager.ShowCanvasGroup(!SaveManager.Instance.Data.progression.success[bonus.bonusIndex], m_previewLockState);
        m_previewPlayState.alpha = SaveManager.Instance.Data.progression.success[bonus.bonusIndex] ? 1 : 0;
        
        // Update preview
        m_titleText.text = m_currentBonus.titleKey.GetLocalizedString();
        m_descriptionText.text = m_currentBonus.descriptionKey.GetLocalizedString();
        m_nameText.text = m_currentBonus.nameKey.GetLocalizedString();
        m_previewImage.sprite = m_currentBonus.previewMiniature;

        // Update bonuses up navigation
        for (int i = 0; i < m_bonusMiniatures.Count; i++)
        {
            Button button = m_bonusMiniatures[i].GetComponent<Button>();
            Navigation nav = button.navigation;
            nav.selectOnUp = m_previewPlayState.alpha == 1 ? m_previewPlayButton : null;
            button.navigation = nav;
        }
        
        // Update next arrow up-navigation
        Navigation nextNav = m_nextButton.navigation;
        nextNav.selectOnUp = m_previewPlayState.alpha == 1 ? m_previewPlayButton : null;
        m_nextButton.navigation = nextNav;
        
        // Update previous arrow up-navigation
        Navigation previousNav = m_previousButton.navigation;
        previousNav.selectOnUp = m_previewPlayState.alpha == 1 ? m_previewPlayButton : null;
        m_previousButton.navigation = previousNav;
    }

    public void OpenVideoPlayer()
    {
        m_isPlayingVideo = true;
        VideoManager.Instance.Stop();
        VideoManager.Instance.PlayClip(m_currentBonus.videoClip);
        SoundManager.Instance.StopMusic();
        GameManager.Instance.GetGameState = GameState.VideoPlayer;
        PanelManager.Instance.SetPanel(PanelState.Game, FadeStyle.FadeIn, null, null, VideoManager.Instance.UnPause);
        VideoManager.Instance.GetVideoPlayer.loopPointReached += CloseVideoPlayer;
        m_videoPlayerControls.ShowControls(false);
        PanelManager.ShowCanvasGroup(true, m_closeVideoButton);

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
        GameManager.Instance.GetGameState = GameState.MainMenu;
        VideoManager.Instance.GetVideoPlayer.loopPointReached -= CloseVideoPlayer;
        PanelManager.ShowCanvasGroup(false, m_closeVideoButton);
        Debug.Log("CloseVideoPlayer");
    }

    private void Update()
    {
        if (m_isPlayingVideo && m_escapeVideoAction.action.WasPerformedThisFrame())
        {
            CloseVideoPlayer(VideoManager.Instance.GetVideoPlayer);
        }
    }
}

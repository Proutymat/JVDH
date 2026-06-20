using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;


public class VideoPlayerControls : MonoBehaviour
{
    
    [Title("Parameters")]
    [SerializeField] private float m_autoHideDuration;
    [SerializeField] private float m_arrowSeekStep;
    [SerializeField] private float m_holdInitialDelay = 0.3f;
    [SerializeField] private float m_holdRepeatRate = 0.05f;
    
    [Title("Set in Inspector")]
    [SerializeField] private VideoPlayer m_videoPlayer;
    [SerializeField] private CanvasGroup m_controlsCanvasGroup;
    [SerializeField] private Slider m_videoSlider;
    [SerializeField] private TMP_Text m_subtitleText;
    [Title("Input Actions", horizontalLine: false)]
    [SerializeField] private InputActionReference m_previousAction;
    [SerializeField] private InputActionReference m_nextAction;
    [SerializeField] private InputActionReference m_pauseVideoAction;
    [SerializeField] private InputActionReference m_playBackwardAction;
    [SerializeField] private InputActionReference m_playForwardAction;
    [Title("Pause/Play", horizontalLine: false)]
    [SerializeField] private Sprite m_pauseSprite;
    [SerializeField] private Sprite m_playSprite;
    [SerializeField] private Image m_image;
    
    private bool m_isDraggingSlider;
    private bool m_isSeekingSlider;
    private double m_targetSeekTime;
    private float m_nextSeekTimer;
    private float m_previousSeekTimer;
    private float m_autoHideTimer;
    private Vector2 m_lastMousePosition;
    private float m_heldTime;
    private float m_repeatTimer;
    private bool m_enableScrollbar;
    private bool m_show;


    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Start()
    {
        m_autoHideTimer = 0;
        m_isDraggingSlider = false;
        m_isSeekingSlider = false;
        m_show = false;
        m_enableScrollbar = false;
        
        ShowControls(false);
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------
    
    public void OnSliderPointerDown()
    {
        m_isDraggingSlider = true;
        ShowControls(true);
        m_autoHideTimer = 0f;
    }
    
    public void OnSliderPointerUp()
    {
        if (m_videoPlayer.clip == null) return;

        double targetTime = m_videoSlider.value * m_videoPlayer.length;

        m_isDraggingSlider = false;
        m_isSeekingSlider = true;
        
        m_targetSeekTime = targetTime;
        m_videoPlayer.time = targetTime;
        
        m_autoHideTimer = 0f;
    }
    
    public void OnSliderValueChanged(float value)
    {
        if (!m_isDraggingSlider) return;

        m_videoPlayer.time = value * m_videoPlayer.length;
    }
    
    public void EnableScrollbar(bool enable)
    {
        m_enableScrollbar = enable;
        m_videoSlider.gameObject.SetActive(enable);
    }

    public void ShowControls(bool show)
    {
        if ((!SaveManager.Instance.Data.settings.videoPlayerControls || !m_enableScrollbar) && show) return;
        
        if (!show)
            m_lastMousePosition =  Mouse.current.position.ReadValue();
        else
            m_autoHideTimer = 0;
        
        m_controlsCanvasGroup.alpha = show ? 1 : 0;
        m_controlsCanvasGroup.blocksRaycasts = show;
        m_controlsCanvasGroup.interactable = show;
        
        m_subtitleText.alignment = show ? TextAlignmentOptions.Top : TextAlignmentOptions.Bottom;
        
        m_show = show;
    }
    
    
    private void UpdateAutoHide()
    {
        Vector2 currentMousePos = Mouse.current.position.ReadValue();

        // Has cursor moved
        if (!m_show && currentMousePos != m_lastMousePosition)
        {
            m_lastMousePosition = currentMousePos;
            m_autoHideTimer = 0f;
            
            PanelManager.ShowCursor(true);
            if (SaveManager.Instance.Data.settings.videoPlayerControls)
            {
                ShowControls(true);
            }
        }

        // Currently moving slider
        if (m_isDraggingSlider) return;

        m_autoHideTimer += Time.deltaTime;

        if (m_autoHideTimer >= m_autoHideDuration)
        {
            PanelManager.ShowCursor(false);
            ShowControls(false);
        }
    }

    private void HandleSpacebarInput()
    {
        if (m_pauseVideoAction.action.WasPerformedThisFrame())
        {
            if (m_videoPlayer.isPlaying)
            {
                VideoManager.Instance.Pause();
            }
            else
            {
                VideoManager.Instance.UnPause();
            }
        }
    }
    
    private void HandleArrowInput()
    {

        if (m_playBackwardAction.action.WasPerformedThisFrame())
        {
            // hh
        }
        else if (m_playForwardAction.action.WasPerformedThisFrame())
        {
            //
        }
        
        Debug.Log("Playback speed =  " + m_videoPlayer.playbackSpeed);
    }

    private void UpdateSlider()
    {
        if (m_isDraggingSlider) return;

        // Do not update the slider if it's being dragged
        if (m_isSeekingSlider)
        {
            if (Mathf.Abs((float)(m_videoPlayer.time - m_targetSeekTime)) < 0.1f)
            {
                m_isSeekingSlider = false;
            }
            return;
        }

        // Update slider
        if (m_videoPlayer.length > 0)
        {
            m_videoSlider.value = (float)(m_videoPlayer.time / m_videoPlayer.length);
        }
    }
    
    public void VideoClick()
    {
        if (!SaveManager.Instance.Data.settings.videoPlayerControls || GameManager.Instance.GetGameState == GameState.MainMenu) return;
        
        if (VideoManager.Instance.GetVideoPlayer.isPlaying)
        {
            VideoManager.Instance.Pause();
            m_image.sprite = m_pauseSprite;
        }
        else
        {
            VideoManager.Instance.UnPause();
            m_image.sprite = m_playSprite;
        }
        
        ShowControls(true);
    }
    
    private void Update()
    {
        // Update video controls only in play mode (will not be shown otherwise)
        if (GameManager.Instance.GetGameState != GameState.Game && GameManager.Instance.GetGameState != GameState.VideoPlayer) return;
        
        UpdateAutoHide();

        if (!SaveManager.Instance.Data.settings.videoPlayerControls) return;
        
        HandleSpacebarInput();
        HandleArrowInput();
        UpdateSlider();
    }
}

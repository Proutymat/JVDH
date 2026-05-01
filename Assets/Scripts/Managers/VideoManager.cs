using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    private static VideoManager m_instance;
    public static VideoManager Instance => m_instance;

    [Header("Parameters")]
    [SerializeField] private float m_autoHideDuration;

    [Header("Set in Inspector")]
    [SerializeField] private RenderTexture m_renderTexture;
    [SerializeField] private VideoPlayer m_videoPlayer;
    [SerializeField] private VideoClip m_menuClip;
    [SerializeField] private CanvasGroup m_controlsCanvasGroup;
    [SerializeField] private Slider m_videoSlider;
    [SerializeField] private TMP_Text m_subtitleText;
    
    private bool m_isDragging;
    private bool m_isSeeking;
    private double m_targetSeekTime;
    private float m_autoHideTimer;
    private Vector2 m_lastMousePosition;
    private bool m_controlsVisible;

    public VideoPlayer GetVideoPlayer { get => m_videoPlayer; }
    
    /*
     * Init singleton
     */
    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning("Multiple VideoManager instances in scene!");
            Destroy(gameObject);
            return;
        }

        m_instance = this;
    }

    public void Initialize()
    {
        PlayClip(m_menuClip, true);
        m_autoHideTimer = 0;
        m_controlsVisible = false;
        m_isDragging = false;
        m_isSeeking = false;
    }
    
    private void Update()
    {
        if (m_videoPlayer.clip == null) return;
        if (PanelManager.Instance.GetPanelState != PanelManager.PanelState.Game) return;

        HandleKeyboardInputs();
        HandleAutoHide();
        
        if (m_isDragging) return;

        // Do not update the slider if it's being dragged
        if (m_isSeeking)
        {
            if (Mathf.Abs((float)(m_videoPlayer.time - m_targetSeekTime)) < 0.1f)
            {
                m_isSeeking = false;
            }
            return;
        }

        // Update slider
        if (m_videoPlayer.length > 0)
        {
            m_videoSlider.value = (float)(m_videoPlayer.time / m_videoPlayer.length);
        }
    }
    
    
    // --------------------------------------------
    //                  CORE FUNCTIONS
    // --------------------------------------------

    /*
     * Used to play a clip video with possibility to loop
     */
    public void PlayClip(VideoClip clip, bool loop = false)
    {
        if (clip == null) return;

        SubtitleManager.Instance.SetSRTFile();
        
        m_videoPlayer.Stop();
        m_videoPlayer.clip = clip;
        m_videoPlayer.isLooping = loop;
        m_videoPlayer.Prepare();
        m_videoPlayer.prepareCompleted += OnPrepared;
    }
    
    /*
     * Stop Video 
     */
    public void Stop()
    {
        m_videoPlayer.Stop();
        m_videoPlayer.clip = null;

        if (m_renderTexture != null)
        {
            RenderTexture activeRT = RenderTexture.active;
            RenderTexture.active = m_renderTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = activeRT;
        }
    }
    
    /*
     * Pause Video
     */
    public void Pause() => m_videoPlayer.Pause();
    
    /*
     * Unpause Video
     */
    public void UnPause() => m_videoPlayer.Play();
    
    /*
     * Hide Video controls
     */
    public void HideControls()
    {
        if (!m_controlsVisible) return;

        m_controlsVisible = false;
        m_controlsCanvasGroup.alpha = 0f;
        m_controlsCanvasGroup.blocksRaycasts = false;
        m_controlsCanvasGroup.interactable = false;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        
        m_subtitleText.alignment = TextAlignmentOptions.Bottom;
    }
    
    // --------------------------------------------
    //                  FUNCTIONS HELPERS
    // --------------------------------------------
    
    /*
     * Buffer to load videos
     */
    private void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;
        vp.Play();
        PanelManager.Instance.HideBlackScreen();
    }

    public void PlayMainMenuClip()
    {
        PlayClip(m_menuClip, true);
    }
    
    #region Video PLayer Editor
    public void OnSliderPointerDown()
    {
        m_isDragging = true;
        ShowControls();
        m_autoHideTimer = 0f;
    }
    
    public void OnSliderPointerUp()
    {
        if (m_videoPlayer.clip == null) return;

        double targetTime = m_videoSlider.value * m_videoPlayer.length;

        m_isDragging = false;
        m_isSeeking = true;
        
        m_targetSeekTime = targetTime;
        m_videoPlayer.time = targetTime;
        
        m_autoHideTimer = 0f;
    }
    
    private void OnSliderValueChanged(float value)
    {
        if (!m_isDragging) return;

        m_videoPlayer.time = value * m_videoPlayer.length;
    }

    private void ShowControls()
    {
        if (m_controlsVisible) return;

        m_controlsVisible = true;
        m_controlsCanvasGroup.alpha = 1f;
        m_controlsCanvasGroup.blocksRaycasts = true;
        m_controlsCanvasGroup.interactable = true;
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        m_subtitleText.alignment = TextAlignmentOptions.Top;
    }
    
    private void HandleAutoHide()
    {
        Vector2 currentMousePos = Mouse.current.position.ReadValue();

        if (currentMousePos != m_lastMousePosition)
        {
            m_lastMousePosition = currentMousePos;
            m_autoHideTimer = 0f;
            ShowControls();
        }

        if (m_isDragging) return;

        m_autoHideTimer += Time.deltaTime;

        if (m_autoHideTimer >= m_autoHideDuration)
        {
            HideControls();
        }
    }
    
    private void HandleKeyboardInputs()
    {
        // Spacebar
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (m_videoPlayer.isPaused)
            {
                UnPause();
            }
            else
            {
                Pause();
            }
        }
    }
    #endregion
}

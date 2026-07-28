using System;
using System.Collections.Generic;
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
    [SerializeField] private float m_directionSwitchCooldown = 1f;
    
    [Title("Set in Inspector")]
    [SerializeField] private VideoPlayer m_forwardVideoPlayer;
    [SerializeField] private VideoPlayer m_backwardVideoPlayer;
    [SerializeField] private CanvasGroup m_backwardVideoPlayerCP;
    [SerializeField] private CanvasGroup m_forwardVideoPlayerCP;
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
    [Title("Forward/backward", horizontalLine: false)]
    [SerializeField] private Image m_backwardImage;
    [SerializeField] private Image m_forwardImage;
    [SerializeField] private List<Sprite> m_forwardSprites;

    
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
    private int m_speedLevel; // 0 : normal
    private List<int> m_speeds = new List<int> {1, 4, 10, 20};
    private bool m_wasForward;
    private bool m_backwardSeekPending;
    private bool m_forwardSeekPending;
    private float m_switchLockedUntil = -Mathf.Infinity;


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
        m_wasForward = true;
        
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
        if (m_forwardVideoPlayer.clip == null) return;

        double targetTime = m_videoSlider.value * m_forwardVideoPlayer.length;

        m_isDraggingSlider = false;
        m_isSeekingSlider = true;
        
        m_targetSeekTime = targetTime;
        m_forwardVideoPlayer.time = targetTime;
        
        m_autoHideTimer = 0f;
    }
    
    public void OnSliderValueChanged(float value)
    {
        if (!m_isDraggingSlider) return;

        m_forwardVideoPlayer.time = value * m_forwardVideoPlayer.length;
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
            if (m_forwardVideoPlayer.isPlaying || m_backwardVideoPlayer.isPlaying)
            {
                if (m_speedLevel == 0)
                {
                    VideoManager.Instance.Pause();
                }
                else
                {
                    m_speedLevel = 0;
                    SetNormalPlaybackspeed();
                }
            }
            else
            {
                VideoManager.Instance.UnPause();
            }
        }
    }

    private bool m_audioResetPending;

    private void ForceAudioReset(VideoPlayer vp, float volume)
    {
        if (m_audioResetPending) return;

        long targetFrame = vp.frame;
        m_audioResetPending = true;

        void OnPrepared(VideoPlayer p)
        {
            p.prepareCompleted -= OnPrepared;
            p.frame = targetFrame;
            p.Play();
            p.SetDirectAudioVolume(0, volume);
            m_audioResetPending = false;
        }

        vp.prepareCompleted += OnPrepared;
        vp.Stop();
        vp.Prepare();
    }
    
    private void SwitchToBackward()
    {
        if (m_backwardSeekPending || !m_forwardVideoPlayer.isPrepared || !m_backwardVideoPlayer.isPrepared)
            return;

        m_forwardVideoPlayer.Pause();

        long totalFrames = (long)m_forwardVideoPlayer.frameCount;
        long targetFrame = System.Math.Clamp(totalFrames - 1 - m_forwardVideoPlayer.frame, 0, totalFrames - 1);

        m_backwardSeekPending = true;
        m_backwardVideoPlayer.seekCompleted += OnBackwardSeekCompleted;
        m_backwardVideoPlayer.frame = targetFrame;
    }
    
    private void SwitchToForward()
    {
        if (m_forwardSeekPending || !m_forwardVideoPlayer.isPrepared || !m_backwardVideoPlayer.isPrepared)
            return;

        m_backwardVideoPlayer.Pause();

        long totalFrames = (long)m_forwardVideoPlayer.frameCount;
        long targetFrame = System.Math.Clamp(totalFrames - 1 - m_backwardVideoPlayer.frame, 0, totalFrames - 1);

        m_forwardSeekPending = true;
        m_forwardVideoPlayer.seekCompleted += OnForwardSeekCompleted;
        m_forwardVideoPlayer.frame = targetFrame;
    }

    private void OnBackwardSeekCompleted(VideoPlayer vp)
    {
        vp.seekCompleted -= OnBackwardSeekCompleted;
        m_backwardSeekPending = false;

        PanelManager.ShowCanvasGroup(false, m_forwardVideoPlayerCP);
        PanelManager.ShowCanvasGroup(true, m_backwardVideoPlayerCP);
        vp.Play();
        vp.SetDirectAudioVolume(0, 0f);

        m_wasForward = false;
    }

    private void OnForwardSeekCompleted(VideoPlayer vp)
    {
        vp.seekCompleted -= OnForwardSeekCompleted;
        m_forwardSeekPending = false;

        PanelManager.ShowCanvasGroup(false, m_backwardVideoPlayerCP);
        PanelManager.ShowCanvasGroup(true, m_forwardVideoPlayerCP);

        vp.EnableAudioTrack(0, false);
        vp.EnableAudioTrack(0, true);
        vp.Play();
        vp.SetDirectAudioVolume(0, 1f);

        m_wasForward = true;
    }

    private void SetNormalPlaybackspeed()
    {
        m_forwardVideoPlayer.playbackSpeed = m_speeds[0];
        m_backwardImage.sprite = m_forwardSprites[0];
        m_forwardImage.sprite = m_forwardSprites[0];
        m_forwardVideoPlayer.SetDirectAudioVolume(0, 1f);
        m_switchLockedUntil = Time.time + m_directionSwitchCooldown;
        
        if (!m_wasForward)
        {
            SwitchToForward();
        }
        else
        {
            ForceAudioReset(m_forwardVideoPlayer, 1f);
        }
    }
    
    private void HandleArrowInput()
    {
        bool arrowPressed = false;
        if (m_playBackwardAction.action.WasPerformedThisFrame())
        {
            m_speedLevel--;
            if (m_speedLevel < -3) m_speedLevel = -3;
            arrowPressed = true;
        }
        else if (m_playForwardAction.action.WasPerformedThisFrame())
        {
            m_speedLevel++;
            if (m_speedLevel > 3) m_speedLevel = 3;
            arrowPressed = true;
        }

        if (!arrowPressed) return;
        
        //Debug.Log("Speed Level: " + m_speedLevel);
        //Debug.Log("Playback speed =  " + m_forwardVideoPlayer.playbackSpeed);
        
        switch (m_speedLevel)
        {
            case -3:
                m_backwardVideoPlayer.playbackSpeed = m_speeds[3];
                m_backwardImage.sprite = m_forwardSprites[3];
                break;
            case -2:
                m_backwardVideoPlayer.playbackSpeed = m_speeds[2];
                m_backwardImage.sprite = m_forwardSprites[2];
                break;
            case -1:
                m_forwardVideoPlayer.SetDirectAudioVolume(0, 0f);
                m_backwardVideoPlayer.playbackSpeed = m_speeds[1];
                m_backwardImage.sprite = m_forwardSprites[1];
                if (m_wasForward) SwitchToBackward();
                break;
            case 0:
                SetNormalPlaybackspeed();
                break;
            case 1:
                m_forwardVideoPlayer.SetDirectAudioVolume(0, 0f);
                m_forwardVideoPlayer.playbackSpeed = m_speeds[1];
                m_forwardImage.sprite = m_forwardSprites[1];
                break;
            case 2:
                m_forwardVideoPlayer.playbackSpeed = m_speeds[2];
                m_forwardImage.sprite = m_forwardSprites[2];
                break;
            case 3:
                m_forwardVideoPlayer.playbackSpeed = m_speeds[3];
                m_forwardImage.sprite = m_forwardSprites[3];
                break;
        }
    }

    private void UpdateSlider()
    {
        if (m_isDraggingSlider) return;

        // Do not update the slider if it's being dragged
        if (m_isSeekingSlider)
        {
            if (Mathf.Abs((float)(m_forwardVideoPlayer.time - m_targetSeekTime)) < 0.1f)
            {
                m_isSeekingSlider = false;
            }
            return;
        }

        // Update slider
        if (m_forwardVideoPlayer.length > 0)
        {
            m_videoSlider.value = (float)(m_forwardVideoPlayer.time / m_forwardVideoPlayer.length);
        }
    }
    
    public void VideoClick()
    {
        if (!SaveManager.Instance.Data.settings.videoPlayerControls || GameManager.Instance.GetGameState == GameState.MainMenu) return;
        
        /* DISABLED THIS BEHAVIOR FOR NOW (because I think it's unuseful)
        if (VideoManager.Instance.GetVideoPlayer.isPlaying)
        {
            VideoManager.Instance.Pause();
            m_image.sprite = m_pauseSprite;
        }
        else
        {
            VideoManager.Instance.UnPause();
            m_image.sprite = m_playSprite;
        }*/
        
        ShowControls(true);
    }
    
    private void Update()
    {
        // Update video controls only in play mode (will not be shown otherwise)
        if (GameManager.Instance.GetGameState != GameState.Game && GameManager.Instance.GetGameState != GameState.VideoPlayer) return;
        
        UpdateAutoHide();

        if (!SaveManager.Instance.Data.settings.videoPlayerControls) return;
        
        HandleSpacebarInput();
        Debug.Log(Time.time > m_switchLockedUntil);
        if (Time.time > m_switchLockedUntil) HandleArrowInput();
        UpdateSlider();
    }
}

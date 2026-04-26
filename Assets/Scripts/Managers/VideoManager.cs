using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    private static VideoManager m_instance;
    public static VideoManager Instance => m_instance;

    [SerializeField] private float m_autoHideDuration = 2f;

    [Header("Set in Inspector")]
    [SerializeField] private RenderTexture m_renderTexture;
    [SerializeField] private VideoPlayer m_videoPlayer;
    [SerializeField] private VideoClip m_menuClip;
    [SerializeField] private VideoClip m_startClip;
    [SerializeField] private Slider m_videoSlider;
    
    private bool m_isDragging;
    private bool m_isSeeking;
    private double m_targetSeekTime;
    private float m_autoHideTimer;
    
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
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
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    private void PlayClip(VideoClip clip, bool loop)
    {
        if (clip == null) return;

        m_videoPlayer.Stop();
        m_videoPlayer.clip = clip;
        m_videoPlayer.isLooping = loop;
        m_videoPlayer.Prepare();
        m_videoPlayer.prepareCompleted += OnPrepared;
    }
    
    private void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;
        vp.Play();
        PanelManager.Instance.ShowPanel();
    }

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
    
    public void Pause()
    {
        m_videoPlayer.Pause();
    }
    
    public void Play()
    {
        m_videoPlayer.Play();
    }

    public void StartGame()
    {
        PlayClip(m_startClip, false);
    }

    public void MainMenu()
    {
        PlayClip(m_menuClip, true);
    }
    
    public void OnSliderPointerDown()
    {
        m_isDragging = true;
    }
    
    public void OnSliderPointerUp()
    {
        if (m_videoPlayer.clip == null) return;

        double targetTime = m_videoSlider.value * m_videoPlayer.length;

        m_isDragging = false;
        m_isSeeking = true;
        
        m_targetSeekTime = targetTime;
        m_videoPlayer.time = targetTime;
    }
    
    public void OnSliderValueChanged(float value)
    {
        if (!m_isDragging) return;

        m_videoPlayer.time = value * m_videoPlayer.length;
    }

    private void HandleKeyboardInputs()
    {
        // Spacebar
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (m_videoPlayer.isPaused)
            {
                Play();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Update()
    {
        if (m_videoPlayer.clip == null) return;

        HandleKeyboardInputs();
        
        if (m_isDragging) return;

        if (m_isSeeking)
        {
            if (Mathf.Abs((float)(m_videoPlayer.time - m_targetSeekTime)) < 0.1f)
            {
                m_isSeeking = false;
            }
            return;
        }

        if (m_videoPlayer.length > 0)
        {
            m_videoSlider.value = (float)(m_videoPlayer.time / m_videoPlayer.length);
        }
    }
}

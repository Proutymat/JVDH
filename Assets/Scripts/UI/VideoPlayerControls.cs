using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;


public class VideoPlayerControls : MonoBehaviour
{
    
    [Header("Parameters")]
    [SerializeField] private float m_autoHideDuration;
    
    [Header("Set in Inspector")]
    [SerializeField] private VideoPlayer m_videoPlayer;
    [SerializeField] private CanvasGroup m_controlsCanvasGroup;
    [SerializeField] private Slider m_videoSlider;
    [SerializeField] private TMP_Text m_subtitleText;
    
    private bool m_isDraggingSlider;
    private bool m_isSeekingSlider;
    private double m_targetSeekTime;
    private float m_autoHideTimer;
    private Vector2 m_lastMousePosition;
    private bool m_enableControls;
    private bool m_showControls;

    public bool EnableControls
    {
        get => m_enableControls;
        set => m_enableControls = value;
    }

    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Start()
    {
        m_autoHideTimer = 0;
        m_enableControls = false;
        m_isDraggingSlider = false;
        m_isSeekingSlider = false;
        
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
    
    private void ShowCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Confined;
    }

    public void ShowControls(bool show)
    {
        if (!m_enableControls && show) return;

        m_showControls = show;
        m_controlsCanvasGroup.alpha = show ? 1 : 0;
        m_controlsCanvasGroup.blocksRaycasts = show;
        m_controlsCanvasGroup.interactable = show;
        
        m_subtitleText.alignment = TextAlignmentOptions.Bottom;
    }
    
    
    private void UpdateAutoHide()
    {
        Vector2 currentMousePos = Mouse.current.position.ReadValue();

        // Has cursor moved
        if (currentMousePos != m_lastMousePosition)
        {
            m_lastMousePosition = currentMousePos;
            m_autoHideTimer = 0f;
            
            ShowCursor(true);
            if (m_enableControls)
            {
                ShowControls(true);

            }
        }

        if (m_isDraggingSlider) return;

        m_autoHideTimer += Time.deltaTime;

        if (m_autoHideTimer >= m_autoHideDuration)
        {
            ShowCursor(false);
            if (m_enableControls)
            {
                ShowControls(false);

            }
        }
    }
    
    private void HandleKeyboardInputs()
    {
        // Space-bar (pause)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (m_videoPlayer.isPlaying)
            {
                VideoManager.Instance.UnPause();
            }
            else
            {
                VideoManager.Instance.Pause();
            }
        }
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
    
    private void Update()
    {
        // Update video controls only in play mode (will not be shown otherwise)
        if (GameManager.Instance.GetGameState != GameManager.GameState.Game) return;
        
        UpdateAutoHide();

        if (!m_enableControls) return;
        
        HandleKeyboardInputs();
        UpdateSlider();
    }
}

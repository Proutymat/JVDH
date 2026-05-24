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
    [SerializeField] private InputActionReference m_previousAction;
    [SerializeField] private InputActionReference m_nextAction;
    [SerializeField] private InputActionReference m_pauseVideoAction;
    
    private bool m_isDraggingSlider;
    private bool m_isSeekingSlider;
    private double m_targetSeekTime;
    private float m_nextSeekTimer;
    private float m_previousSeekTimer;
    private float m_autoHideTimer;
    private Vector2 m_lastMousePosition;
    private float m_heldTime;
    private float m_repeatTimer;


    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Start()
    {
        m_autoHideTimer = 0;
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
        if (!SaveManager.Instance.Data.settings.videoPlayerControls && show) return;

        m_controlsCanvasGroup.alpha = show ? 1 : 0;
        m_controlsCanvasGroup.blocksRaycasts = show;
        m_controlsCanvasGroup.interactable = show;
        
        m_subtitleText.alignment = show ? TextAlignmentOptions.Top : TextAlignmentOptions.Bottom;
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
            ShowCursor(false);
            if (SaveManager.Instance.Data.settings.videoPlayerControls)
            {
                ShowControls(false);
            }
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
    
    private void Seek(double delta)
    {
        Debug.Log("Cliqued");

        double newTime = m_videoPlayer.time + delta;
        newTime = Mathf.Clamp((float)newTime, 0f, (float)m_videoPlayer.length);

        m_videoSlider.value = (float)(newTime / m_videoPlayer.length);

        m_autoHideTimer = 0f;
    }
    
    // DOES NOT WORK CORRECTLY
    /*
    private void HandleArrowInput(InputActionReference arrow, float seekStep)
    {
        // Key pressed, move once on video
        if (arrow.action.WasPerformedThisFrame())
        {
            m_isSeekingSlider = true;
            m_heldTime = 0f;
            m_repeatTimer = 0f;
            m_videoPlayer.time += seekStep;
            Seek(seekStep);
            ShowControls(true);
        }
        
        // Key held down, move on video continuously
        if (arrow.action.IsPressed())
        {
            m_isDraggingSlider = true;
            m_heldTime += Time.deltaTime;

            // After delay, move on video at a set rate 
            if (m_heldTime > m_holdInitialDelay)
            {
                m_repeatTimer += Time.deltaTime;

                if (m_repeatTimer >= m_holdRepeatRate)
                {
                    m_repeatTimer = 0f;
                    Seek(seekStep);
                }
            }
        }

        // Key released
        if (arrow.action.WasReleasedThisFrame())
        {
            m_heldTime = 0f;
            m_repeatTimer = 0f;
            m_isDraggingSlider = false;
            m_targetSeekTime = m_videoSlider.value * m_videoPlayer.length;
        }
    }*/

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
        if (GameManager.Instance.GameState != GameState.Game && GameManager.Instance.GameState != GameState.VideoPlayer) return;
        
        UpdateAutoHide();

        if (!SaveManager.Instance.Data.settings.videoPlayerControls) return;
        
        HandleSpacebarInput();
        //HandleArrowInput(m_previousAction, -m_arrowSeekStep);
        //HandleArrowInput(m_nextAction, m_arrowSeekStep);
        UpdateSlider();
    }
}

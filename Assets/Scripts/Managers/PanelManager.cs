using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelManager : MonoBehaviour
{
    private static PanelManager m_instance;
    public static PanelManager Instance => m_instance;

    [Header("Parameters")]
    [SerializeField] private float m_fadeBlackDuration = 1f;
    
    [Header("Set in Inspector")]
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private CanvasGroup m_mainPanel;
    [SerializeField] private CanvasGroup m_settingsPanel;
    [SerializeField] private CanvasGroup m_bonusPanel;
    [SerializeField] private CanvasGroup m_creditsPanel;
    [SerializeField] private CanvasGroup m_startupPanel;
    [SerializeField] private CanvasGroup m_gamePanel;
    [SerializeField] private CanvasGroup m_pausePanel;
    [SerializeField] private VideoPlayerControls m_videoPlayerControls;
    [SerializeField] private CanvasGroup m_fadeImageCanvasGroup;
    [SerializeField] private RectTransform m_creditsContainer;
    [SerializeField] private float m_creditsSpeed;


    private PanelState m_panelState;
    
    public PanelState GetPanelState { get => m_panelState; }
    
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning("Multiple PanelManager instances in scene!");
            Destroy(gameObject);
            return;
        }

        m_instance = this;
    }
    
    public void Start()
    {
        m_panelState = PanelState.Main;
    }
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    private static void ShowCanvasGroup(bool show, CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = show ? 1 : 0;
        canvasGroup.blocksRaycasts = show;
        canvasGroup.interactable = show;
    }

    private void SetPanelsState(PanelState state)
    {
        ShowCanvasGroup(state == PanelState.Main, m_mainPanel);
        ShowCanvasGroup(state == PanelState.Options, m_settingsPanel);
        ShowCanvasGroup(state == PanelState.Bonus, m_bonusPanel);
        ShowCanvasGroup(state == PanelState.Credits, m_creditsPanel);
        ShowCanvasGroup(state == PanelState.Startup, m_startupPanel);
        ShowCanvasGroup(state == PanelState.Game, m_gamePanel);
        ShowCanvasGroup(state == PanelState.Pause, m_pausePanel);

        m_panelState = state;
    }
    
    public void SetPanel(PanelState state, FadeStyle fadeStyle = FadeStyle.None, TweenCallback onMidFade = null, TweenCallback onFadeFinished = null)
    {
        Sequence seq = DOTween.Sequence();
        
        switch (fadeStyle)
        {
            case FadeStyle.None:
                SetPanelsState(state);
                seq.AppendCallback(() => onMidFade?.Invoke());
                seq.OnComplete(() => onFadeFinished?.Invoke());
                break;
            case FadeStyle.Wait:
                seq.AppendCallback(() => m_fadeImageCanvasGroup.alpha = 1f);
                seq.AppendCallback(() => SetPanelsState(state));
                seq.AppendCallback(() => onMidFade?.Invoke());
                seq.AppendInterval(m_fadeBlackDuration);
                seq.AppendCallback(() => m_fadeImageCanvasGroup.alpha = 0f);
                seq.OnComplete(() => onFadeFinished?.Invoke());
                break;
            case FadeStyle.FadeIn:
                seq.Append(m_fadeImageCanvasGroup.DOFade(1f, GameManager.Instance.FadeDuration));
                seq.AppendCallback(() => SetPanelsState(state));
                seq.AppendCallback(() => onMidFade?.Invoke());
                seq.AppendInterval(m_fadeBlackDuration);
                seq.AppendCallback(() => m_fadeImageCanvasGroup.alpha = 0f);
                seq.OnComplete(() => onFadeFinished?.Invoke());
                break;
            case FadeStyle.FadeOut:
                seq = DOTween.Sequence();
                seq.AppendCallback(() => m_fadeImageCanvasGroup.alpha = 1f);
                seq.AppendCallback(() => SetPanelsState(state));
                seq.AppendCallback(() => onMidFade?.Invoke());
                seq.AppendInterval(m_fadeBlackDuration);
                seq.Append(m_fadeImageCanvasGroup.DOFade(0f, GameManager.Instance.FadeDuration));
                seq.OnComplete(() => onFadeFinished?.Invoke());
                break;
            case FadeStyle.FadeInAndOut:
                seq = DOTween.Sequence();
                seq.Append(m_fadeImageCanvasGroup.DOFade(1f, GameManager.Instance.FadeDuration));
                seq.AppendCallback(() => SetPanelsState(state));
                seq.AppendCallback(() => onMidFade?.Invoke());
                seq.AppendInterval(m_fadeBlackDuration);
                seq.Append(m_fadeImageCanvasGroup.DOFade(0f, GameManager.Instance.FadeDuration));
                seq.OnComplete(() => onFadeFinished?.Invoke());
                break;
        }
    }

    public void ShowOptionsMenu()
    {
        SetPanel(PanelState.Options);
    }

    public void ShowBonusMenu()
    {
        SetPanel(PanelState.Bonus);
    }

    public void ShowCreditsMenu()
    {
        float canvasHalfHeight = ((RectTransform)m_canvas.transform).rect.height * 0.5f;
        float containerHalfHeight = m_creditsContainer.rect.height * 0.5f;

        // Centre du container positionné juste sous le bord bas du canvas
        m_creditsContainer.anchoredPosition = new Vector2(
            m_creditsContainer.anchoredPosition.x,
            -canvasHalfHeight - containerHalfHeight
        );
        
        SetPanel(PanelState.Credits);
    }

    private float GetCreditsHalfHeight()
    {
        return m_creditsContainer != null ? m_creditsContainer.rect.height * 0.5f : 0f;
    }
    
    private void Update()
    {
        // Credits roll
        if (m_panelState == PanelState.Credits)
        {
            float speed = Mouse.current.leftButton.isPressed ? m_creditsSpeed * 30 : m_creditsSpeed;
            m_creditsContainer.transform.position += Time.deltaTime * speed * Vector3.up;
            
            // Back to main menu if ended
            float topEdge = m_creditsContainer.transform.position.y - GetCreditsHalfHeight();
            if (topEdge > Screen.height + 50)
            {
                GameManager.Instance.LoadMainMenu(false, FadeStyle.None);
            }
        }
    }
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelManager : MonoBehaviour
{
    public enum PanelState
    {
        Main,
        Options,
        Bonus,
        Credits,
        Pause,
        Game
    }
    
    private static PanelManager m_instance;
    public static PanelManager Instance => m_instance;

    [Header("Parameters")] [SerializeField]
    private float m_fadeBlackDuration = 1f;
    
    [Header("Set in Inspector")]
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private GameObject m_mainPanel;
    [SerializeField] private GameObject m_settingsPanel;
    [SerializeField] private GameObject m_bonusPanel;
    [SerializeField] private CanvasGroup m_creditsPanel;
    [SerializeField] private GameObject m_gamePanel;
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

    public void Initialize()
    {
        m_mainPanel.SetActive(true);
        m_settingsPanel.SetActive(false);
        m_creditsPanel.alpha = 0;
        m_bonusPanel.SetActive(false);
        m_gamePanel.SetActive(false);
        m_pausePanel.alpha = 0;
        m_pausePanel.blocksRaycasts = false;
        m_fadeImageCanvasGroup.alpha = 1;
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void HideBlackScreen()
    {
        m_fadeImageCanvasGroup.alpha = 0;
    }
    
    
    // I SWEAR ITS THE WORST FUNCTION I EVER MADE, ITS NONSENSE BOLLOCKS
    public void SetPanel(PanelState state, bool doFade)
    {
        if (doFade)
        {
            Sequence anim = DOTween.Sequence();

            anim.Append(m_fadeImageCanvasGroup.DOFade(1f, GameManager.Instance.FadeDuration)); // Fade in

            anim.AppendInterval(m_fadeBlackDuration);

            // Switch panel
            anim.AppendCallback(() =>
            {
                VideoManager.Instance.Stop();

                m_mainPanel.SetActive(state == PanelState.Main);
                m_settingsPanel.SetActive(state == PanelState.Options);
                m_bonusPanel.SetActive(state == PanelState.Bonus);
                m_creditsPanel.alpha = state == PanelState.Credits ? 1 : 0;
                m_gamePanel.SetActive(state == PanelState.Game);
                
                
                if (state == PanelState.Main)
                {
                    VideoManager.Instance.PlayMainMenuClip();
                }
            });

            anim.OnComplete(() =>
            {
                if (state == PanelState.Game)
                {
                    m_videoPlayerControls.ShowControls(false);
                    VideoTreePlayer.instance.StartVideoTree();
                }
            });
        }
        else
        {
            m_mainPanel.SetActive(state == PanelState.Main);
            m_settingsPanel.SetActive(state == PanelState.Options);
            m_bonusPanel.SetActive(state == PanelState.Bonus);
            m_creditsPanel.alpha = state == PanelState.Credits ? 1 : 0;
            m_gamePanel.SetActive(state == PanelState.Game);
        }
        
        m_panelState = state;
    }

    public void TogglePauseMenu(bool pause)
    {
        m_pausePanel.alpha = pause ? 1 : 0;
        m_pausePanel.blocksRaycasts = pause;

        if (pause)
        {
            m_videoPlayerControls.ShowControls(false);
        }
    }
    
    public void ShowMainMenu()
    {
        SetPanel(PanelState.Main, false);
        SoundManager.Instance.PlayMenuMusic();
    }

    public void ShowOptionsMenu()
    {
        SetPanel(PanelState.Options, false);
    }

    public void ShowBonusMenu()
    {
        SetPanel(PanelState.Bonus, false);
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
        
        SetPanel(PanelState.Credits, false);
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
                GameManager.Instance.LoadMainMenu(false);
            }
        }
    }
}

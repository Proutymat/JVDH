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
    [SerializeField] private GameObject m_optionsPanel;
    [SerializeField] private GameObject m_bonusPanel;
    [SerializeField] private CanvasGroup m_creditsPanel;
    [SerializeField] private GameObject m_gamePanel;
    [SerializeField] private GameObject m_pausePanel;
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
        m_optionsPanel.SetActive(false);
        m_bonusPanel.SetActive(false);
        m_gamePanel.SetActive(false);
        m_pausePanel.SetActive(false);
        m_fadeImageCanvasGroup.alpha = 1;
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void HideBlackScreen()
    {
        m_fadeImageCanvasGroup.alpha = 0;
    }
    
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
                m_optionsPanel.SetActive(state == PanelState.Options);
                m_bonusPanel.SetActive(state == PanelState.Bonus);
                m_creditsPanel.alpha = state == PanelState.Credits ? 1 : 0;
                m_gamePanel.SetActive(state == PanelState.Game);
                m_pausePanel.SetActive(state == PanelState.Pause);
                
                
                if (state == PanelState.Main)
                {
                    VideoManager.Instance.StartMainMenuClip();
                }
            });

            anim.OnComplete(() =>
            {
                if (state == PanelState.Game)
                {
                    VideoManager.Instance.StartGame();
                }
            });
        }
        else
        {
            m_mainPanel.SetActive(state == PanelState.Main);
            m_optionsPanel.SetActive(state == PanelState.Options);
            m_bonusPanel.SetActive(state == PanelState.Bonus);
            m_creditsPanel.alpha = state == PanelState.Credits ? 1 : 0;
            m_gamePanel.SetActive(state == PanelState.Game);
            m_pausePanel.SetActive(state == PanelState.Pause);
        }
        
        m_panelState = state;
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
        SetPanel(PanelState.Credits, false);
        
        /*
        // Set credits position
        Vector3 newPos = m_creditsContainer.transform.position;
        newPos.y = m_creditsPosition;
        m_creditsContainer.transform.position = newPos;*/
        
        float canvasHalfHeight = ((RectTransform)m_canvas.transform).rect.height * 0.5f;
        float containerHalfHeight = m_creditsContainer.rect.height * 0.5f;

        // Centre du container positionné juste sous le bord bas du canvas
        m_creditsContainer.anchoredPosition = new Vector2(
            m_creditsContainer.anchoredPosition.x,
            -canvasHalfHeight - containerHalfHeight
        );
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
                GameManager.Instance.LoadMainMenu();
            }
        }
    }
}

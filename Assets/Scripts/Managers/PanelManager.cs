using DG.Tweening;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public enum PanelState
    {
        Main,
        Pause,
        Game
    }
    
    private static PanelManager m_instance;
    public static PanelManager Instance => m_instance;

    [Header("Parameters")] [SerializeField]
    private float m_fadeBlackDuration = 1f;
    
    [Header("Set in Inspector")]
    [SerializeField] private GameObject m_mainPanel;
    [SerializeField] private GameObject m_gamePanel;
    [SerializeField] private GameObject m_pausePanel;
    [SerializeField] private CanvasGroup m_fadeImageCanvasGroup;

    private PanelState m_panelState;
    
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
        m_gamePanel.SetActive(false);
        m_pausePanel.SetActive(false);
        m_fadeImageCanvasGroup.alpha = 1;
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void SetPanel(PanelState state)
    {
        Sequence anim = DOTween.Sequence();
        
        anim.Append(m_fadeImageCanvasGroup.DOFade(1f, GameManager.Instance.FadeDuration)); // Fade in
        
        anim.AppendInterval(m_fadeBlackDuration);
        
        // Switch panel
        anim.AppendCallback(() =>
        {
            VideoManager.Instance.Stop();
            
            m_mainPanel.SetActive(state == PanelState.Main);
            m_gamePanel.SetActive(state == PanelState.Game);
            m_pausePanel.SetActive(state == PanelState.Pause);

            m_panelState = state;

            if (state == PanelState.Main)
            {
                VideoManager.Instance.MainMenu();
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

    public void ShowPanel()
    {
        m_fadeImageCanvasGroup.alpha = 0;
    }
}

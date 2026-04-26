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
    
    [SerializeField] private GameObject m_mainPanel;
    [SerializeField] private GameObject m_gamePanel;
    [SerializeField] private GameObject m_pausePanel;

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
        SetPanel(PanelState.Main);
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void SetPanel(PanelState state)
    {
        m_mainPanel.SetActive(state == PanelState.Main);
        m_gamePanel.SetActive(state == PanelState.Game);
        m_pausePanel.SetActive(state == PanelState.Pause);
        
        m_panelState = state;
    }
}

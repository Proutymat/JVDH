using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance => m_instance;
    
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning("Multiple GameManager instances in scene!");
            Destroy(gameObject);
            return;
        }

        m_instance = this;
    }

    private void Start()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        PanelManager.Instance.Initialize();
        SoundManager.Instance.Initialize();
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void StartGame()
    {
        PanelManager.Instance.SetPanel(PanelManager.PanelState.Game);
        SoundManager.Instance.StopMusic();
    }

    public void GoToMainMenu()
    {
        PanelManager.Instance.SetPanel(PanelManager.PanelState.Main);
        SoundManager.Instance.PlayMenuMusic();
    }
    
}

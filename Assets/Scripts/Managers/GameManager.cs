using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance => m_instance;
    
    [Header("Parameters")]
    [SerializeField] private float m_fadeDuration = 0.5f;

    public float FadeDuration { get => m_fadeDuration; }
    
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
    
    private void Initialize()
    {
        SettingsManager.Instance.Initialize();
        VideoManager.Instance.Initialize();
        PanelManager.Instance.Initialize();
        SoundManager.Instance.Initialize();
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void StartGame()
    {
        PanelManager.Instance.SetPanel(PanelManager.PanelState.Game, true);
        SoundManager.Instance.StopMusic();
    }

    public void LoadMainMenu(bool loadBackVideo)
    {
        PanelManager.Instance.ShowMainMenu();
        SoundManager.Instance.PlayMenuMusic();
        if (loadBackVideo) VideoManager.Instance.PlayMainMenuClip();
    }
    

    public void LoadOptionsMenu()
    {
        PanelManager.Instance.ShowOptionsMenu();
    }

    public void LoadBonusMenu()
    {
        
    }

    public void LoadCreditsMenu()
    {
        PanelManager.Instance.ShowCreditsMenu();
        SoundManager.Instance.PlayCreditMusic();
    }
    
}

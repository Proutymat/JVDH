using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance => m_instance;
    
    [Header("Parameters")]
    [SerializeField] private float m_fadeDuration = 0.5f;

    private bool m_isPaused;
    private GameState m_gameState;

    public GameState GetGameState { get => m_gameState; }

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

    private void StartMainMenuEvents()
    {
        SoundManager.Instance.PlayMenuMusic(false);
        VideoManager.Instance.UnPause();
    }
    
    private void Start()
    {
        PanelManager.Instance.SetPanel(PanelState.Main, FadeStyle.Wait, VideoManager.Instance.PlayMainMenuClip, null, StartMainMenuEvents);
        
        m_isPaused = false;
        m_gameState = GameState.MainMenu;
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------
    
    
    public void StartGame()
    {
        PanelManager.Instance.SetPanel(PanelState.Game, FadeStyle.FadeIn, VideoTreePlayer.instance.StartVideoTree, null, VideoManager.Instance.UnPause);
        m_gameState = GameState.Game;
    }

    private void BackToMainMenuEvents()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SoundManager.Instance.PlayMenuMusic(true);
        VideoManager.Instance.UnPause();
    }

    public void BackToMainMenu()
    {
        VideoManager.Instance.PlayMainMenuClip();
        PanelManager.Instance.SetPanel(PanelState.Main, FadeStyle.FadeInAndOut, null, BackToMainMenuEvents);
        m_gameState = GameState.MainMenu;
    }
    
    public void LoadMainMenu()
    {
        PanelManager.Instance.SetPanel(PanelState.Main);
        m_gameState = GameState.MainMenu;
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

    public void PauseGame()
    {
        m_isPaused = !m_isPaused;

        if (m_isPaused)
        {
            VideoManager.Instance.Pause();
            m_gameState = GameState.Paused;
            PanelManager.Instance.SetPanel(PanelState.Pause);
        }
        else
        {
            VideoManager.Instance.UnPause();
            m_gameState = GameState.Game;
            PanelManager.Instance.SetPanel(PanelState.Game);
        }
    }

    private void Update()
    {
        // Pause menu
        if ((m_gameState == GameState.Game || m_gameState == GameState.Paused) && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseGame();
        }
    }
    
}

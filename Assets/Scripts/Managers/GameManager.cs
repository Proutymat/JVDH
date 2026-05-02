using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Game,
        Paused
    }
    
    
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
            Debug.LogWarning("Multiple GameManager insta²nces in scene!");
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
        
        m_isPaused = false;
        m_gameState = GameState.MainMenu;
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void StartGame()
    {
        PanelManager.Instance.SetPanel(PanelManager.PanelState.Game, true);
        SoundManager.Instance.StopMusic();
        m_gameState = GameState.Game;
        
    }

    public void LoadMainMenu(bool loadBackVideo)
    {
        PanelManager.Instance.ShowMainMenu();
        SoundManager.Instance.PlayMenuMusic();
        if (loadBackVideo) VideoManager.Instance.PlayMainMenuClip();
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

    private void PauseGame()
    {
        m_isPaused = !m_isPaused;
        PanelManager.Instance.TogglePauseMenu(m_isPaused);

        if (m_isPaused)
        {
            VideoManager.Instance.Pause();
            m_gameState = GameState.Paused;
        }
        else
        {
            VideoManager.Instance.Play();
            m_gameState = GameState.Game;
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

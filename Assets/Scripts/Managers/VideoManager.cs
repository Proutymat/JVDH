using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    private static VideoManager m_instance;
    public static VideoManager Instance => m_instance;

    [Header("Set in Inspector")] [SerializeField]
    private RenderTexture m_renderTexture;
    [SerializeField] private VideoPlayer m_videoPlayer;
    [SerializeField] private VideoClip m_menuClip;
    [SerializeField] private VideoClip m_startClip;
    
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning("Multiple VideoManager instances in scene!");
            Destroy(gameObject);
            return;
        }

        m_instance = this;
    }

    public void Initialize()
    {
        PlayClip(m_menuClip, true);
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    private void PlayClip(VideoClip clip, bool loop)
    {
        if (clip == null) return;

        m_videoPlayer.Stop();
        m_videoPlayer.clip = clip;
        m_videoPlayer.isLooping = loop;
        m_videoPlayer.Play();
    }

    public void Stop()
    {
        m_videoPlayer.Stop();
        m_videoPlayer.clip = null;

        if (m_renderTexture != null)
        {
            RenderTexture activeRT = RenderTexture.active;
            RenderTexture.active = m_renderTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = activeRT;
        }
    }
    
    public void Pause()
    {
        m_videoPlayer.Pause();
    }
    
    public void Play()
    {
        m_videoPlayer.Play();
    }

    public void StartGame()
    {
        PlayClip(m_startClip, false);
    }

    public void MainMenu()
    {
        PlayClip(m_menuClip, true);
    }
}

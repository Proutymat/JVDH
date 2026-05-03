using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    private static VideoManager m_instance;
    public static VideoManager Instance => m_instance;

    [Header("Set in Inspector")]
    [SerializeField] private RenderTexture m_renderTexture;
    [SerializeField] private VideoPlayer m_videoPlayer;
    [SerializeField] private VideoClip m_menuClip;
    

    public VideoPlayer GetVideoPlayer { get => m_videoPlayer; }
    
    
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
    //                CORE FUNCTIONS
    // --------------------------------------------

    /*
     * Used to play a clip video with possibility to loop
     */
    public void PlayClip(VideoClip clip, bool loop = false)
    {
        if (clip == null) return;

        SubtitleManager.Instance.SetSRTFile();
        
        m_videoPlayer.Stop();
        m_videoPlayer.clip = clip;
        m_videoPlayer.isLooping = loop;
        m_videoPlayer.Prepare();
        m_videoPlayer.prepareCompleted += OnPrepared;
    }
    
    /*
     * Stop Video 
     */
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
    
    /*
     * Pause Video
     */
    public void Pause() => m_videoPlayer.Pause();
    
    /*
     * Unpause Video
     */
    public void UnPause() => m_videoPlayer.Play();
    
    // --------------------------------------------
    //              FUNCTIONS HELPERS
    // --------------------------------------------
    
    /*
     * Buffer to load videos
     */
    private void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;
        vp.Play();
        PanelManager.Instance.HideBlackScreen();
    }

    public void PlayMainMenuClip()
    {
        PlayClip(m_menuClip, true);
    }
}

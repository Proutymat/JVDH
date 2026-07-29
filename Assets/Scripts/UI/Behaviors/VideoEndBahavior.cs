using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

public class VideoEndBehavior : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField] private float m_fadeDuration = 2f;
    
    [Title("Set in Inspector")]
    [SerializeField] private VideoPlayer m_forwardVideoPlayer;
    [SerializeField] private VideoPlayer m_backwardVideoPlayer;
    [SerializeField] private Material m_videoMaterial;
    [SerializeField] private SoundManager m_soundManager;
    [SerializeField] private AudioClip m_videoEndClip;
    [SerializeField] private VideoPlayerControls m_videoPlayerControls;

    public bool IsForward { set => m_isForward = value; }
    public bool IsEnded { get => m_videoEnded; }
    
    private bool m_enable;
    private bool m_playingMusic;
    private bool m_isForward;
    private bool m_videoEnded = false;
    

    private void Start()
    {
        m_enable = false;
        m_playingMusic = false;
        m_videoMaterial.SetFloat("_Strength", 0f);
        m_isForward = true;
    }
    
    private void OnVideoFinished(VideoPlayer vp)
    {
        m_videoEnded = true;
        m_videoPlayerControls.StopForwardSpeed();
    }
    
    public void ResetEndedState()
    {
        m_videoEnded = false;
    }

    public void Enable(bool enable)
    {
        m_enable = enable;

        if (m_enable)
        {
            m_forwardVideoPlayer.loopPointReached += OnVideoFinished;
            m_backwardVideoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            m_forwardVideoPlayer.loopPointReached -= OnVideoFinished;
            m_backwardVideoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
    
    public void Update()
    {
        Debug.Log(m_videoEnded);
        if (!m_enable) return;
        
        double timeLeft;
        if (m_isForward)
        {
            timeLeft = m_forwardVideoPlayer.clip.length - m_forwardVideoPlayer.time;
        }
        else
        {
            timeLeft = m_backwardVideoPlayer.time;
        }
        
        // End reached
        if (timeLeft <= m_fadeDuration)
        {
            float strength = 1f - (float)(timeLeft / m_fadeDuration);
            m_videoMaterial.SetFloat("_Strength", Mathf.Clamp01(strength));

            if (!m_playingMusic)
            {
                m_soundManager.PlayMusic(m_videoEndClip, true, true);
                m_playingMusic = true;
            }
        }
        else
        {
            m_videoMaterial.SetFloat("_Strength", 0f);

            if (m_playingMusic)
            {
                m_soundManager.StopMusic();
                m_playingMusic = false;
            }
        }
    }
    
}
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

public class VideoEndBehavior : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField] private float m_fadeDuration = 2f;
    
    [Title("Set in Inspector")]
    [SerializeField] private VideoPlayer m_videoPlayer;
    [SerializeField] private Material m_videoMaterial;
    [SerializeField] private SoundManager m_soundManager;
    [SerializeField] private AudioClip m_videoEndClip;

    private bool m_enable;
    private bool m_playingMusic;

    private void Start()
    {
        m_enable = false;
        m_playingMusic = false;
        m_videoMaterial.SetFloat("_Strength", 0f);
    }
    
    private void OnVideoFinished(VideoPlayer vp)
    {
        // Other behavior at the very end ?
    }

    public void Enable(bool enable)
    {
        m_enable = enable;
        
        if (m_enable)
            m_videoPlayer.loopPointReached += OnVideoFinished;
        else
            m_videoPlayer.loopPointReached -= OnVideoFinished;
    }
    
    void Update()
    {
        if (!m_enable) return;
        
        double timeLeft = m_videoPlayer.clip.length - m_videoPlayer.time;
        
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
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

public class VideoEndBehavior : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField] private float m_fadeDuration = 2f;
    
    [Title("Set in Inspector")]
    [SerializeField] private VideoManager m_videoManager;
    [SerializeField] private VideoPlayer m_videoPlayer;
    [SerializeField] private Material m_videoMaterial;
    [SerializeField] private SoundManager m_soundManager;
    [SerializeField] private AudioClip m_videoEndClip;

    private bool m_enable;
    public bool Enable { get => m_enable; set => m_enable = value; } 

    private void Start()
    {
        m_enable = true;
        m_videoMaterial.SetFloat("_Strength", 0f);
    }

    
    private void HandleBlackAndWhite()
    {
        double timeLeft = m_videoPlayer.clip.length - m_videoPlayer.time;
        
        if (timeLeft <= m_fadeDuration)
        {
            float strength = 1f - (float)(timeLeft / m_fadeDuration);

            m_videoMaterial.SetFloat("_Strength", Mathf.Clamp01(strength));
            m_soundManager.PlayMusic(m_videoEndClip, true, true);
        }
        else
        {
            m_videoMaterial.SetFloat("_Strength", 0f);
        }
    }
    
    
    void Update()
    {
        if (!m_enable) return;
        
        HandleBlackAndWhite();
    }
}
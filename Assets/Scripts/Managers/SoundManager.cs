using DG.Tweening;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager m_instance;
    public static SoundManager Instance => m_instance;
    
    [Header("Set in Inspector")]
    [SerializeField] private AudioSource m_musicSource;
    [SerializeField] private AudioClip m_menuMusic;
    
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning("Multiple SoundManager instances in scene!");
            Destroy(gameObject);
            return;
        }

        m_instance = this;
    }

    public void Initialize()
    {
        PlayMenuMusic();
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------
    
    public void PlayMenuMusic()
    {
        PlayMusic(m_menuMusic);
    }
    
    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        // Ignore if same music
        if (m_musicSource.clip == clip && m_musicSource.isPlaying)
            return;

        // Fade out then fade in
        m_musicSource.DOKill();

        m_musicSource.DOFade(0f, GameManager.Instance.FadeDuration).OnComplete(() =>
        {
            m_musicSource.clip = clip;
            m_musicSource.loop = true;
            m_musicSource.Play();
            
            m_musicSource.DOFade(1f, GameManager.Instance.FadeDuration);
        });
    }
    
    public void StopMusic()
    {
        m_musicSource.DOKill();

        m_musicSource.DOFade(0f, GameManager.Instance.FadeDuration).OnComplete(() =>
        {
            m_musicSource.Stop();
            m_musicSource.clip = null;
        });
    }
    
}

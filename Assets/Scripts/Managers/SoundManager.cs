using DG.Tweening;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager m_instance;
    public static SoundManager Instance => m_instance;
    
    [Header("Set in Inspector")]
    [SerializeField] private AudioSource m_musicSource;
    [SerializeField] private AudioSource m_clicSource;
    [SerializeField] private AudioClip m_menuMusic;
    [SerializeField] private AudioClip m_creditMusic;
    
    
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

    public void Start()
    {
        // Something yet to come
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------
    
    public void PlayClicSound()
    {
        m_clicSource.Play();
    }
    
    public void PlayMenuMusic(bool doFade)
    {
        Debug.Log("Playing Menu Music");
        PlayMusic(m_menuMusic, true, doFade);
    }

    public void PlayCreditMusic()
    {
        PlayMusic(m_creditMusic, false, true);
    }
    
    private void PlayMusic(AudioClip clip, bool loop, bool doFade)
    {
        if (clip == null) return;

        // Ignore if same music
        if (m_musicSource.clip == clip && m_musicSource.isPlaying)
            return;
        
        if (doFade)
        {
            // Fade out then fade in
            m_musicSource.DOFade(0f, GameManager.Instance.FadeDuration).OnComplete(() =>
            {
                m_musicSource.DOKill();
                m_musicSource.clip = clip;
                m_musicSource.loop = loop;
                m_musicSource.Play();
            
                m_musicSource.DOFade(1f, GameManager.Instance.FadeDuration);
            });
        }
        else
        {
            m_musicSource.DOKill();
            m_musicSource.clip = clip;
            m_musicSource.loop = loop;
            m_musicSource.volume = 1;
            m_musicSource.Play();
        }
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

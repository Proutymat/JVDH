using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

public class VideoBWEffect : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField] private float m_fadeDuration = 2f;
    
    [Title("Set in Inspector")]
    [SerializeField] private VideoPlayer m_videoPlayer;
    [SerializeField] private Material m_videoMaterial;

    private float timer = 0f;

    void Update()
    {
        double timeLeft = m_videoPlayer.clip.length - m_videoPlayer.time;
        Debug.Log(timeLeft);
        
        if (timeLeft <= m_fadeDuration)
        {
            float strength = 1f - (float)(timeLeft / m_fadeDuration);

            m_videoMaterial.SetFloat("_Strength", Mathf.Clamp01(strength));
        }
        else
        {
            m_videoMaterial.SetFloat("_Strength", 0f);
        }
    }
}
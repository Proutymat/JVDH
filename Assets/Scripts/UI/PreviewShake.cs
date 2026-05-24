using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class PreviewShake : MonoBehaviour
{
    [Title("Set in inspector")]
    [SerializeField] private RectTransform m_rectTransform;
    [SerializeField] private AudioSource m_audioSource;
    
    public float duration = 0.2f;
    public float strength = 10f;
    private Vector2 originalPos;
    private bool isShaking;
    
    public void OnLockedClick()
    {
        m_audioSource.Play();
        if (!isShaking)
            StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        isShaking = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * strength;

            m_rectTransform.anchoredPosition =
                originalPos + new Vector2(x, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        m_rectTransform.anchoredPosition = originalPos;
        isShaking = false;
    }
}

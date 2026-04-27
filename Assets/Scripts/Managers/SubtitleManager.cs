using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    private static SubtitleManager m_instance;
    public static SubtitleManager Instance => m_instance;
    
    [SerializeField] private TextAsset m_srtFile;
    [SerializeField] private TextMeshProUGUI m_subtitleText;
    
    private List<SubtitleEntry> m_subtitles;
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning("Multiple SubtitleManager instances in scene!");
            Destroy(gameObject);
            return;
        }

        m_instance = this;
    }

    public void Initialize()
    {
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void SetSRTFile()
    {
        m_subtitles = SRTParser.Parse(m_srtFile.text);
    }
    
    private void Update()
    {
        double currentTime = VideoManager.Instance.GetVideoPlayer.time;

        foreach (var sub in m_subtitles)
        {
            if (currentTime >= sub.startTime && currentTime <= sub.endTime)
            {
                m_subtitleText.text = sub.text;
                return;
            }
        }

        m_subtitleText.text = "";
    }
    
}

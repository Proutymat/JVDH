using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    private static SettingsManager m_instance;
    public static SettingsManager Instance => m_instance;
    
    [Header("Set in Inspector")]
    [SerializeField] private Slider m_globalVolumeSlider;
    [SerializeField] private TMP_Text m_subtitlesText;
    [SerializeField] private TMP_Text m_languageText;
    [SerializeField] private TMP_Text m_windowedModeText;
    
    [SerializeField] private LocalizedString  m_enabledTextKey;
    [SerializeField] private LocalizedString  m_disabledTextKey;
    [SerializeField] private LocalizedString  m_subtitlesTextKey;
    [SerializeField] private LocalizedString  m_windowedModeTextKey;
    
    private float m_globalVolume;
    private bool m_subtitles;
    private int m_localID;
    private bool m_windowedMode;

    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning("Multiple SettingsManager instances in scene!");
            Destroy(gameObject);
            return;
        }

        m_instance = this;
    }

    public void Initialize()
    {
        m_globalVolume =  m_globalVolumeSlider.value;
        m_subtitles = false;
        m_localID = 0;
        m_windowedMode = true;
        SetWindowedModeButtonsText();
        SetSubtitlesButtonsText();
    }

    private void SetSubtitlesButtonsText()
    {
        m_subtitlesText.text = m_subtitlesTextKey.GetLocalizedString();
        m_subtitlesText.text += m_subtitles ? m_enabledTextKey.GetLocalizedString() : m_disabledTextKey.GetLocalizedString();
    }

    public void SubtitlesButton()
    {
        m_subtitles = !m_subtitles;
        SetSubtitlesButtonsText();
    }

    private void SetWindowedModeButtonsText()
    {
        m_windowedModeText.text = m_windowedModeTextKey.GetLocalizedString();
        m_windowedModeText.text += m_windowedMode ? m_enabledTextKey.GetLocalizedString() : m_disabledTextKey.GetLocalizedString();
    }

    public void WindowedModeButton()
    {
        m_windowedMode = !m_windowedMode;
        SetWindowedModeButtonsText();
        Screen.fullScreen = m_windowedMode;
    }

    public void LanguageButton()
    {
        m_localID = (m_localID + 1) % LocalizationSettings.AvailableLocales.Locales.Count;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[m_localID];
        SetWindowedModeButtonsText();
        SetSubtitlesButtonsText();
    }
}

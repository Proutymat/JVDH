using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    private static SettingsManager m_instance;
    public static SettingsManager Instance => m_instance;
    
    [Title("Set in Inspector")]
    [Title("Main menu settings", horizontalLine: false)]
    [SerializeField] private Slider m_masterVolumeSlider;
    [SerializeField] private TMP_Text m_subtitlesText;
    [SerializeField] private TMP_Text m_languageText;
    [SerializeField] private TMP_Text m_windowedModeText;
    [SerializeField] private TMP_Text m_videoPlayerControlsText;
    [SerializeField] private VideoPlayerControls m_videoPlayerControlsScript;
    [Title("Pause menu settings", horizontalLine: false)]
    [SerializeField] private Slider m_masterVolumeSliderInGame;
    [SerializeField] private TMP_Text m_subtitlesTextInGame;
    [SerializeField] private TMP_Text m_languageTextInGame;
    [SerializeField] private TMP_Text m_windowedModeTextInGame;
    [SerializeField] private TMP_Text m_videoPlayerControlsTextInGame;
    [Title("Localization keys", horizontalLine: false)]
    [SerializeField] private LocalizedString  m_enabledTextKey;
    [SerializeField] private LocalizedString  m_disabledTextKey;
    [SerializeField] private LocalizedString  m_subtitlesTextKey;
    [SerializeField] private LocalizedString  m_windowedModeTextKey;
    [SerializeField] private LocalizedString  m_videoPlayerControlsTextKey;
    
    private float m_masterVolume;
    private bool m_windowedMode;
    private bool m_subtitles;
    private int m_localID;
    private bool m_videoPlayerControls;

    
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

    public void Start()
    {
        // SET VALUES FROM SAVE
        m_masterVolume =  SaveManager.Instance.Data.settings.masterVolume;
        SetMasterVolumeSlider();
        m_windowedMode = SaveManager.Instance.Data.settings.windowed;
        SetWindowedModeButtonsText();
        m_subtitles = SaveManager.Instance.Data.settings.subtitles;
        SetSubtitlesButtonsText();
        m_localID = SaveManager.Instance.Data.settings.language;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[m_localID];
        m_videoPlayerControls = SaveManager.Instance.Data.settings.videoPlayerControls;
        SetVPCButtonsText();
    }

    private void SetMasterVolumeSlider()
    {
        m_masterVolumeSlider.value = m_masterVolume;
        m_masterVolumeSliderInGame.value = m_masterVolume;
    }
    
    private void SetWindowedModeButtonsText()
    {
        // Main menu
        m_windowedModeText.text = m_windowedModeTextKey.GetLocalizedString();
        m_windowedModeText.text += m_windowedMode ? m_enabledTextKey.GetLocalizedString() : m_disabledTextKey.GetLocalizedString();
        
        // Pause menu
        m_windowedModeTextInGame.text = m_windowedModeTextKey.GetLocalizedString();
        m_windowedModeTextInGame.text += m_windowedMode ? m_enabledTextKey.GetLocalizedString() : m_disabledTextKey.GetLocalizedString();
    }

    private void SetSubtitlesButtonsText()
    {
        // Main menu
        m_subtitlesText.text = m_subtitlesTextKey.GetLocalizedString();
        m_subtitlesText.text += m_subtitles ? m_enabledTextKey.GetLocalizedString() : m_disabledTextKey.GetLocalizedString();
        
        // Pause menu
        m_subtitlesTextInGame.text = m_subtitlesTextKey.GetLocalizedString();
        m_subtitlesTextInGame.text += m_subtitles ? m_enabledTextKey.GetLocalizedString() : m_disabledTextKey.GetLocalizedString();
    }

    private void SetVPCButtonsText()
    {
        // Main menu
        m_videoPlayerControlsText.text = m_videoPlayerControlsTextKey.GetLocalizedString();
        m_videoPlayerControlsText.text += m_videoPlayerControls ? m_enabledTextKey.GetLocalizedString() : m_disabledTextKey.GetLocalizedString();
        
        // Pause menu
        m_videoPlayerControlsTextInGame.text = m_videoPlayerControlsTextKey.GetLocalizedString();
        m_videoPlayerControlsTextInGame.text += m_videoPlayerControls ? m_enabledTextKey.GetLocalizedString() : m_disabledTextKey.GetLocalizedString();
    }

    public void VideoPlayerControlsButton()
    {
        m_videoPlayerControls = !m_videoPlayerControls;
        SetVPCButtonsText();
        m_videoPlayerControlsScript.EnableControls = m_videoPlayerControls;
    }

    public void SubtitlesButton()
    {
        m_subtitles = !m_subtitles;
        SetSubtitlesButtonsText();
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
        SetVPCButtonsText();
    }

    public void DeleteDatasButton()
    {
        PanelManager.Instance.SetPanel(PanelState.Confirm, FadeStyle.FadeInAndOut, null, null, null, 0);
    }
}

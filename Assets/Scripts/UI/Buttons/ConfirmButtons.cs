using UnityEngine;

public class ConfirmButtons : MonoBehaviour
{
    public void YesButton()
    {
        SettingsManager.Instance.ConfirmDeleteDatasButton();
        PanelManager.Instance.SetPanel(PanelState.Settings, FadeStyle.FadeInAndOut);
    }

    public void NoButton()
    {
        PanelManager.Instance.SetPanel(PanelState.Settings, FadeStyle.FadeInAndOut);
    }
}

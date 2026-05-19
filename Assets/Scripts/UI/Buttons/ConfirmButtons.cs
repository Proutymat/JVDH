using UnityEngine;

public class ConfirmButtons : MonoBehaviour
{
    public void YesButton()
    {
        SettingsManager.Instance.ConfirmDeleteDatasButton();
        PanelManager.Instance.SetPanel(PanelState.Settings, FadeStyle.FadeInAndOut, null, null, null, 0);
    }

    public void NoButton()
    {
        PanelManager.Instance.SetPanel(PanelState.Settings, FadeStyle.FadeInAndOut, null, null, null, 0);
    }
}

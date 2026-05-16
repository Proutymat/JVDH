using UnityEngine;

public class ConfirmButtons : MonoBehaviour
{
    public void YesButton()
    {
        SettingsManager.Instance.ConfirmDeleteDatasButton();
        PanelManager.Instance.SetPanel(PanelState.Options, FadeStyle.FadeInAndOut);
    }

    public void NoButton()
    {
        PanelManager.Instance.SetPanel(PanelState.Options, FadeStyle.FadeInAndOut);
    }
}

using UnityEngine;

public class BackButton : MonoBehaviour
{
    public void ClickButton()
    {
        // If we're leaving settings, save them
        if (PanelManager.Instance.GetPanelState == PanelState.Settings)
        {
            SaveManager.Instance.Save();
        }
        
        SoundManager.Instance.PlayClicSound();
        GameManager.Instance.LoadMainMenu();
    }
}

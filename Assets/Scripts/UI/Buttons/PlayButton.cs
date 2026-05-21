using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void ClickButton()
    {
        SoundManager.Instance.PlayClicSound();
        SoundManager.Instance.StopMusic();
        
        // todo: add parameter to know if he can start at last played video
        // Show startup panel before starting the game
        if (SaveManager.Instance.Data.progression.currentVideo != 0)
        {
            PanelManager.Instance.SetPanel(PanelState.Startup, FadeStyle.FadeInAndOut);
        }
        // Start the game directly
        else
        {
            GameManager.Instance.StartGame();
        }
    }
}

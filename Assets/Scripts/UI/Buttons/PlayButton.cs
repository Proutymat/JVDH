using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void ClickButton()
    {
        SoundManager.Instance.PlayClicSound();
        
        // todo: add parameter to know if he can start at last played video
        // Show startup panel before starting the game
        if (true)
        {
            SoundManager.Instance.StopMusic();
            PanelManager.Instance.SetPanel(PanelState.Startup, FadeStyle.FadeInAndOut);
        }
        // Start the game directly
        else
        {
            GameManager.Instance.StartGame();
        }
    }
}

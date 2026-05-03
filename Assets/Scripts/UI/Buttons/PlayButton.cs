using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void ClickButton()
    {
        SoundManager.Instance.PlayClicSound();
        GameManager.Instance.StartGame();
    }
}

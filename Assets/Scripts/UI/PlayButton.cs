using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.StartGame();
        SoundManager.Instance.PlayClicSound();
    }
}

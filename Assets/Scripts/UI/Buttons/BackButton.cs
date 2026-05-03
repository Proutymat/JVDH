using UnityEngine;

public class BackButton : MonoBehaviour
{
    public void ClickButton()
    {
        SoundManager.Instance.PlayClicSound();
        GameManager.Instance.LoadMainMenu(false, false);
    }
}

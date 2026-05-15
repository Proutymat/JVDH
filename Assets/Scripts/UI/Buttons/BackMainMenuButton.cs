using UnityEngine;

public class BackMainMenuButton : MonoBehaviour
{
    public void ClickButton()
    {
        SoundManager.Instance.PlayClicSound();
        GameManager.Instance.BackToMainMenu();
    }
}

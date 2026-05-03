using UnityEngine;

public class CreditsButton : MonoBehaviour
{
    public void ClickButton()
    {
        SoundManager.Instance.PlayClicSound();
        GameManager.Instance.LoadCreditsMenu();
    }
}

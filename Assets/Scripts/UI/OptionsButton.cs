using UnityEngine;

public class OptionsButton : MonoBehaviour
{
    public void ClickButton()
    {
        SoundManager.Instance.PlayClicSound();
        GameManager.Instance.LoadOptionsMenu();
    }
    
    
}

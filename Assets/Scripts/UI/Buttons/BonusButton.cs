using UnityEngine;

public class BonusButton : MonoBehaviour
{
    public void ClickButton()
    {
        SoundManager.Instance.PlayClicSound();
        GameManager.Instance.LoadBonusMenu();
    }
}

using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public void ClickButton()
    {
        GameManager.Instance.LoadOptionsMenu();
    }
}

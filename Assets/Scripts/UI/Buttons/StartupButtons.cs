using UnityEngine;

public class StartupButtons : MonoBehaviour
{
    public void YesButton()
    {
        GameManager.Instance.StartGame();
    }

    public void NoButton()
    {
        GameManager.Instance.StartGame();
    }
}

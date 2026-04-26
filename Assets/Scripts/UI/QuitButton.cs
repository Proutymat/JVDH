using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitApplication()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
        #else
                Application.Quit();
        #endif
    }
}

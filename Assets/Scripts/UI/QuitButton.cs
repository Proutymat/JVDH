using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitApplication()
    {
        SoundManager.Instance.PlayClicSound();
        
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
        #else
                Application.Quit();
        #endif
    }
}

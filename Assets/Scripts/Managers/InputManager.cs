using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Title("Set in inspector")]
    [SerializeField] GameObject m_defaultSelected;
    
    private bool m_isGamepad;
    private GameObject m_currentSelected;
    private static InputManager m_instance;
    public static InputManager Instance => m_instance;

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning("Multiple InputManager instances in scene!");
            Destroy(gameObject);
            return;
        }

        m_instance = this;

        m_isGamepad = false;
        m_currentSelected = m_defaultSelected;
    }

    public void SetSelected(GameObject selected)
    {
        m_currentSelected = selected;
        if (EventSystem.current.currentSelectedGameObject != selected)
            EventSystem.current.SetSelectedGameObject(selected);
    }
    
    
    void Update()
    {
        // Mouse check
        if (Mouse.current != null &&
            (Mouse.current.delta.ReadValue() != Vector2.zero ||
             Mouse.current.leftButton.wasPressedThisFrame))
        {
            PanelManager.ShowCursor(true);
            m_isGamepad = false;
        }
        
        // Gamepad and keyboard check
        if (Gamepad.current != null && (
                Gamepad.current.leftStick.ReadValue() != Vector2.zero ||
                Gamepad.current.dpad.ReadValue() != Vector2.zero ||
                Gamepad.current.buttonSouth.wasPressedThisFrame) 
            || Keyboard.current != null &&
                Keyboard.current.anyKey.wasPressedThisFrame)
        {
            PanelManager.ShowCursor(false);
            m_isGamepad = true;
        }


        // If clicked on background, select last button
        if (EventSystem.current.currentSelectedGameObject == null && m_isGamepad)
        {
            EventSystem.current.SetSelectedGameObject(m_currentSelected);
        }
    }
}
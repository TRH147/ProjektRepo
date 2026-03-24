using UnityEngine;

public class CursorManager : MonoBehaviour
{
    void Start()
    {
        // Hide and lock cursor at start
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Show cursor when ESC is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Optional: Hide cursor when clicking mouse (for FPS games)
        if (Input.GetMouseButtonDown(0) && Cursor.visible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
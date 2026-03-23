using UnityEngine;

public class NameRotator : MonoBehaviour
{
    void Update()
    {
        // Add null check to prevent error when camera is destroyed
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
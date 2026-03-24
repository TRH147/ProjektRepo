using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public static MouseLook instance;

    [Header("Settings")]
    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor = true;

    [Header("Sensitivity")]
    public Vector2 sensitivity = new Vector2(2, 2);

    [Header("First Person")]
    public GameObject characterBody;

    private Vector2 _mouseAbsolute;
    private float targetYaw;

    [HideInInspector]
    public bool scoped;

    void Start()
    {
        instance = this;

        if (characterBody)
            targetYaw = characterBody.transform.eulerAngles.y;

        if (lockCursor)
            LockCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Movement.cs FixedUpdate-ből hívja
    public float GetTargetYaw()
    {
        return targetYaw;
    }

    void Update()
    {
        // Nyers egér input
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity.x;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity.y;

        // Y tengely (fel-le nézés) – csak a kamerán
        _mouseAbsolute.y += mouseY;
        if (clampInDegrees.y < 360)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, Vector3.right);

        // X tengely (jobbra-balra nézés) – csak tároljuk, Movement forgatja a testet
        targetYaw += mouseX;
        if (clampInDegrees.x < 360)
            targetYaw = Mathf.Clamp(targetYaw, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
    }
}
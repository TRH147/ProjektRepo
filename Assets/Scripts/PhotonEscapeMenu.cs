using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class PhotonEscapeMenu : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public GameObject menuPanel;
    public Button exitButton;

    [Header("Settings")]
    public KeyCode menuKey = KeyCode.Escape;

    [Header("Player References - Drag here if possible")]
    public GameObject playerObject; // Drag your Player GameObject here
    public MouseLook mouseLookScript;
    public Weapon weaponScript;
    public Movement movementScript;

    private bool isMenuOpen = false;
    private bool isExiting = false;
    private AnimationSyncer animationSyncerScript;
    private GameObject player;

    // Store the original rigidbody constraints
    private RigidbodyConstraints originalConstraints;
    private Rigidbody playerRigidbody;

    void Start()
    {
        // Make sure menu starts closed
        if (menuPanel != null)
            menuPanel.SetActive(false);

        // Setup exit button with click protection
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);

            // Add EventTrigger to prevent click from passing through to weapon
            AddEventTriggerToButton(exitButton);
        }

        // Find the player
        FindPlayer();

        // Get player's rigidbody
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
        }

        // Lock cursor at start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // New method to prevent button clicks from passing through to the game
    void AddEventTriggerToButton(Button button)
    {
        // Check if an EventTrigger already exists, if not add one
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Create a new entry for PointerDown event
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;

        // Add a callback that consumes the event
        entry.callback.AddListener((data) => {
            // This empty callback consumes the event
            Debug.Log("Button click intercepted - preventing weapon fire");
        });

        trigger.triggers.Add(entry);
    }

    void FindPlayer()
    {
        // If playerObject is assigned manually, use that
        if (playerObject != null)
        {
            player = playerObject;
        }
        else
        {
            // Try to find the player by tag or name
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer;
                Debug.Log("Found player by tag: Player");
            }
            else
            {
                // Try to find by name
                foundPlayer = GameObject.Find("Player");
                if (foundPlayer != null)
                {
                    player = foundPlayer;
                    Debug.Log("Found player by name: Player");
                }
                else
                {
                    Debug.LogError("Could not find Player GameObject! Please assign playerObject manually in the Inspector.");
                    return;
                }
            }
        }

        Debug.Log($"Player found: {player.name}");

        // If scripts weren't assigned manually, find them on the player
        if (mouseLookScript == null)
        {
            Transform fpCamera = player.transform.Find("ScaleFix/FP_Camera");
            if (fpCamera != null)
            {
                mouseLookScript = fpCamera.GetComponent<MouseLook>();
                Debug.Log($"MouseLook script {(mouseLookScript != null ? "found" : "NOT found")} on FP_Camera");
            }
            else
            {
                Debug.LogError("FP_Camera not found under ScaleFix in Player!");
            }
        }

        if (weaponScript == null)
        {
            Transform weaponObj = player.transform.Find("ScaleFix/FP_Camera/Weapon_02");
            if (weaponObj != null)
            {
                weaponScript = weaponObj.GetComponent<Weapon>();
                Debug.Log($"Weapon script {(weaponScript != null ? "found" : "NOT found")} on Weapon_02");
            }
        }

        if (movementScript == null)
        {
            movementScript = player.GetComponent<Movement>();
            Debug.Log($"Movement script {(movementScript != null ? "found" : "NOT found")} on Player");
        }

        animationSyncerScript = player.GetComponent<AnimationSyncer>();
        Debug.Log($"AnimationSyncer script {(animationSyncerScript != null ? "found" : "NOT found")} on Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(menuKey) && !isExiting)
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (menuPanel != null)
            menuPanel.SetActive(isMenuOpen);

        if (isMenuOpen)
        {
            // Store original rigidbody constraints
            if (playerRigidbody != null)
            {
                originalConstraints = playerRigidbody.constraints;
                // Freeze rotation to prevent spinning, but allow movement
                playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }

            // Menu opened - unlock cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Disable scripts to prevent input while menu is open
            if (mouseLookScript != null)
            {
                mouseLookScript.enabled = false;
                // Try to reset any mouse look values if possible
                ResetMouseLook();
            }

            if (weaponScript != null) weaponScript.enabled = false;
            if (movementScript != null) movementScript.enabled = false;
            if (animationSyncerScript != null) animationSyncerScript.enabled = false;

            Debug.Log("Menu opened - all player input disabled, rotation frozen");
        }
        else
        {
            // Restore original rigidbody constraints
            if (playerRigidbody != null)
            {
                playerRigidbody.constraints = originalConstraints;
            }

            // Menu closed - lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Re-enable scripts
            if (mouseLookScript != null) mouseLookScript.enabled = true;
            if (weaponScript != null) weaponScript.enabled = true;
            if (movementScript != null) movementScript.enabled = true;
            if (animationSyncerScript != null) animationSyncerScript.enabled = true;

            Debug.Log("Menu closed - all player input enabled, rotation restored");
        }
    }

    // Helper method to reset mouse look values using reflection
    private void ResetMouseLook()
    {
        if (mouseLookScript == null) return;

        // Try to find common MouseLook variable names and reset them
        var type = mouseLookScript.GetType();

        // Try to set rotation to 0 using reflection
        var rotationXField = type.GetField("xRotation", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var rotationYField = type.GetField("yRotation", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var currentRotationField = type.GetField("currentRotation", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (rotationXField != null) rotationXField.SetValue(mouseLookScript, 0f);
        if (rotationYField != null) rotationYField.SetValue(mouseLookScript, 0f);
        if (currentRotationField != null) currentRotationField.SetValue(mouseLookScript, Quaternion.identity);
    }

    public void ExitGame()
    {
        if (isExiting) return; // Prevent multiple calls

        Debug.Log("Exiting game...");
        isExiting = true;

        if (RoomManager.Instance != null)
            RoomManager.Instance.SetQuittingFlag(true);

        // Disconnect from Photon (don't wait for it)
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Disconnecting from Photon...");
            PhotonNetwork.Disconnect();
        }

        // QUIT IMMEDIATELY - no waiting, no coroutines
        ForceQuit();
    }

    void ForceQuit()
    {
        Debug.Log("Force quitting application...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Method 1: Application.Quit
        Application.Quit();
        
        // Method 2: If we're still here, force kill (this will execute immediately)
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected: {cause}");
        // Don't do anything - we're quitting
    }
}
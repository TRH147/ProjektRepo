using UnityEngine;
using Photon.Pun;

public class AnimationSyncer : MonoBehaviour, IPunObservable
{
    public Animator animator; // This is for the Player Model (body movement)

    [Header("Animator Parameters")]
    public string horizontalParam = "Horizontal";
    public string verticalParam = "Vertical";

    [Range(-2, 2)]
    public float horizontal;
    [Range(-2, 2)]
    public float vertical;

    [Header("Third Person Weapon")]
    public GameObject thirdPersonWeaponObject; // Drag TP_GunHolder/Weapon_02 here
    private Animator thirdPersonWeaponAnimator;

    [Header("Weapon Animation Parameters")]
    private bool isShooting = false;
    private bool isReloading = false;

    private string currentWeaponState = "Weapon_Idle";
    private bool hasInitialized = false;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        // Try to find the weapon automatically if not assigned
        if (thirdPersonWeaponObject == null)
        {
            FindThirdPersonWeapon();
        }
    }

    void FindThirdPersonWeapon()
    {
        // Find Player Model first (it's under ScaleFix)
        Transform playerModel = transform.Find("ScaleFix/Player Model");
        if (playerModel == null)
        {
            Transform scaleFix = transform.Find("ScaleFix");
            if (scaleFix != null)
            {
                playerModel = scaleFix.Find("Player Model");
            }
        }

        if (playerModel != null)
        {
            Debug.Log($"Found Player Model: {playerModel.name}");

            // Find TP_GunHolder under Player Model
            Transform tpGunHolder = playerModel.Find("TP_GunHolder");
            if (tpGunHolder != null)
            {
                Debug.Log($"Found TP_GunHolder: {tpGunHolder.name}");

                // Find Weapon_02 under TP_GunHolder
                Transform weapon = tpGunHolder.Find("Weapon_02");
                if (weapon != null)
                {
                    thirdPersonWeaponObject = weapon.gameObject;
                    Debug.Log($"Auto-found third person weapon: {GetFullPath(weapon)}");
                }
            }
        }
    }

    void Start()
    {
        Debug.Log($"=== ANIMATION SYNCER START on {gameObject.name} ===");
        Debug.Log($"Is mine: {photonView.IsMine}");

        InitializeThirdPersonWeapon();

        // IMPORTANT: For remote players, ensure TP weapon is active
        if (photonView != null && !photonView.IsMine)
        {
            Debug.Log("This is a REMOTE player - activating TP weapon");
            if (thirdPersonWeaponObject != null && !thirdPersonWeaponObject.activeSelf)
            {
                thirdPersonWeaponObject.SetActive(true);
            }
        }
    }

    void InitializeThirdPersonWeapon()
    {
        if (hasInitialized) return;

        if (thirdPersonWeaponObject != null)
        {
            Debug.Log($"Third person weapon object: {thirdPersonWeaponObject.name}");
            Debug.Log($"Is active self: {thirdPersonWeaponObject.activeSelf}");
            Debug.Log($"Is active in hierarchy: {thirdPersonWeaponObject.activeInHierarchy}");
            Debug.Log($"Full path: {GetFullPath(thirdPersonWeaponObject.transform)}");

            // Get the animator
            thirdPersonWeaponAnimator = thirdPersonWeaponObject.GetComponent<Animator>();

            if (thirdPersonWeaponAnimator != null)
            {
                Debug.Log($"Animator exists: yes");
                Debug.Log($"Animator enabled: {thirdPersonWeaponAnimator.enabled}");
            }
            else
            {
                Debug.LogError("Animator component not found on third person weapon!");
            }

            hasInitialized = true;
            Debug.Log("Third person weapon initialized successfully");
        }
        else
        {
            Debug.LogError("Third person weapon object is null! Please assign it in the inspector.");
        }
    }

    string GetFullPath(Transform obj)
    {
        string path = obj.name;
        while (obj.parent != null)
        {
            obj = obj.parent;
            path = obj.name + "/" + path;
        }
        return path;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send movement parameters
            stream.SendNext(horizontal);
            stream.SendNext(vertical);

            // Send weapon animation states
            stream.SendNext(isShooting);
            stream.SendNext(isReloading);
        }
        else
        {
            // Receive movement parameters
            horizontal = (float)stream.ReceiveNext();
            vertical = (float)stream.ReceiveNext();

            // Receive weapon animation states
            bool receivedIsShooting = (bool)stream.ReceiveNext();
            bool receivedIsReloading = (bool)stream.ReceiveNext();

            // Apply to third person weapon animator
            if (thirdPersonWeaponAnimator != null && thirdPersonWeaponObject.activeInHierarchy)
            {
                // Update shooting state
                if (receivedIsShooting != isShooting)
                {
                    thirdPersonWeaponAnimator.SetBool("IsShooting", receivedIsShooting);
                    isShooting = receivedIsShooting;
                }

                // Update reloading state (trigger)
                if (receivedIsReloading && !isReloading)
                {
                    thirdPersonWeaponAnimator.SetTrigger("Reload");
                }
                isReloading = receivedIsReloading;
            }
        }
    }

    [PunRPC]
    public void RPC_PlayJump()
    {
        if (animator != null)
            animator.SetTrigger("Jump");
    }

    [PunRPC]
    public void SyncWeaponAnimation(string stateName, float normalizedTime)
    {
        if (!hasInitialized)
        {
            InitializeThirdPersonWeapon();
        }

        if (thirdPersonWeaponAnimator == null || thirdPersonWeaponObject == null)
        {
            return;
        }

        // For remote players receiving RPCs, ensure weapon is active
        if (!thirdPersonWeaponObject.activeInHierarchy)
        {
            thirdPersonWeaponObject.SetActive(true);
        }

        // Handle different animation states
        if (stateName == "Weapon_Shoot")
        {
            thirdPersonWeaponAnimator.SetBool("IsShooting", true);
            // Schedule reset (will be handled by the animation itself or another RPC)
            Invoke("ResetShooting", 0.2f);
        }
        else if (stateName == "Reload")
        {
            thirdPersonWeaponAnimator.SetTrigger("Reload");
        }
        else if (stateName == "Weapon_Idle")
        {
            thirdPersonWeaponAnimator.SetBool("IsShooting", false);
        }

        currentWeaponState = stateName;
    }

    void ResetShooting()
    {
        if (thirdPersonWeaponAnimator != null)
        {
            thirdPersonWeaponAnimator.SetBool("IsShooting", false);
        }
    }

    public void PlayWeaponAnimation(string stateName, float normalizedTime = 0)
    {
        if (!hasInitialized)
        {
            InitializeThirdPersonWeapon();
        }

        if (thirdPersonWeaponAnimator == null || thirdPersonWeaponObject == null)
        {
            return;
        }

        // FOR LOCAL PLAYER: Don't try to play on inactive TP weapon
        if (photonView != null && photonView.IsMine)
        {
            // Local player's TP weapon should be inactive - silently return
            return;
        }

        // FOR REMOTE PLAYERS: Ensure weapon is active
        if (!thirdPersonWeaponObject.activeInHierarchy)
        {
            Debug.Log("Activating TP weapon for remote player");
            thirdPersonWeaponObject.SetActive(true);
        }

        if (stateName == currentWeaponState) return;

        currentWeaponState = stateName;

        // Update the state variables for network sync
        if (stateName == "Weapon_Shoot")
        {
            isShooting = true;
            isReloading = false;
            thirdPersonWeaponAnimator.SetBool("IsShooting", true);
        }
        else if (stateName == "Reload")
        {
            isReloading = true;
            isShooting = false;
            thirdPersonWeaponAnimator.SetTrigger("Reload");
        }
        else if (stateName == "Weapon_Idle")
        {
            isShooting = false;
            isReloading = false;
            thirdPersonWeaponAnimator.SetBool("IsShooting", false);
        }

        if (photonView != null && PhotonNetwork.IsConnected)
        {
            photonView.RPC("SyncWeaponAnimation", RpcTarget.Others, stateName, normalizedTime);
        }
    }

    void Update()
    {
        if (animator == null) return;

        // Only set the parameters that exist (for body movement)
        if (!string.IsNullOrEmpty(horizontalParam))
            animator.SetFloat(horizontalParam, horizontal);

        if (!string.IsNullOrEmpty(verticalParam))
            animator.SetFloat(verticalParam, vertical);
    }
}
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviourPunCallbacks
{
    [Header("Weapon Stats")]
    public float fireRate = 10f;
    public int damagePerShot = 25;
    public float hitscanDistance = 500f;

    [Header("Animation Set Up")]
    public Animator animator; // First person weapon animator

    [Header("Third Person Weapon Sync")]
    public GameObject thirdPersonWeapon; // Drag TP_GunHolder/Weapon_02 here
    private Animator tpAnimator;
    private string currentAnimationState = "Weapon_Idle";

    [Header("Hit And Kills Manager")]
    public PlayerHitAndKillsManager playerHitAndKillsManager;

    [Header("Hit Particle Set Up")]
    public GameObject concreteHitParticle;
    public GameObject playerHitParticle;

    [Header("Shoot SFX")]
    public PhotonPlayerSoundsManager photonPlayerSoundsManager;
    [Space]
    public byte shootSoundIndex = 0;

    [Header("Ammo Set Up")]
    public int magSize;
    public int currentAmmoInMag = 30;
    [Space]
    public TextMeshProUGUI ammoText;
    public Image ammoIndicator;

    [Header("Muzzle Flash Set Up")]
    public Transform muzzleFlashSpawnPoint;
    public GameObject muzzleFlashPrefab;

    [Header("Camera Reference")]
    public Transform cameraTransfrom;

    // Reference to PlayerShooting script for muzzleflash networking
    private PlayerShooting playerShooting;

    // Reference to AnimationSyncer on the Player object
    private AnimationSyncer animationSyncer;

    // Animation states
    private bool isShooting;
    private bool isReloading;
    private float timeUntilAllowNextShot;

    // Public methods for other scripts to access
    public bool IsShooting() { return isShooting; }
    public bool IsReloading() { return isReloading; }

    void Start()
    {
        UpdateAmmoUI();

        // Get first person animator if not set
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Get third person animator
        if (thirdPersonWeapon != null)
        {
            tpAnimator = thirdPersonWeapon.GetComponent<Animator>();
            if (tpAnimator == null)
            {
                tpAnimator = thirdPersonWeapon.GetComponentInChildren<Animator>();
            }
        }

        // Get reference to PlayerShooting from the player root
        playerShooting = GetComponentInParent<PlayerShooting>();

        // Find AnimationSyncer on the Player object
        animationSyncer = GetComponentInParent<AnimationSyncer>();

        if (animationSyncer == null)
        {
            Debug.LogError("AnimationSyncer not found on Player object!");
        }

        // Log available parameters for debugging
        if (animator != null)
        {
            Debug.Log("=== ANIMATOR PARAMETERS ===");
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                Debug.Log($"Parameter: {param.name}, Type: {param.type}");
            }
        }

        // Start with idle animation
        SetAnimationState("Weapon_Idle");
    }

    void Update()
    {
        timeUntilAllowNextShot = Mathf.Max(0, timeUntilAllowNextShot - Time.deltaTime);

        // Shooting input
        if (Input.GetButton("Fire1") && timeUntilAllowNextShot <= 0 && currentAmmoInMag > 0 && !IsPlayingReloadClip())
        {
            HitscanShoot();
            timeUntilAllowNextShot = 1 / fireRate;
        }

        // Reload input
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    // Sets animation state on both first and third person weapons
    private void SetAnimationState(string stateName, float normalizedTime = 0)
    {
        Debug.Log($"SetAnimationState - current: {currentAnimationState}, new: {stateName}");

        if (currentAnimationState == stateName)
        {
            Debug.Log("Already in this state, ignoring");
            return;
        }

        currentAnimationState = stateName;
        Debug.Log($"Playing weapon animation: {stateName}");

        // For shoot animation, use IsShooting bool
        if (stateName == "Weapon_Shoot")
        {
            if (animator != null)
            {
                Debug.Log("Setting IsShooting=true on first person");
                animator.SetBool("IsShooting", true);
            }

            if (tpAnimator != null && tpAnimator.gameObject.activeInHierarchy)
            {
                Debug.Log("Setting IsShooting=true on third person");
                tpAnimator.SetBool("IsShooting", true);
            }

            // Schedule returning to idle after shoot animation
            Invoke("StopShooting", 0.1f);
        }
        // For reload animation
        else if (stateName == "Reload")
        {
            if (animator != null)
            {
                Debug.Log("Setting Reload trigger on first person");
                animator.SetTrigger("Reload");
            }

            if (tpAnimator != null && tpAnimator.gameObject.activeInHierarchy)
            {
                Debug.Log("Setting Reload trigger on third person");
                tpAnimator.SetTrigger("Reload");
            }
        }
        // For idle
        else if (stateName == "Weapon_Idle")
        {
            if (animator != null)
            {
                Debug.Log("Setting IsShooting=false on first person");
                animator.SetBool("IsShooting", false);
            }

            if (tpAnimator != null && tpAnimator.gameObject.activeInHierarchy)
            {
                Debug.Log("Setting IsShooting=false on third person");
                tpAnimator.SetBool("IsShooting", false);
            }
        }

        // Use AnimationSyncer for network sync
        if (animationSyncer != null)
        {
            animationSyncer.PlayWeaponAnimation(stateName, normalizedTime);
        }
    }

    // Check if reload animation is playing
    private bool IsPlayingReloadClip()
    {
        if (animator == null) return false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Reload");
    }

    // Reload method
    private void Reload()
    {
        // Only allow reload if magazine is not already full AND we're not already reloading
        if (currentAmmoInMag < magSize && !IsPlayingReloadClip())
        {
            Debug.Log("Starting reload");
            isReloading = true;
            SetAnimationState("Reload");
            currentAmmoInMag = magSize;
            UpdateAmmoUI();

            // Reset reloading state after animation completes
            Invoke("ResetReloading", 1.5f);
        }
    }

    private void ResetReloading()
    {
        Debug.Log("ResetReloading called - returning to idle");
        isReloading = false;
        // Return to idle after reload
        SetAnimationState("Weapon_Idle");
    }

    // Update ammo UI
    private void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"<b>{currentAmmoInMag}/</b>{magSize}";

        if (ammoIndicator != null)
            ammoIndicator.fillAmount = (float)currentAmmoInMag / magSize;
    }

    // Main shooting method
    void HitscanShoot()
    {
        Debug.Log("=== HITSCAN SHOOT CALLED ===");
        Debug.Log($"Current ammo: {currentAmmoInMag}, isShooting: {isShooting}, isReloading: {isReloading}");

        isShooting = true;
        currentAmmoInMag--;
        UpdateAmmoUI();

        // Show muzzleflash on first-person weapon (local)
        ShowLocalMuzzleFlash();

        // Tell other players to show muzzleflash on the third-person weapon
        if (playerShooting != null)
        {
            playerShooting.ShowMuzzleFlashOnRemote();
        }

        // Play shoot animation on both weapons
        Debug.Log("Calling SetAnimationState with Weapon_Shoot");
        SetAnimationState("Weapon_Shoot");

        // Play shoot sound
        if (photonPlayerSoundsManager != null)
        {
            photonPlayerSoundsManager.photonView.RPC("RPC_PlayShootSound", RpcTarget.All, shootSoundIndex);
        }

        // Raycast shooting with layer mask to ignore dead players
        Ray ray = new Ray(cameraTransfrom.position, cameraTransfrom.forward);
        RaycastHit hit;

        // Ignore IgnoreRaycast layer
        int layerMask = ~LayerMask.GetMask("IgnoreRaycast");

        if (Physics.Raycast(ray, out hit, hitscanDistance, layerMask))
        {
            Quaternion rotation = Quaternion.LookRotation(hit.normal);

            if (hit.transform.gameObject.CompareTag("Player"))
            {
                PhotonView hitPhotonView = hit.transform.GetComponent<PhotonView>();
                PlayerHealth playerHealth = hit.transform.GetComponent<PlayerHealth>();

                // Don't shoot dead players
                if (playerHealth == null || playerHealth.isDead)
                    return;

                // Store health BEFORE damage to check if this shot will kill
                int healthBeforeDamage = playerHealth.health;

                // Send damage RPC
                hitPhotonView.RPC("RPC_TakeDamage", RpcTarget.All, damagePerShot);

                // Spawn hit particle
                PhotonNetwork.Instantiate(playerHitParticle.name, hit.point, rotation);

                // Check for kill based on health BEFORE damage
                if (healthBeforeDamage <= damagePerShot)
                {
                    if (playerHitAndKillsManager != null)
                        playerHitAndKillsManager.GetKill(hitPhotonView.Owner.NickName);
                }
                else
                {
                    if (playerHitAndKillsManager != null)
                        playerHitAndKillsManager.GetHit(damagePerShot);
                }
            }
            else
            {
                PhotonNetwork.Instantiate(concreteHitParticle.name, hit.point, rotation);
            }
        }
    }

    // Shows muzzleflash on the first-person weapon locally
    void ShowLocalMuzzleFlash()
    {
        if (muzzleFlashPrefab != null && muzzleFlashSpawnPoint != null)
        {
            GameObject muzzleFlashGO = Instantiate(muzzleFlashPrefab, muzzleFlashSpawnPoint.position, muzzleFlashSpawnPoint.rotation);
            muzzleFlashGO.transform.parent = muzzleFlashSpawnPoint;
            Destroy(muzzleFlashGO, 1f);
        }
    }

    // Reset shooting state
    void StopShooting()
    {
        Debug.Log("StopShooting called - returning to idle");
        isShooting = false;

        // Return to idle - this will set IsShooting=false
        SetAnimationState("Weapon_Idle");
    }
}
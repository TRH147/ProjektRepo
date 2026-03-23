using Photon.Pun;
using UnityEngine;

public class ThirdPersonWeapon : MonoBehaviourPunCallbacks
{
    [Header("Animation Setup")]
    public Animator animator;

    [Header("Muzzle Flash Setup")]
    public Transform muzzleFlashSpawnPoint;
    public GameObject muzzleFlashPrefab;

    [Header("Audio Setup")]
    public PhotonPlayerSoundsManager photonPlayerSoundsManager;

    [Header("References")]
    public PlayerHealth playerHealth;

    // Animation state tracking
    private bool isShooting;
    private bool isReloading;
    private bool isDead;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (playerHealth == null)
            playerHealth = GetComponentInParent<PlayerHealth>();
    }

    void Update()
    {
        // Only update animations if we're NOT the local player
        // This script should ONLY control visuals for OTHER players
        if (photonView.IsMine)
        {
            // If this is our own third-person model, we don't need to animate it
            // The first-person script will handle sending RPCs
            return;
        }

        // Update animator parameters based on current state
        if (animator != null)
        {
            animator.SetBool("IsShooting", isShooting);
            animator.SetBool("IsReloading", isReloading);

            // Optional: Add death state
            if (playerHealth != null)
            {
                if (playerHealth.isDead != isDead)
                {
                    isDead = playerHealth.isDead;
                    animator.SetBool("IsDead", isDead);
                }
            }
        }
    }

    // Called by Photon when this weapon needs to show shooting
    [PunRPC]
    public void RPC_PlayShootAnimation()
    {
        // Only play for remote players (or all if you want to test)
        if (photonView.IsMine) return;

        isShooting = true;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        // Reset shooting state after short delay
        CancelInvoke(nameof(StopShooting));
        Invoke(nameof(StopShooting), 0.1f);

        // Show muzzle flash
        if (muzzleFlashPrefab != null && muzzleFlashSpawnPoint != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab,
                muzzleFlashSpawnPoint.position,
                muzzleFlashSpawnPoint.rotation);
            muzzleFlash.transform.parent = muzzleFlashSpawnPoint;
            Destroy(muzzleFlash, 1f);
        }
    }

    // Called by Photon when this weapon needs to show reloading
    [PunRPC]
    public void RPC_PlayReloadAnimation()
    {
        // Only play for remote players
        if (photonView.IsMine) return;

        isReloading = true;

        if (animator != null)
        {
            animator.SetTrigger("Reload");
        }

        // Reset reloading state after animation (adjust time to match your animation)
        CancelInvoke(nameof(StopReloading));
        Invoke(nameof(StopReloading), 1.5f);
    }

    // Called when player takes damage (optional - for hit reactions)
    [PunRPC]
    public void RPC_PlayHitReaction()
    {
        if (photonView.IsMine) return;

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    private void StopShooting()
    {
        isShooting = false;
    }

    private void StopReloading()
    {
        isReloading = false;
    }

    // Optional: Play weapon raise/draw animation
    [PunRPC]
    public void RPC_PlayWeaponDraw()
    {
        if (photonView.IsMine) return;

        if (animator != null)
        {
            animator.SetTrigger("Draw");
        }
    }

    // Optional: Play weapon holster animation
    [PunRPC]
    public void RPC_PlayWeaponHolster()
    {
        if (photonView.IsMine) return;

        if (animator != null)
        {
            animator.SetTrigger("Holster");
        }
    }
}
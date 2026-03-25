using UnityEngine;
using Photon.Pun;

public class TP_Weapon : MonoBehaviour
{
    [Header("Muzzle Flash Set Up")]
    public Transform muzzleFlashSpawnPoint;
    public GameObject muzzleFlashPrefab;

    void Start()
    {
        // Auto-find muzzle flash spawn point if not assigned
        if (muzzleFlashSpawnPoint == null)
        {
            muzzleFlashSpawnPoint = transform.Find("MuzzleFlashInstantiate");
        }
    }

    // Called from PlayerShooting.cs when RPC is received
    public void ShowMuzzleFlash()
    {
        if (muzzleFlashPrefab != null && muzzleFlashSpawnPoint != null)
        {
            GameObject muzzleFlashGO = Instantiate(muzzleFlashPrefab, muzzleFlashSpawnPoint.position, muzzleFlashSpawnPoint.rotation);
            muzzleFlashGO.transform.parent = muzzleFlashSpawnPoint;
            Destroy(muzzleFlashGO, 1f);
        }
    }
}
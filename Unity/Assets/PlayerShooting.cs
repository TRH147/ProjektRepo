using UnityEngine;
using Photon.Pun;

public class PlayerShooting : MonoBehaviourPunCallbacks
{
    private TP_Weapon thirdPersonWeapon;

    void Start()
    {
        // Find the third-person weapon in the hierarchy
        Transform playerModel = transform.Find("ScaleFix/Player Model");
        if (playerModel != null)
        {
            Transform hips = playerModel.Find("mixamorig:Hips");
            if (hips != null)
            {
                Transform spine = hips.Find("mixamorig:Spine");
                if (spine != null)
                {
                    Transform spine1 = spine.Find("mixamorig:Spine1");
                    if (spine1 != null)
                    {
                        Transform spine2 = spine1.Find("mixamorig:Spine2");
                        if (spine2 != null)
                        {
                            Transform tpGunHolder = spine2.Find("TP_GunHolder");
                            if (tpGunHolder != null)
                            {
                                Transform weaponTransform = tpGunHolder.Find("Weapon_02");
                                if (weaponTransform != null)
                                {
                                    thirdPersonWeapon = weaponTransform.GetComponent<TP_Weapon>();
                                    if (thirdPersonWeapon == null)
                                    {
                                        thirdPersonWeapon = weaponTransform.gameObject.AddComponent<TP_Weapon>();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Called from Weapon.cs when local player shoots
    public void ShowMuzzleFlashOnRemote()
    {
        if (photonView != null)
        {
            photonView.RPC("RPC_ShowMuzzleFlash", RpcTarget.Others);
        }
    }

    [PunRPC]
    void RPC_ShowMuzzleFlash()
    {
        // This runs on other players' machines
        if (thirdPersonWeapon != null)
        {
            thirdPersonWeapon.ShowMuzzleFlash();
        }
    }
}
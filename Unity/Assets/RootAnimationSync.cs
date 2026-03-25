using Photon.Pun;
using UnityEngine;

public class RootAnimationSync : MonoBehaviourPun
{
    [Header("Animator References")]
    public Animator thirdPersonAnimator; // Drag Player Model's animator here
    public Animator firstPersonWeaponAnimator; // Drag FP_Camera/Weapon_02's animator here

    [Header("Animation States")]
    private string currentState = "Weapon_Idle";

    [PunRPC]
    public void SyncAnimationState(string stateName, float normalizedTime)
    {
        // Play on third person model
        if (thirdPersonAnimator != null)
        {
            thirdPersonAnimator.Play(stateName, 0, normalizedTime);
            currentState = stateName;
        }
    }

    // Call this from Weapon.cs to sync animations
    public void PlayAnimation(string stateName, float normalizedTime = 0)
    {
        if (photonView != null && PhotonNetwork.IsConnected)
        {
            photonView.RPC("SyncAnimationState", RpcTarget.Others, stateName, normalizedTime);
        }

        // Also play locally on third person
        if (thirdPersonAnimator != null)
        {
            thirdPersonAnimator.Play(stateName, 0, normalizedTime);
        }
    }
}
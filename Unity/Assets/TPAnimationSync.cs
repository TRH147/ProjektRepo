using Photon.Pun;
using UnityEngine;

public class TPAnimationSync : MonoBehaviourPun
{
    public Animator tpWeaponAnimator;  // TP_GunHolder/Weapon_02
    public Animator fpWeaponAnimator;  // FP_Camera/Weapon_02

    void Update()
    {
        if (fpWeaponAnimator == null || tpWeaponAnimator == null) return;

        // Just mirror the weapon animation
        AnimatorStateInfo state = fpWeaponAnimator.GetCurrentAnimatorStateInfo(0);
        string animName = GetAnimationName(state);

        // Check if current animation is different
        AnimatorStateInfo tpState = tpWeaponAnimator.GetCurrentAnimatorStateInfo(0);
        if (!tpState.IsName(animName))
        {
            tpWeaponAnimator.Play(animName, 0, state.normalizedTime);
        }
    }

    string GetAnimationName(AnimatorStateInfo stateInfo)
    {
        if (stateInfo.IsName("Weapon_Idle")) return "Weapon_Idle";
        if (stateInfo.IsName("Weapon_Walk")) return "Weapon_Walk";
        if (stateInfo.IsName("Weapon_Shoot")) return "Weapon_Shoot";
        if (stateInfo.IsName("Reload")) return "Reload";
        return "Weapon_Idle";
    }
}
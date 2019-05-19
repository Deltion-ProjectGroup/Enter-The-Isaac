using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoModifierEffect : Effect
{
    [SerializeField] int modifyAmount;
    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().gun.gunClone.maxAmmo += modifyAmount;
    }
    public override void RemoveEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().gun.gunClone.maxAmmo -= modifyAmount;
    }
    public void OnSwapGun(PlayerController thisPlayer)
    {
        thisPlayer.gun.gunClone.maxAmmo += modifyAmount;
    }
}

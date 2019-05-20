using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoModifierEffect : Effect
{
    [SerializeField] int modifyAmount;
    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().gun.gunClone.maxAmmo += modifyAmount;
        target.GetComponent<PlayerController>().gun.onSwapGun += OnSwapGun;
    }
    public override void RemoveEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().gun.gunClone.maxAmmo -= modifyAmount;
        target.GetComponent<PlayerController>().gun.onSwapGun -= OnSwapGun;
    }
    public void OnSwapGun(Gun gunSwappedTo)
    {
        gunSwappedTo.gunClone.maxAmmo += modifyAmount;
    }
}

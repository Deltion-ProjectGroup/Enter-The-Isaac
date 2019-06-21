using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateModfierEffect : GunEffect
{
    public float modifyAmount;
    public override void ApplyEffect(GameObject target)
    {
        Gun gun = target.GetComponent<PlayerController>().gun;
        gun.gunClone.fireRate = Mathf.Max(gun.gunClone.fireRate + modifyAmount, gun.gunClone.minFireRate);
        gun.onSwapGun += OnSwapGun;
    }
    public override void RemoveEffect(GameObject target)
    {
        Gun gun = target.GetComponent<PlayerController>().gun;
        gun.gunClone.fireRate += modifyAmount;
        gun.onSwapGun -= OnSwapGun;
    }
    public void OnSwapGun(Gun gunSwappedTo, GameObject player)
    {
        gunSwappedTo.gunClone.fireRate = Mathf.Max(gunSwappedTo.gunClone.fireRate - modifyAmount, gunSwappedTo.gunClone.minFireRate);
    }
}

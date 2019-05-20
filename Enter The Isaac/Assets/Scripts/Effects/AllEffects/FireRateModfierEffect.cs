using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateModfierEffect : GunEffect
{
    public float modifyPercentage;
    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().gun.gunClone.fireRate += modifyPercentage;
        target.GetComponent<PlayerController>().gun.onSwapGun += OnSwapGun;
    }
    public override void RemoveEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().gun.gunClone.fireRate -= modifyPercentage;
        target.GetComponent<PlayerController>().gun.onSwapGun -= OnSwapGun;
    }
    public void OnSwapGun(Gun gunSwappedTo)
    {
        gunSwappedTo.gunClone.fireRate += modifyPercentage;
    }
}

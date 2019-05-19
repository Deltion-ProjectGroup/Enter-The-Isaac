using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateModfierEffect : GunEffect
{
    public int modifyValue;
    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<PlayerController>().gun.gunClone.fireRate += modifyValue;
    }
    public override void RemoveEffect(GameObject target)
    {
        throw new System.NotImplementedException();
    }
    public void OnSwapGun(PlayerController thisPlayer)
    {
        thisPlayer.gun.gunClone.fireRate += modifyValue;
    }
}

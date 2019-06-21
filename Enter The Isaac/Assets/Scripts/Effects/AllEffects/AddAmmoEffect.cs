using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAmmoEffect : Effect
{
    public float ammoPercentage;
    public override void ApplyEffect(GameObject target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        float ammoRefundAmount = player.gun.gunClone.maxAmmo;
        ammoRefundAmount *= (ammoPercentage / 100);
        player.magazineStore[player.curGun] = Mathf.Min(player.magazineStore[player.curGun] + (int)ammoRefundAmount, player.gun.gunClone.maxAmmo);
        player.gun.totalMaxAmmo = Mathf.Min(player.gun.totalMaxAmmo + (int)ammoRefundAmount, player.gun.gunClone.maxAmmo);
    }
    public override void RemoveEffect(GameObject target)
    {
        throw new System.NotImplementedException();
    }
}

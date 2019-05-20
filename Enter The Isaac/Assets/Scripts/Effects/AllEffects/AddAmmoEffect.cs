using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAmmoEffect : Effect
{
    public int ammoAmount;
    public override void ApplyEffect(GameObject target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        player.magazineStore[player.curGun] = Mathf.Min(player.magazineStore[player.curGun] + ammoAmount, player.guns[player.curGun].maxAmmo);
    }
    public override void RemoveEffect(GameObject target)
    {
        throw new System.NotImplementedException();
    }
}

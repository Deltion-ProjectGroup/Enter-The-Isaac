using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineModifierEffect : Effect
{
    public int modifyAmount;
    public override void ApplyEffect(GameObject target)
    {
        Gun gun = target.GetComponent<PlayerController>().gun;
        gun.gunClone.magazineSize += modifyAmount;
        gun.onSwapGun += OnSwapGun;

    }
    public override void RemoveEffect(GameObject target)
    {
        Gun gun = target.GetComponent<PlayerController>().gun;
        gun.gunClone.magazineSize -= modifyAmount;
        gun.onSwapGun -= OnSwapGun;
    }
    public void OnSwapGun(Gun newGun)
    {
        newGun.gunClone.magazineSize += modifyAmount;
    }
}

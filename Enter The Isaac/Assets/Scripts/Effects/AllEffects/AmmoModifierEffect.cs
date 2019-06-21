using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoModifierEffect : Effect
{
    [SerializeField] int modifyAmount;
    public override void ApplyEffect(GameObject target)
    {
        print(target);
        PlayerController controller = target.GetComponent<PlayerController>();
        controller.gun.gunClone.maxAmmo += (int)modifyAmount;
        controller.magazineStore[controller.curGun] += modifyAmount;
        controller.gun.onSwapGun += OnSwapGun;
    }
    public override void RemoveEffect(GameObject target)
    {
        PlayerController controller = target.GetComponent<PlayerController>();
        controller.gun.gunClone.maxAmmo -= (int)modifyAmount;
        controller.magazineStore[controller.curGun] -= modifyAmount;
        target.GetComponent<PlayerController>().gun.onSwapGun -= OnSwapGun;
    }
    public void OnSwapGun(Gun gunSwappedTo, GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        gunSwappedTo.gunClone.maxAmmo += (int)modifyAmount;
        controller.magazineStore[controller.curGun] += modifyAmount;
    }
}

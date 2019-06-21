using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoModifierEffect : Effect
{
    [SerializeField] int modifyPercentage;
    public override void ApplyEffect(GameObject target)
    {
        PlayerController controller = target.GetComponent<PlayerController>();
        float modifyAmount = (controller.guns[controller.curGun].maxAmmo * (modifyPercentage / 100));
        controller.gun.gunClone.maxAmmo += (int)modifyAmount;
        controller.gun.onSwapGun += OnSwapGun;
    }
    public override void RemoveEffect(GameObject target)
    {
        PlayerController controller = target.GetComponent<PlayerController>();
        float modifyAmount = (controller.guns[controller.curGun].maxAmmo * (modifyPercentage / 100));
        controller.gun.gunClone.maxAmmo -= (int)modifyAmount;
        target.GetComponent<PlayerController>().gun.onSwapGun -= OnSwapGun;
    }
    public void OnSwapGun(Gun gunSwappedTo, GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        float modifyAmount = (controller.guns[controller.curGun].maxAmmo * (modifyPercentage / 100));
        gunSwappedTo.gunClone.maxAmmo += (int)modifyAmount;
    }
}

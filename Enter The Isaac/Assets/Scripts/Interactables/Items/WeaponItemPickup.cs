using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemPickup : InventoryItemPickup
{
    public GunType gunData;
    public override void OnInteract(GameObject player)
    {
        player.GetComponent<Inventory>().weaponItems.Add(pickUpData);
        player.GetComponent<PlayerController>().guns.Add(gunData);
        player.GetComponent<PlayerController>().ammoStore.Add(gunData.magazineSize);
        player.GetComponent<PlayerController>().magazineStore.Add(gunData.maxAmmo);
        player.GetComponent<PlayerController>().curGun = player.GetComponent<PlayerController>().guns.Count - 1;
        base.OnInteract(player);
    }
    public override void OnDropItem(GameObject player)
    {
        player.GetComponent<Inventory>().weaponItems.Remove(pickUpData);
        player.GetComponent<PlayerController>().guns.Remove(gunData);
        player.GetComponent<PlayerController>().ammoStore.Remove(gunData.magazineSize);
        player.GetComponent<PlayerController>().magazineStore.Remove(gunData.maxAmmo);
        if(player.GetComponent<PlayerController>().guns.Count < player.GetComponent<PlayerController>().curGun)
        {
            player.GetComponent<PlayerController>().curGun--;
        }
    }   
}

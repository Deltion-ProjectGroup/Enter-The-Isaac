using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemPickup : InventoryItemPickup
{
    public GunType gunData;
    public int remainingAmmoStore;
    public int remainingClipStore;
    public override void OnInteract(GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        if(controller.guns.Count >= 2)
        {
            GameObject droppedWeapon = Instantiate(controller.guns[controller.curGun].dropPrefab, transform.position, transform.rotation);
            droppedWeapon.GetComponent<WeaponItemPickup>().remainingAmmoStore = controller.magazineStore[controller.curGun];
            droppedWeapon.GetComponent<WeaponItemPickup>().remainingClipStore = controller.ammoStore[controller.curGun];
            player.GetComponent<Inventory>().weaponItems[controller.curGun] = pickUpData;
            controller.guns[controller.curGun] = gunData;
            controller.ammoStore[controller.curGun] = gunData.magazineSize;
            controller.magazineStore[controller.curGun] = gunData.maxAmmo;
            base.OnInteract(player);
        }
        else
        {
            player.GetComponent<Inventory>().weaponItems.Add(pickUpData);
            controller.guns.Add(gunData);
            controller.ammoStore.Add(gunData.magazineSize);
            controller.magazineStore.Add(gunData.maxAmmo);
            controller.curGun = controller.guns.Count - 1;
        }
    }
    public override void OnDropItem(GameObject player)
    {
        if(player.GetComponent<PlayerController>().guns.Count > 1)
        {
            player.GetComponent<Inventory>().weaponItems.Remove(pickUpData);
            player.GetComponent<PlayerController>().guns.Remove(gunData);
            player.GetComponent<PlayerController>().ammoStore.Remove(gunData.magazineSize);
            player.GetComponent<PlayerController>().magazineStore.Remove(gunData.maxAmmo);
            if (player.GetComponent<PlayerController>().guns.Count < player.GetComponent<PlayerController>().curGun)
            {
                player.GetComponent<PlayerController>().curGun--;
            }
        }
    }   
}

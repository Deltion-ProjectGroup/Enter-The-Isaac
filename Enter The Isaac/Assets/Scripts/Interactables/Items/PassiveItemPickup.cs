using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItemPickup : InventoryItemPickup
{
    public override void Interact(GameObject player)
    {
        player.GetComponent<Inventory>().passiveItems.Add(pickUpData);
        base.Interact(player);
    }
    public override void OnDropItem(GameObject player)
    {
        player.GetComponent<Inventory>().passiveItems.Remove(pickUpData);
    }
}

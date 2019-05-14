using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItemPickup : Interactable
{
    public PickupSO pickUpData;
    public override void Interact(GameObject player)
    {
        Destroy(gameObject);
    }
    public abstract void OnDropItem(GameObject player);
}

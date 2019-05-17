using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItemPickup : Interactable
{
    public PickupSO pickUpData;
    public override void OnInteract(GameObject player)
    {
        Destroy(gameObject);
    }
    public abstract void OnDropItem(GameObject player);
    public sealed override void Interact(GameObject player_)
    {
        base.Interact(player_);
    }
}

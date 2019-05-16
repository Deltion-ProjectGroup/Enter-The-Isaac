using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItemPickup : InventoryItemPickup
{
    public override void Interact(GameObject player)
    {
        Effect[] effects = GetComponents<Effect>();
        foreach(Effect effect in effects)
        {
            effect.ApplyEffect(player);
        }
        player.GetComponent<Inventory>().passiveItems.Add(pickUpData);
        base.Interact(player);
    }
    public override void OnDropItem(GameObject player)
    {
        Effect[] effects = GetComponents<Effect>();
        foreach (Effect effect in effects)
        {
            effect.RemoveEffect(player);
        }
        player.GetComponent<Inventory>().passiveItems.Remove(pickUpData);
    }
}

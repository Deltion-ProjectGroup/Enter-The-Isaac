using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : Interactable
{
    public int cost;
    public override void OnInteract(GameObject player)
    {
        player.GetComponent<PlayerController>().money -= cost;
        Interactable[] interactables = GetComponents<Interactable>();
        foreach(Interactable interactable in interactables)
        {
            if(interactable != this)
            {
                interactable.OnInteract(player);
            }
        }
    }
    public override void Interact(GameObject player_)
    {
        print("INTERACTOOD");
        if (canInteract && player_.GetComponent<PlayerController>().money >= cost)
        {
            print("ENUF MUNZ");
            OnInteract(player_);
        }
    }
}

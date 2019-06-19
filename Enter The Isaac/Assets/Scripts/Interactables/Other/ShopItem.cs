using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : Interactable
{
    public int cost;
    public GameObject attachedValueHolder;
    public ShopRoom shop;

    public void Initialize(int cost_, GameObject costHolder_, ShopRoom ownerRoom)
    {
        shop = ownerRoom;
        cost = cost_;
        attachedValueHolder = costHolder_;
        foreach(Interactable interactable in GetComponents<Interactable>())
        {
            interactable.canInteract = false;
        }
        canInteract = true;
    }
    public override void OnInteract(GameObject player)
    {
        shop.allItemsInRoom.Remove(gameObject);
        Destroy(attachedValueHolder);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : Interactable
{
    public int cost;
    public AudioClip buySound;
    public AudioClip cantBuySound;
    public GameObject attachedValueHolder;
    public ShopRoom shop;

    public void Initialize(int cost_, GameObject costHolder_, ShopRoom ownerRoom, AudioClip buySFX, AudioClip noBuySFX)
    {
        buySound = buySFX;
        cantBuySound = noBuySFX;
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
        SoundSpawn soundSpawner = GameObject.FindGameObjectWithTag("Manager").GetComponent<SoundSpawn>();
        print("INTERACTOOD");
        if (canInteract && player_.GetComponent<PlayerController>().money >= cost)
        {
            soundSpawner.SpawnEffect(buySound);
            print("ENUF MUNZ");
            OnInteract(player_);
        }
        else
        {
            soundSpawner.SpawnEffect(cantBuySound);
        }
    }
    public void OnDestroy()
    {
        Destroy(attachedValueHolder);
    }
}

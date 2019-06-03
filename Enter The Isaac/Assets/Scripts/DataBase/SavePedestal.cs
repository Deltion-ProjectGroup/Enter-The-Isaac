using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePedestal : Interactable
{
    public Teleporter teleporter;
    public override void OnInteract(GameObject player)
    {
        GameObject manager = GameObject.FindGameObjectWithTag("Manager");
        GameObject.FindGameObjectWithTag("Database").GetComponent<SaveDatabase>().SaveLevel(teleporter.levelToLoad);
        GameObject.FindGameObjectWithTag("Database").GetComponent<SaveDatabase>().SavePlayerData(player.GetComponent<PlayerController>(), player.GetComponent<Inventory>(), player.GetComponentInChildren<Hitbox>());
        manager.GetComponent<InGameManager>().ReturnToHub();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool canInteract = true;
    public abstract void OnInteract(GameObject player);
    public virtual void Interact(GameObject player_)
    {
        if (canInteract)
        {
            OnInteract(player_);
        }
    }
}

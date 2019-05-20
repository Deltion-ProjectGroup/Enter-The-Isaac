using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Interactable
{
    public string playerTag;
    public override void OnInteract(GameObject player)
    {
        ApplyEffects(player);
    }
    public void ApplyEffects(GameObject target_)
    {
        Effect[] effects = GetComponents<Effect>();
        foreach(Effect effect in effects)
        {
            effect.ApplyEffect(target_);
        }
        Destroy(gameObject);
    }
    public void OnTriggerEnter(Collider hit)
    {
        if(canInteract && hit.transform.tag == playerTag)
        {
            ApplyEffects(hit.gameObject.transform.parent.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatItemPickup : PassiveItemPickup
{
    public int fireChanceModifier;



    public override void Interact(GameObject player)
    {
        BuffManager buffManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<BuffManager>();
        buffManager.fireBuffChance += fireChanceModifier;
        base.Interact(player);

    }
    public override void OnDropItem(GameObject player)
    {
        BuffManager buffManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<BuffManager>();
        buffManager.fireBuffChance -= fireChanceModifier;
        base.OnDropItem(player);
    }
}

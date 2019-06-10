using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatItemPickup : PassiveItemPickup
{
    public int fireChanceModifier;



    public override void OnInteract(GameObject player)
    {
        ChanceManager buffManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChanceManager>();
        buffManager.fireBuffChance += fireChanceModifier;
        base.OnInteract(player);

    }
    public override void OnDropItem(GameObject player)
    {
        ChanceManager buffManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChanceManager>();
        buffManager.fireBuffChance -= fireChanceModifier;
        base.OnDropItem(player);
    }
}

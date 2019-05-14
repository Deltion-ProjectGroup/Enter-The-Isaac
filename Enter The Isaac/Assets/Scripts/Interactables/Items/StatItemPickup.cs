using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatItemPickup : PassiveItemPickup
{
    public int fireChanceModifier;
    public int explosionChanceModifier;
    public int healthModifier;
    public int fireRateModifier;
    public int magCapModifier;




    public override void Interact(GameObject player)
    {
        BuffManager buffManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<BuffManager>();
        buffManager.fireBuffChance += fireChanceModifier;
        buffManager.explosionBuffChance += explosionChanceModifier;
        base.Interact(player);

    }
    public override void OnDropItem(GameObject player)
    {
        BuffManager buffManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<BuffManager>();
        buffManager.fireBuffChance -= fireChanceModifier;
        buffManager.explosionBuffChance -= explosionChanceModifier;
        base.OnDropItem(player);
    }
}

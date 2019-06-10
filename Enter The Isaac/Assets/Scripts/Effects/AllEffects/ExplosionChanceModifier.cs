using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionChanceModifier : Effect
{
    public int modifyAmount;
    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<ChanceManager>().explosionBuffChance += modifyAmount;
    }
    public override void RemoveEffect(GameObject target)
    {
        target.GetComponent<ChanceManager>().explosionBuffChance -= modifyAmount;
    }
}

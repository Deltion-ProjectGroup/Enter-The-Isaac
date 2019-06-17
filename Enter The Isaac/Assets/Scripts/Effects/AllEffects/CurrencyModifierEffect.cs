using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyModifierEffect : Effect
{
    public int value;
    public override void ApplyEffect(GameObject target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        player.money += value;
    }
    public override void RemoveEffect(GameObject target)
    {
        throw new System.NotImplementedException();
    }
}

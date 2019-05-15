using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthModifyEffect : Effect
{
    public int modifyAmount;

    public override void ApplyEffect(GameObject target)
    {
        Hitbox healthData = target.GetComponentInChildren<Hitbox>();
        healthData.maxHealth += modifyAmount;
        healthData.curHealth += modifyAmount;
    }
    public override void RemoveEffect(GameObject target)
    {
        Hitbox healthData = target.GetComponentInChildren<Hitbox>();
        healthData.maxHealth -= modifyAmount;
        healthData.curHealth -= modifyAmount;
    }
}

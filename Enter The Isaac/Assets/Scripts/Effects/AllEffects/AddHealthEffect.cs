using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHealthEffect : Effect
{
    public float modifyAmount;
    public bool fakeHeart;
    public override void ApplyEffect(GameObject target)
    {
        Hitbox player = target.GetComponentInChildren<Hitbox>();
        if (fakeHeart)
        {
            player.curHealth = Mathf.Min(player.curHealth + modifyAmount, player.maxHealth);
        }
        else
        {
            player.fakeHealth += modifyAmount;
        }
    }
    public override void RemoveEffect(GameObject target)
    {
        throw new System.NotImplementedException();
    }
}

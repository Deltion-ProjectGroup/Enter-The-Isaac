using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : Effect
{
    public DOTBuffSO burnData;
    public override void ApplyEffect(GameObject target)
    {
        if (target.GetComponent<Hitbox>())
        {
            DOTBuff[] buffs = target.GetComponents<DOTBuff>();
            for(int i = 0; i < buffs.Length; i++)
            {
                DOTBuff thisBuff = buffs[i];
                if(thisBuff.dotData.type == burnData.type)
                {
                    thisBuff.StopAllCoroutines();
                    thisBuff.StartCoroutine(thisBuff.Duration(thisBuff.dotData.duration));
                    return;
                }
            }
            DOTBuff burn = target.AddComponent<DOTBuff>();
            burn.dotData = burnData;
        }
    }
    public override void RemoveEffect(GameObject target)
    {
        throw new System.NotImplementedException();
    }
}

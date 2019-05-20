using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOTBuff : DurationBuff
{
    public DOTBuffSO dotData;
    public override IEnumerator Duration(int duration)
    {
        for(int i = 0; i < duration * dotData.ticksPerSecond; i++)
        {
            print("USED TO BE DAMAGED FOR " + dotData.damagePerTick);
            yield return new WaitForSeconds(1 / dotData.ticksPerSecond);
        }
        RemoveEffect();
    }
}

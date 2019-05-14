using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOTBuff : DurationBuff
{
    public int ticksPerSecond;
    public float tickDamage;
    public override void ApplyEffect()
    {
        StartCoroutine(Duration(duration));
    }
    public override IEnumerator Duration(int duration)
    {
        for(int i = 0; i < duration * ticksPerSecond; i++)
        {
            print("USED TO BE DAMAGED FOR " + tickDamage);
            yield return new WaitForSeconds(1 / ticksPerSecond);
        }
        RemoveEffect();
    }
}

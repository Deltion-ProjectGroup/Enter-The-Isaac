using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOTBuff : DurationBuff
{
    public DOTBuffSO dotData;
    public void Start()
    {
        StartCoroutine(Duration(dotData.duration));
    }
    public override IEnumerator Duration(int duration)
    {
        for(int i = 0; i < duration * dotData.ticksPerSecond; i++)
        {
            GetComponent<Hitbox>().Hit(dotData.damagePerTick);
            yield return new WaitForSeconds(1 / dotData.ticksPerSecond);
        }
        RemoveEffect();
    }
}

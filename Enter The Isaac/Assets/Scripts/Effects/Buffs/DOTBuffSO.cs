using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DOTBuffData", menuName = "Buffs/Negative/Duration/Damage Over Time")]
public class DOTBuffSO : DurationBuffSO
{
    public int damagePerTick;
    public int ticksPerSecond;
    public DOTType type;

    public enum DOTType { Poison, Burn}
}

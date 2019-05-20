using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public DOTBuffSO fireBuffData;
    public int fireBuffChance;
    public int explosionBuffChance;
    public GameObject explosion;

    public void ApplyBuffEffects(GameObject target)
    {
        int randomChance = Random.Range(1, 101);
        if(randomChance <= fireBuffChance)
        {
            BurnEffect burnEffect = target.AddComponent<BurnEffect>();
            burnEffect.burnData = fireBuffData;
        }
        randomChance = Random.Range(1, 101);
        if (randomChance <= explosionBuffChance)
        {
            ExplosionEffect explEffect = target.AddComponent<ExplosionEffect>();
            explEffect.explosion = explosion;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : Effect
{
    public GameObject explosion;
    public override void ApplyEffect(GameObject target)
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        print("SPAWNED EXPL");
    }
    public override void RemoveEffect(GameObject target)
    {
        throw new System.NotImplementedException();
    }
}

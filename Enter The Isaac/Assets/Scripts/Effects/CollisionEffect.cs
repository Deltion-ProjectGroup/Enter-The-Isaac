using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEffect : MonoBehaviour
{
    public LayerMask possibleTargets;
    public void OnTriggerEnter(Collider other)
    {
        print("I HIT SUMTHING");
        if(other.gameObject.GetComponent<Hitbox>())
        {
            print("I HIT RITE LAYER");
            Effect[] effectsToApply = GetComponents<Effect>();
            foreach(Effect effect in effectsToApply)
            {
                effect.ApplyEffect(other.gameObject);
            }
        }
    }
}

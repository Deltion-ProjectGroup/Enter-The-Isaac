using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEffect : MonoBehaviour
{
    public LayerMask possibleTargets;
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == possibleTargets)
        {
            Effect[] effectsToApply = GetComponents<Effect>();
            foreach(Effect effect in effectsToApply)
            {
                effect.ApplyEffect(other.gameObject);
            }
        }
    }
}

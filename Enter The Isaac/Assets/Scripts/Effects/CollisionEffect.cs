using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEffect : MonoBehaviour
{
    public LayerMask possibleTargets;
    public UnityEvent nonEffectActivations;
    public void OnTriggerEnter(Collider other)
    {
        //print("I HIT SUMTHING");
        if(other.gameObject.GetComponent<Hitbox>() && other.gameObject.tag == "Player")
        {
            print("I HIT RITE LAYER");
            Effect[] effectsToApply = GetComponents<Effect>();
            foreach(Effect effect in effectsToApply)
            {
                effect.ApplyEffect(other.GetComponentInParent<PlayerController>().gameObject);
            }
            nonEffectActivations.Invoke();
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEffect : MonoBehaviour
{
    public LayerMask possibleTargets;
    public UnityEvent nonEffectActivations;
    public bool DestroyOnTrigger = true;
    public void OnTriggerEnter(Collider other)
    {
        //print("I HIT SUMTHING");
        print(other.gameObject);
        if(other.gameObject.GetComponentInChildren<Hitbox>() && other.gameObject.tag == "Player")
        {
            print("I HIT RITE LAYER");
            Effect[] effectsToApply = GetComponents<Effect>();
            foreach(Effect effect in effectsToApply)
            {
                effect.ApplyEffect(other.GetComponentInParent<PlayerController>().gameObject);
            }
            nonEffectActivations.Invoke();
            if (DestroyOnTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}

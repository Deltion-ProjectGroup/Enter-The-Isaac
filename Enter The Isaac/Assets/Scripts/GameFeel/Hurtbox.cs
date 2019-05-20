using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Rigidbody))]

public class Hurtbox : MonoBehaviour
{

    public float damage = 1;
    public int team = 0;
    public bool destroyOnHit = true;
    [SerializeField] UnityEvent hitEvent;
    [SerializeField] new bool enabled = true; // enabled is used for the grenade launchers normal bullets. The explosion inherits the damage.
    [SerializeField] bool knockBackForward = true;
    [SerializeField] float knockBackMultiply = 1;

    void OnTriggerEnter(Collider other)
    {
        Hitbox hit = other.GetComponent<Hitbox>();
        if (hit != null)
        {
            if (team != hit.team)
            {
                if (enabled == true)
                {
                    hit.hurtboxKnockBackMultiplier = knockBackMultiply;
                    hit.Hit(damage);
                    if (knockBackForward == true)
                    {
                        hit.impactDir = transform.forward;
                    }
                    else
                    {
                        hit.impactDir = transform.position - hit.lastPos;
                    }
                    hit.impactDir.y = 0;
                }
                hitEvent.Invoke();

                if (destroyOnHit == true)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}

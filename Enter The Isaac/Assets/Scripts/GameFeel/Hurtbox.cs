using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Rigidbody))]

public class Hurtbox : MonoBehaviour
{

    public float damage = 1;
    public int team = 0;
    [SerializeField] bool destroyOnHit = true;
    [SerializeField] UnityEvent hitEvent;

    void OnTriggerEnter(Collider other)
    {
        Hitbox hit = other.GetComponent<Hitbox>();
        if (hit != null)
        {
            if (team != hit.team)
            {
                hit.Hit(damage);
                hitEvent.Invoke();
                if (destroyOnHit == true)
                {
                    Destroy(transform.root.gameObject);
                }
            }
        }
    }
}

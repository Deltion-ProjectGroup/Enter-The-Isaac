using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour
{
    public float curHealth = 3;
    public int team = 0;
    float maxHealth = 1;
    public HitEvent hitEvent;
    [SerializeField] float invincibleTime = 0f;
    public UnityEvent deathEvent;
    [SerializeField] EventArray[] timedEvents;
    [HideInInspector] public Vector3 impactDir = Vector3.zero; //see hurtbox for more
    [SerializeField] float knockBack = 0;
    [SerializeField] Transform knockBackTrans;
    [SerializeField] float knockbackWaitTime = 0.01f;
    [HideInInspector] public Vector3 lastPos;
    void Start()
    {
        StartStuff();
    }

    public void StartStuff()
    {
        maxHealth = curHealth;
        if (knockBackTrans == null)
        {
            knockBackTrans = transform;
        }
    }

    void LateUpdate()
    {
        if (IsInvoking("InvokedKnockback") == false)
        {
            lastPos = transform.position;
        }
    }
    public virtual void Hit(float damage)
    {
        if (IsInvoking("Invincible") == false)
        {
            Invoke("InvokedKnockback", knockbackWaitTime);
            curHealth -= damage;
            HealthEvent(curHealth / maxHealth * 100);
            hitEvent.Invoke(curHealth / maxHealth * 100);
            if (curHealth <= 0)
            {
                Die();
            }
            else
            {
                Invoke("Invincible", invincibleTime);
                float totalTime = 0;
                for (int i = 0; i < timedEvents.Length; i++)
                {
                    totalTime += timedEvents[i].nextEvent;
                    StartCoroutine(TimedEvents(timedEvents[i].curEvent, totalTime));
                }
            }
        }

    }

    void InvokedKnockback()
    {
        Knockback(knockBack);
    }

    public void Knockback(float str)
    {
        if (knockBackTrans.GetComponent<PlayerController>() == null)
        {
            RaycastHit _hit;
            if (Physics.Raycast(knockBackTrans.position, -impactDir.normalized, out _hit, str))
            {
                knockBackTrans.position -= impactDir.normalized * (Vector3.Distance(knockBackTrans.position, _hit.point) / 2);
            }
            else
            {
                knockBackTrans.position -= impactDir.normalized * str;
            }
        }
        else
        {
            knockBackTrans.GetComponent<PlayerController>().moveV3 -= impactDir.normalized * str * 5;
        }
    }

    public void AddHealth(int toAdd)
    {
        curHealth += toAdd;
        curHealth = Mathf.Min(curHealth, maxHealth);
    }

    IEnumerator TimedEvents(UnityEvent ev, float time)
    {
        yield return new WaitForSeconds(time);
        ev.Invoke();
    }

    void Invincible()
    {
        //invoke function
    }

    public void HealthEvent(float value)
    {
        hitEvent.RemoveAllListeners();
        hitEvent.AddListener(HealthEvent);
    }



    public virtual void Die()
    {
        //  Destroy(transform.root.gameObject);
        deathEvent.Invoke();
    }
}

[System.Serializable]
public class HitEvent : UnityEvent<float> { }

[System.Serializable]
public class EventArray
{
    public float nextEvent = 1;
    public UnityEvent curEvent;
}

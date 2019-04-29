using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour
{
    [SerializeField] float curHealth = 3;
    public int team = 0;
    float maxHealth = 1;
    public HitEvent hitEvent;
    [SerializeField] float invincibleTime = 0f;
    public UnityEvent deathEvent;
    [SerializeField] EventArray[] timedEvents;
    public Vector3 impactDir = Vector3.zero; //see hurtbox for more
    [SerializeField] float knockBack = 0;
    void Start()
    {
        StartStuff();
    }

    public void StartStuff()
    {
        maxHealth = curHealth;
    }
    public virtual void Hit(float damage)
    {
        if (IsInvoking("Invincible") == false)
        {
            Invoke("Knockback",0.01f);
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

    void Knockback()
    {
        RaycastHit _hit;
        if (Physics.Raycast(transform.position, -impactDir.normalized, out _hit, knockBack))
        {
            transform.position -= impactDir.normalized * (Vector3.Distance(transform.position, _hit.point) / 2);
        }
        else
        {
            transform.position -= impactDir.normalized * knockBack;
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

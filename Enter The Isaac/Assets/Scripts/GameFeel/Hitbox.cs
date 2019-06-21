using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Hitbox : MonoBehaviour {
    public float curHealth = 3;
    public float fakeHealth = 0;
    public int team = 0;
    [HideInInspector] public float maxHealth = 1;
    [SerializeField] float maxHPOverride = 0;
    public HitEvent hitEvent;
    [SerializeField] float invincibleTime = 0f;
    public UnityEvent deathEvent;
    [SerializeField] EventArray[] timedEvents;
    [HideInInspector] public Vector3 impactDir = Vector3.zero; //see hurtbox for more
    [SerializeField] float knockBack = 0;
    [HideInInspector] public float hurtboxKnockBackMultiplier = 1;
    [SerializeField] Transform knockBackTrans;
    [SerializeField] float knockbackWaitTime = 0.01f;
    [HideInInspector] public Vector3 lastPos;
    [SerializeField] bool destoy = false;
    bool dead = false;

    public delegate void VoidDelegate ();
    public delegate void GameObjectDelegate (GameObject thisObject);
    public GameObjectDelegate onDeath;
    public VoidDelegate onHit;

    void Start () {
        StartStuff ();
    }

    public void StartStuff () {
        if (maxHPOverride == 0) {
            maxHealth = curHealth;
        } else {
            maxHealth = maxHPOverride;
        }
        if (knockBackTrans == null) {
            knockBackTrans = transform;
        }
    }

    void LateUpdate () {
        if (IsInvoking ("InvokedKnockback") == false) {
            lastPos = transform.position;
        }
    }
    public virtual void Hit (float damage) {
        if (IsInvoking ("Invincible") == false) {
            Invoke ("InvokedKnockback", knockbackWaitTime);
            if (fakeHealth < 0) {
                curHealth -= damage;
            } else {
                curHealth -= Mathf.Max (0, damage - fakeHealth);
                fakeHealth = Mathf.Max (0, fakeHealth - damage);
            }
            HealthEvent (curHealth / maxHealth * 100);
            hitEvent.Invoke (curHealth / maxHealth * 100);
            if (curHealth <= 0) {
                Die ();
            } else {
                if (onHit != null) {
                    onHit ();
                }
                if (invincibleTime > 0) {
                    Invoke ("Invincible", invincibleTime);
                }
                float totalTime = 0;
                for (int i = 0; i < timedEvents.Length; i++) {
                    totalTime += timedEvents[i].nextEvent;
                    StartCoroutine (TimedEvents (timedEvents[i].curEvent, totalTime));
                }
            }
        }

    }

    void InvokedKnockback () {
        Knockback (knockBack * hurtboxKnockBackMultiplier);
    }

    public void Knockback (float str) {
        if (knockBackTrans != null && knockBackTrans.GetComponent<PlayerController> () == null) {
            RaycastHit _hit;
            if (Physics.Raycast (knockBackTrans.position, -impactDir.normalized, out _hit, str)) {
                knockBackTrans.position -= impactDir.normalized * (Vector3.Distance (knockBackTrans.position, _hit.point) / 2);
            } else {
                knockBackTrans.position -= impactDir.normalized * str;
            }
        } else if (knockBackTrans != null) {
            knockBackTrans.GetComponent<PlayerController> ().moveV3 -= impactDir.normalized * str * 5;
        }
        hurtboxKnockBackMultiplier = 1;
    }

    public void AddHealth (int toAdd) {
        curHealth += toAdd;
        curHealth = Mathf.Min (curHealth, maxHealth);
    }

    IEnumerator TimedEvents (UnityEvent ev, float time) {
        yield return new WaitForSeconds (time);
        ev.Invoke ();
    }

    void Invincible () {
        GetComponent<Collider> ().enabled = false;
        GetComponent<Collider> ().enabled = true;
    }

    public void HealthEvent (float value) {
        hitEvent.RemoveAllListeners ();
        hitEvent.AddListener (HealthEvent);
    }

    public virtual void Die () {
        //  Destroy(transform.root.gameObject);
       // if (!dead) {
            deathEvent.Invoke ();
            if (onDeath != null) {
                onDeath (gameObject);
            }
            if (destoy == true) {
                Destroy (gameObject);
            }
         //   dead = true;
        //}
    }
}

[System.Serializable]
public class HitEvent : UnityEvent<float> { }

[System.Serializable]
public class EventArray {
    public float nextEvent = 1;
    public UnityEvent curEvent;
}
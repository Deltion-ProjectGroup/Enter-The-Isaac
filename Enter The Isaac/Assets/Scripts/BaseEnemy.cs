using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{

    public enum State
    {
        Spawn,
        Die,
        Attack,
        Move,
        Idle
    }
    public State curState;
    [SerializeField] Hitbox hitbox;
    float timer = 0;
    [Header("Spawning")]
    [SerializeField] Renderer[] spawnInvisible;
    [SerializeField] GameObject spawnParticle;
    [SerializeField] bool skipParticle = false;
    void Start()
    {
        if (skipParticle == false)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        for (int i = 0; i < spawnInvisible.Length; i++)
        {
            spawnInvisible[i].enabled = false;
        }
        curState = State.Spawn;
        hitbox.enabled = false;
        Instantiate(spawnParticle, transform.position, spawnParticle.transform.rotation);
        Invoke("AfterSpawnComplete", 3.2f);
    }

    void AfterSpawnComplete()
    {
        hitbox.enabled = true;
        for (int i = 0; i < spawnInvisible.Length; i++)
        {
            spawnInvisible[i].enabled = true;
        }
        curState = State.Move;
    }

    void Update()
    {
        switch (curState)
        {
            case State.Move:
                Move();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    public virtual void Move()
    {
        curState = State.Attack;
    }

    public virtual void Attack()
    {

    }

    public void DieEvent()
    {
        if (GetComponent<Hitbox>().impactDir != Vector3.zero)
        {
            transform.LookAt(transform.position + GetComponent<Hitbox>().impactDir);
        }
        hitbox.enabled = false;
        timer += Time.deltaTime;
        if (timer > 0.1f)
        {
            transform.position -= transform.forward * 30 * Time.deltaTime;
        }
        if (timer > 0.25f)
        {
            Destroy(gameObject);
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public enum PathMethod
    {
        GoToPlayer,
        KeepDistanceFromPlayer,
        DontMove
    }
    [Header("PathFinding")]
    [SerializeField] float distanceFromPlayer = 10;
    public PathMethod myPathMethod = PathMethod.GoToPlayer;
    NavMeshAgent agent;
    Transform player;
    //animation
    Animator anim;
    Vector3 lastPos;
    float walkWaitTime = 1;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>().transform;
        anim = GetComponent<Animator>();
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
                SetWalkAnim();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    void SetWalkAnim()
    {
        if (agent.velocity.magnitude > 0)
        {
            anim.SetBool("walking", true);
        }
        else
        {
            anim.SetBool("walking", false);
            lastPos = transform.position;
        }
    }

    public virtual void Move()
    {
        switch (myPathMethod)
        {
            case PathMethod.GoToPlayer:
                GoToPlayer();
                agent.isStopped = (Vector3.Distance(player.position, transform.position) < distanceFromPlayer);
                break;

            case PathMethod.KeepDistanceFromPlayer:
                KeepDistanceFromPlayer();
                break;

            case PathMethod.DontMove:
                //yeah well, dont move
                break;
        }
    }

    void KeepDistanceFromPlayer()
    {

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > distanceFromPlayer - (distanceFromPlayer / 10) && distance < distanceFromPlayer + (distanceFromPlayer / 10))//if i'm close    if(5 units away from player > 10 / 10 = 1)
        {
            //dont move
            walkWaitTime = 0.2f;
            agent.isStopped = true;
        }
        else if (walkWaitTime <= 0)
        {
            agent.isStopped = false;
            GoAwayFromPlayer();
        }
        walkWaitTime = Mathf.Max(-0.001f, walkWaitTime - Time.deltaTime);

    }

    void GoToPlayer()
    {
        agent.SetDestination(player.position);
        if (agent.isStopped == false)
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }

    void GoAwayFromPlayer()
    {
        Vector3 awayFromPlayerDir = (transform.position - player.position).normalized;
        awayFromPlayerDir.y = 0;
        agent.SetDestination(player.position + (awayFromPlayerDir * distanceFromPlayer));
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    public virtual void Attack()
    {
        // agent.path.corners.
    }

    public void DieEvent()
    {
        curState = State.Die;
        agent.enabled = false;
        if (IsInvoking("DieEvent") == false)
        {
            Invoke("DieEvent", Time.deltaTime);
        }
        if (GetComponent<Hitbox>().impactDir != Vector3.zero)
        {
            transform.LookAt(transform.position + GetComponent<Hitbox>().impactDir);
        }
        if (hitbox.enabled == true)
        {
            Camera.main.GetComponent<Shake>().SmallShake();
            hitbox.enabled = false;
        }
        timer += Time.deltaTime;
        if (timer < 0.05f)
        {
            transform.position -= transform.forward * 100 * Time.deltaTime;
        }
        if (timer > 0.25f)
        {
            Camera.main.GetComponent<Shake>().SmallShake();
            Camera.main.fieldOfView = 60;
            Destroy(gameObject);
        }
    }
}
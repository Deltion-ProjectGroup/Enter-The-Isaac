using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{

    public enum State
    {
        Spawn,
        Die,
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
    public delegate void VoidDelegate();
    public VoidDelegate onDeath;
    NavMeshAgent agent;
    Transform player;
    //animation
    Animator anim;
    Vector3 lastPos;
    float walkWaitTime = 1;
    [Header("Attacking")]
    [SerializeField] bool canAttackWhileWalking = true;
    public enum AttackType
    {
        Repeat,
        RepeatOnSight,
        Close,
        ResponseFire,
        OnDeath,
        Overridable
    }
    [SerializeField] AttackType[] attackType;
    [SerializeField] float repeatRate = 1;

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
        }
        CheckAttack();
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

    //ATACKING RELATED
    public virtual void Attack()
    {
        print("attack b!tch");
    }

    void CheckAttack()
    {
        bool helper = false;
        if (canAttackWhileWalking == false && anim.GetBool("walking") == true)
        {
            helper = true;
        }
        if (helper == false)
        {
            for (int i = 0; i < attackType.Length; i++)
            {
                switch (attackType[i])
                {
                    case AttackType.Repeat:
                        AttackRepeat();
                        break;
                    case AttackType.RepeatOnSight:
                        AttackRepeatOnSight();
                        break;
                    case AttackType.ResponseFire:
                        AttackResponseFire();
                        break;
                    case AttackType.Close:
                        AttackClose();
                        break;
                    case AttackType.OnDeath:
                        AttackOnDeath();
                        break;
                    case AttackType.Overridable:
                        AttackOverridable();
                        break;
                }
            }
        }
    }

    void AttackRepeat()
    {
        if (IsInvoking("AttackRepeatFunction") == false)
        {
            InvokeRepeating("AttackRepeatFunction", repeatRate, repeatRate);
        }
    }

    void AttackRepeatOnSight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0,0.15f,0), player.position - transform.position, out hit, Vector3.Distance(transform.position, player.position)))
        {
            if (hit.transform == player || hit.transform.parent == player)
            {
                if (IsInvoking("AttackRepeatFunction") == false)
                {
                    InvokeRepeating("AttackRepeatFunction", repeatRate, repeatRate);
                }
            }
            else
            {
                CancelInvoke("AttackRepeatFunction");
            }
        }
        else
        {
            CancelInvoke("AttackRepeatFunction");
        }
    }
    void AttackRepeatFunction()
    {
        //basically this is the actual called function
        Attack();
    }

    void AttackResponseFire()
    {
        if (player != null)
        {
            PlayerController playCon = player.GetComponent<PlayerController>();
            if (playCon.gun != null && playCon.gun.IsInvoking("Shoot") == true)
            {
                if (IsInvoking("AttackRepeatFunction") == false)
                {
                    InvokeRepeating("AttackRepeatFunction", repeatRate, repeatRate);
                }
            }
            else
            {
                CancelInvoke("AttackRepeatFunction");
            }
        }
        else
        {
            CancelInvoke("AttackRepeatFunction");
        }
    }

    void AttackClose()
    {
        if (Vector3.Distance(transform.position, player.position) < distanceFromPlayer)
        {
            if (IsInvoking("AttackRepeatFunction") == false)
            {
                InvokeRepeating("AttackRepeatFunction", repeatRate, repeatRate);
            }
        }
        else
        {
            CancelInvoke("AttackRepeatFunction");
        }
    }
    void AttackOnDeath()
    {
        if (curState == State.Die)
        {
            if (IsInvoking("AttackRepeatFunction") == false)
            {
                InvokeRepeating("AttackRepeatFunction", 0, repeatRate);
            }
        }
    }

    public virtual void AttackOverridable()
    {
        //for inheritance
    }
}
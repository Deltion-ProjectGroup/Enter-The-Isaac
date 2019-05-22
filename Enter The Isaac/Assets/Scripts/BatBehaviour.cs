using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehaviour : MonoBehaviour
{

    [SerializeField] Transform home;
    [SerializeField] float homeRadius = 20;
    public enum State
    {
        Idle,
        FlyToTarget,
        Attack,
        Die,
        Spawn
    }
    [SerializeField] State curState = State.Idle;
    Transform player;
    Animator anim;
    float timer = 0;
    Vector3 goalPos;
    [SerializeField] float noticePlayerTime = 1;
    [SerializeField] float moveToTargetSpeed = 10;
    Vector3 lastSpottedPos;
    [SerializeField] float attackDistance = 2;
    [SerializeField] float attackChargeTime = 0.5f;
    [SerializeField] float attackGoForwardSpeed = 30;
    [SerializeField] Sine sine;
    [SerializeField] GameObject spawnParticle;
    [SerializeField] Renderer[] spawnInvisible;
    Collider hitbox;
    [SerializeField] GameObject hurtbox;
    [SerializeField] bool skipSpawnParticle = false;
    int attackPhase = 0;
    Vector3 startRot;

    void Start()
    {
        transform.position += transform.up;
        anim = transform.GetComponentInChildren<Animator>();
        startRot = transform.eulerAngles;
        hitbox = GetComponent<Collider>();
        if (skipSpawnParticle == false)
        {
            curState = State.Spawn;
            StartCoroutine(SpawnEvents());
        }
        player = FindObjectOfType<PlayerController>().transform;
        if (home == null)
        {
            home = new GameObject().transform;
            home.position = transform.position;
        }
        sine = GetComponent<Sine>();
        InvokeRepeating("SetNewIdlePos", 2, 5);
        goalPos = home.position;
        hurtbox.SetActive(false);
    }

    void Update()
    {
        switch (curState)
        {
            case State.Idle:
                transform.eulerAngles = startRot;
                LookForTarget();
                IdleMove();
                sine.speed = 1f;
                break;
            case State.FlyToTarget:
                sine.speed = 5f;
                MoveToTarget();
                break;
            case State.Attack:
                sine.speed = 0;
                Attack();
                break;
        }

        transform.Rotate(-transform.localEulerAngles.x, 0, 0);
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("GetHit") == true && curState == State.Attack)
        {
            timer = 0;
            StopAllCoroutines();
            curState = State.Idle;
        }
    }

    IEnumerator SpawnEvents()
    {
        Instantiate(spawnParticle, transform.position, spawnParticle.transform.rotation);
        for (int i = 0; i < spawnInvisible.Length; i++)
        {
            spawnInvisible[i].enabled = false;
        }
        hitbox.enabled = false;
        yield return new WaitForSeconds(3.2f);
        for (int i = 0; i < spawnInvisible.Length; i++)
        {
            spawnInvisible[i].enabled = true;
        }
        hitbox.enabled = true;
        curState = State.Idle;
    }

    public void Die()
    {
        curState = State.Die;
        timer = 0;
        if (GetComponent<Hitbox>().impactDir != Vector3.zero)
        {
            transform.LookAt(transform.position + GetComponent<Hitbox>().impactDir);
        }
        InvokeRepeating("DieEvent", 0, Time.deltaTime);
    }

    void DieEvent()
    {
        if (GetComponent<Hitbox>().impactDir != Vector3.zero)
        {
            transform.LookAt(transform.position + GetComponent<Hitbox>().impactDir);
        }
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

    void LookForTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), player.position - transform.position, out hit, Vector3.Distance(transform.position, player.position), ~LayerMask.GetMask("Ignore Raycast", "Enemy")))
        {
            if (hit.transform.tag == "Player")
            {
                timer += Time.deltaTime;
                if (timer >= noticePlayerTime)
                {
                    timer = 0;
                    curState = State.FlyToTarget;
                }
            }
            else
            {
                timer = Mathf.Max(0, timer - Time.deltaTime);
            }
        }
        else
        {
            timer = Mathf.Max(0, timer - Time.deltaTime);
        }
    }

    void IdleMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, goalPos, Time.deltaTime * moveToTargetSpeed / 3);
    }

    void SetNewIdlePos()
    {
        home.eulerAngles = new Vector3(0, Random.RandomRange(0, 359.9f), 0);
        goalPos = home.position + home.forward * Random.Range(homeRadius / 10, homeRadius);
        if (Physics.Raycast(transform.position, goalPos, Vector3.Distance(transform.position, goalPos)))
        {
            goalPos = home.position;
        }
    }

    void MoveToTarget()
    {
        bool exeptionBool = false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.position - transform.position, out hit, Vector3.Distance(transform.position, player.position)))
        {
            if (hit.transform.tag == "Player")
            {
                transform.LookAt(player.position);
                transform.Rotate(0, 180, 0);
                lastSpottedPos = player.position;
                timer = Mathf.Max(0, timer - Time.deltaTime);
            }
            else
            {
                exeptionBool = true;
                sine.speed = 2;
                timer += Time.deltaTime;
            }
        }
        else
        {
            timer += Time.deltaTime;
        }
        if (timer > 3)
        {
            curState = State.Idle;
        }
        if (exeptionBool == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * moveToTargetSpeed);
        }

        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            timer = 0;
            curState = State.Attack;
            StopAllCoroutines();
            StartCoroutine("AttackEvents");
        }
    }

    IEnumerator AttackEvents()
    {
        //aim
        attackPhase = 0;
        anim.Play("metarig|Attack(Charge)", 0);
        yield return new WaitForSeconds(attackChargeTime);
        //fire
        attackPhase = 1;
        hurtbox.SetActive(true);
        transform.Find("Line").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        //stop
        anim.Play("metarig|Attack(Attack)", 0);
        hurtbox.SetActive(false);
        attackPhase = 2;
        transform.Find("Line").gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        //idle
        attackPhase = 0;
        curState = State.Idle;
    }

    public void SeePlayer()
    {
        //used when getting hit, so that the enemy isn't an idiot
        timer = 0;
        curState = State.FlyToTarget;
        lastSpottedPos = player.position;
    }

    void Attack()
    {
        switch (attackPhase)
        {
            case 0:
                if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Charge") == false)
                {
                    transform.LookAt(player.position);
                    transform.Rotate(0, 180, 0);
                }
                break;
            case 1:
                transform.position += -transform.forward * Time.deltaTime * attackGoForwardSpeed;
                break;
            case 2:
                anim.Play("metarig|Flying", 0);
                //maybe set an animation later i dunno
                break;
        }
    }
}

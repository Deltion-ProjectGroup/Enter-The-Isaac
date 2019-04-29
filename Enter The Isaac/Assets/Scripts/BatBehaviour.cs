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
        Die
    }
    [SerializeField] State curState = State.Idle;
    Transform player;
    float timer = 0;
    [SerializeField] float noticePlayerTime = 1;
    [SerializeField] float moveToTargetSpeed = 10;
    Vector3 lastSpottedPos;
    [SerializeField] float attackDistance = 2;
    [SerializeField] Sine sine;

    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        if (home == null)
        {
            home = new GameObject().transform;
            home.position = transform.position;
        }
        sine = GetComponent<Sine>();
    }

    void Update()
    {
        switch (curState)
        {
            case State.Idle:
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
        Debug.DrawRay(transform.position, player.position - transform.position, Color.red, Time.deltaTime);
        if (Physics.Raycast(transform.position, player.position - transform.position, out hit, Vector3.Distance(transform.position, player.position)))
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

    }

    void MoveToTarget()
    {
        bool exeptionBool = false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.position - transform.position, out hit, Vector3.Distance(transform.position, player.position)))
        {
            if (hit.transform.tag == "Player")
            {
                lastSpottedPos = player.position;
                timer = Mathf.Max(0, timer - Time.deltaTime);
            }
            else
            {
                exeptionBool = true;
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
        }
    }

    public void SeePlayer(){
        //used when getting hit, so that the enemy isn't an idiot
        timer = 0;
        curState = State.FlyToTarget;
        lastSpottedPos = player.position;
    }

    void Attack()
    {
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            print(transform.name + " ATTACKED! Not very effective though..");
            timer = 0;
            curState = State.Idle;
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBehaviour : MonoBehaviour {
    public enum State {
        Idle,
        FlyToTarget,
        Attack,
        Die,
        Spawn,
        Bonk
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
    SoundSpawn soundSpawner;
    [SerializeField] AudioClip chargeSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip getHitSound;
    [SerializeField] AudioClip idleSound;
    [SerializeField] float flapRepeatSpeed = 0.4f;

    void Start () {
        soundSpawner = FindObjectOfType<SoundSpawn> ();
        RaycastHit hit;
        if (Physics.Raycast (transform.position, -Vector3.up, out hit, 3, LayerMask.GetMask ("Default"))) {
            transform.position = hit.point + (Vector3.up / 10);
        }
        transform.position += transform.up;
        anim = transform.GetComponentInChildren<Animator> ();
        startRot = transform.eulerAngles;
        hitbox = GetComponent<Collider> ();
        if (skipSpawnParticle == false) {
            curState = State.Spawn;
            StartCoroutine (SpawnEvents ());
        }
        player = FindObjectOfType<PlayerController> ().transform;
        sine = GetComponent<Sine> ();
        InvokeRepeating ("SetNewIdlePos", 2, 5);
        hurtbox.SetActive (false);
    }

    void Update () {
        switch (curState) {
            case State.Idle:
                transform.eulerAngles = startRot;
                LookForTarget ();
                IdleMove ();
                sine.speed = 1f;
                PlayFlapSound ();
                break;
            case State.FlyToTarget:
                sine.speed = 5f;
                MoveToTarget ();
                PlayFlapSound ();
                break;
            case State.Attack:
                sine.speed = 0;
                Attack ();
                break;
        }

        transform.Rotate (-transform.localEulerAngles.x, 0, 0);
        if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("GetHit") == true && curState == State.Attack) {
            timer = 0;
            StopAllCoroutines ();
            curState = State.Idle;
            hurtbox.SetActive (false);
            attackPhase = 0;
        }
    }

    void PlayFlapSound () {
        if (IsInvoking ("ActualFlapSound") == false) {
            Invoke ("ActualFlapSound", flapRepeatSpeed);
        }

        //also a rotation bug fix
        if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("Attack")) {
            anim.Play ("metarig|Flying", 0);
        }
    }

    void ActualFlapSound () {
        if(soundSpawner != null){
        soundSpawner.SpawnEffect (idleSound, 1, 1, 1, transform);
        }
    }

    IEnumerator SpawnEvents () {
        Instantiate (spawnParticle, transform.position - transform.up, spawnParticle.transform.rotation);
        for (int i = 0; i < spawnInvisible.Length; i++) {
            spawnInvisible[i].enabled = false;
        }
        hitbox.enabled = false;
        yield return new WaitForSeconds (3.2f);
        for (int i = 0; i < spawnInvisible.Length; i++) {
            spawnInvisible[i].enabled = true;
        }
        hitbox.enabled = true;
        curState = State.Idle;
    }

    public void Die () {
        hitbox.gameObject.SetActive(false);
        StopAllCoroutines ();
        CancelInvoke ();
        curState = State.Die;
        timer = 0;
        if(soundSpawner != null){
        soundSpawner.SpawnEffect (deathSound);
        }
        if (GetComponent<Hitbox> ().impactDir != Vector3.zero) {
            transform.LookAt (transform.position + GetComponent<Hitbox> ().impactDir);
        }
        InvokeRepeating ("DieEvent", 0, Time.deltaTime);
    }

    void DieEvent () {
        if (GetComponent<Hitbox> ().impactDir != Vector3.zero) {
            transform.LookAt (transform.position + GetComponent<Hitbox> ().impactDir);
        }
        timer += Time.deltaTime;
        if (timer > 0.1f) {
            transform.position -= transform.forward * 30 * Time.deltaTime;
        }
        if (timer > 0.25f) {
            GameObject dropToSpawn = GameObject.FindGameObjectWithTag ("Manager").GetComponent<ChanceManager> ().enemyDropPool.GetInstanceFromPool ();
            if (dropToSpawn) {
                Instantiate (dropToSpawn, transform.position, Quaternion.identity);
            }
            Destroy (gameObject);
        }
    }

    void LookForTarget () {
        RaycastHit hit;
        if (Physics.Raycast (transform.position + new Vector3 (0, 0.2f, 0), player.position - transform.position, out hit, Vector3.Distance (transform.position, player.position), ~LayerMask.GetMask ("Ignore Raycast", "Enemy"))) {
            if (hit.transform.tag == "Player") {
                timer += Time.deltaTime;
                if (timer >= noticePlayerTime) {
                    timer = 0;
                    curState = State.FlyToTarget;
                }
            } else {
                timer = Mathf.Max (0, timer - Time.deltaTime);
            }
        } else {
            timer = Mathf.Max (0, timer - Time.deltaTime);
        }
    }

    public void GetHit () {
        hitbox.enabled = true;
        if(soundSpawner != null){
        soundSpawner.SpawnEffect (getHitSound,1,Random.Range(0.95f,1.05f),0,transform);
        }
    }

    void IdleMove () {
        transform.position = Vector3.MoveTowards (transform.position, goalPos, Time.deltaTime * moveToTargetSpeed / 3);
    }

    void SetNewIdlePos () {
        goalPos = transform.position;
    }

    void MoveToTarget () {
        bool exeptionBool = false;
        RaycastHit hit;
        if (Physics.Raycast (transform.position, player.position - transform.position, out hit, Vector3.Distance (transform.position, player.position))) {
            if (hit.transform.tag == "Player") {
                transform.LookAt (player.position);
                transform.Rotate (0, 180, 0);
                lastSpottedPos = player.position;
                timer = Mathf.Max (0, timer - Time.deltaTime);
            } else {
                exeptionBool = true;
                sine.speed = 2;
                timer += Time.deltaTime;
            }
        } else {
            timer += Time.deltaTime;
        }
        if (timer > 3) {
            curState = State.Idle;
        }
        if (exeptionBool == false) {
            transform.position = Vector3.MoveTowards (transform.position, player.position, Time.deltaTime * moveToTargetSpeed);
        }

        if (Vector3.Distance (transform.position, player.position) < attackDistance) {
            timer = 0;
            curState = State.Attack;
            StopAllCoroutines ();
            StartCoroutine ("AttackEvents");
        }
    }

    IEnumerator AttackEvents () {
        //aim
        attackPhase = 0;
        anim.Play ("metarig|Attack(Charge)", 0);
        yield return new WaitForSeconds (attackChargeTime);
        //fire
        attackPhase = 1;
        hurtbox.SetActive (true);
        transform.Find ("Line").gameObject.SetActive (true);
        if(soundSpawner != null){
        soundSpawner.SpawnEffect (chargeSound, 1, 1, 1, transform);
        }
        yield return new WaitForSeconds (0.3f);
        //stop
        anim.Play ("metarig|Attack(Attack)", 0);
        hurtbox.SetActive (false);
        attackPhase = 2;
        transform.Find ("Line").gameObject.SetActive (false);
        yield return new WaitForSeconds (0.5f);
        //idle
        attackPhase = 0;
        curState = State.Idle;
    }

    public void SeePlayer () {
        //used when getting hit, so that the enemy isn't an idiot
        timer = 0;
        curState = State.FlyToTarget;
        lastSpottedPos = player.position;
    }

    void Attack () {
        switch (attackPhase) {
            case 0:
                if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("Charge") == false) {
                    transform.LookAt (player.position);
                    transform.Rotate (0, 180, 0);
                }
                break;
            case 1:
                Vector3 lastPos = transform.position + new Vector3 (0, 0.2f, 0);
                transform.position += -transform.forward * Time.deltaTime * attackGoForwardSpeed;
                //dont forget  + new Vector3(0, 0.2f, 0)
                if (Physics.Raycast (lastPos, (transform.position + new Vector3 (0, 0.2f, 0)) - lastPos, Vector3.Distance ((transform.position + new Vector3 (0, 0.2f, 0)), lastPos), ~LayerMask.GetMask ("Ignore Raycast", "Enemy", "SpecialRender", "Player","Bullet","EnemyBullet"),QueryTriggerInteraction.Ignore)) {
                    curState = State.Bonk;
                    attackPhase = 0;
                    timer = 0;
                    anim.Play ("metarig|Flinch", 0);
                    StopCoroutine ("AttackEvents");
                    transform.Find ("Line").gameObject.SetActive (false);
                    transform.position = lastPos;
                    Invoke ("StopBonk", 0.4f);
                    if(soundSpawner != null){
                    soundSpawner.SpawnEffect (getHitSound, 1, 1, 1, transform);
                    }
                }
                break;
            case 2:
                anim.Play ("metarig|Flying", 0);
                break;
        }
    }

    void StopBonk () {
        curState = State.Idle;
        anim.Play ("metarig|Flying", 0);
    }
}
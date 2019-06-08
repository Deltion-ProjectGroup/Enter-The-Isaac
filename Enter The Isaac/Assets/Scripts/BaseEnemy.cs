using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour {

    public enum State {
        Spawn,
        Die,
        Move,
        Idle
    }
    public State curState;
    [SerializeField] Hitbox hitbox;
    float timer = 0;
    [HideInInspector] public ShakeCam shakeCam;
    [SerializeField] Vector2 randomSpeedMultiplier = new Vector2 (0.9f, 1.5f);
    [HideInInspector] public SoundSpawn soundSpawner;
    [SerializeField] AudioClip getHitSound;
    [SerializeField] AudioClip dieSound;
    public AudioClip shootSound;
    [Header ("Spawning")]
    [SerializeField] Renderer[] spawnInvisible;
    [SerializeField] GameObject spawnParticle;
    [SerializeField] bool skipParticle = false;
    public enum PathMethod {
        GoToPlayer,
        KeepDistanceFromPlayer,
        DontMove
    }

    [Header ("PathFinding")]
    [SerializeField] float distanceFromPlayer = 10;
    public PathMethod myPathMethod = PathMethod.GoToPlayer;
    [SerializeField] float ignorePlayerDistance = 30;
    [SerializeField] bool alwaysLookAtPlayer = false;
    [SerializeField] bool lookAtPlayerWhileWalking = true;
    [Header ("Attacking")]
    [SerializeField] bool canAttackWhileWalking = true;
    public enum AttackType {
        Repeat,
        RepeatOnSight,
        Close,
        ResponseFire,
        OnDeath,
        Overridable
    }
    public AttackType[] attackType;
    [SerializeField] float repeatRate = 1;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Transform player;
    PlayerController playerCon;
    //animation
    [HideInInspector] public Animator anim;
    Vector3 lastPos;
    float walkWaitTime = 1;
    [Header ("Shooting")]
    public Transform[] gun;
    Vector3[] gunStartPos;
    public float recoil = 1;
    [SerializeField] float recoilBackSpeed = 1;
    [SerializeField] GameObject toShoot;
    [SerializeField] Transform shootPivot;
    [SerializeField] int amountOfBullets = 1;
    [SerializeField][Range (0, 360)] float angleRadius = 30;
    [SerializeField] float speedMuliplier = 1;
    public int damage = 1;
    [SerializeField] float rotateY = 0;
    [SerializeField] float lifeTime = 1;
    [SerializeField] bool parentToMe = false;

    void Awake () {
        StartBase ();
    }

    public void StartBase () {
        soundSpawner = FindObjectOfType<SoundSpawn> ();
        shakeCam = GetComponent<ShakeCam> ();
        agent = GetComponent<NavMeshAgent> ();
        agent.speed *= Random.Range (randomSpeedMultiplier.x, randomSpeedMultiplier.y);
        player = FindObjectOfType<PlayerController> ().transform;
        playerCon = player.GetComponent<PlayerController> ();
        anim = transform.GetChild (0).GetComponent<Animator> ();
        if (skipParticle == false) {
            Spawn ();
            hitbox.enabled = false;
        }
        gunStartPos = new Vector3[gun.Length];
        for (int i = 0; i < gun.Length; i++) {
            gunStartPos[i] = gun[i].localPosition;
        }
    }

    void Spawn () {
        for (int i = 0; i < spawnInvisible.Length; i++) {
            spawnInvisible[i].enabled = false;
        }
        curState = State.Spawn;
        hitbox.enabled = false;
        Instantiate (spawnParticle, transform.position, spawnParticle.transform.rotation);
        Invoke ("AfterSpawnComplete", 3.2f);
    }

    void AfterSpawnComplete () {
        hitbox.enabled = true;
        for (int i = 0; i < spawnInvisible.Length; i++) {
            spawnInvisible[i].enabled = true;
        }
        curState = State.Move;
    }

    void Update () {
        UpdateBase ();
    }

    public void UpdateBase () {
        switch (curState) {
            case State.Move:
                Move ();
                SetWalkAnim ();
                break;
        }
        if (curState != State.Spawn) {
            CheckAttack ();
            if (alwaysLookAtPlayer == true && curState != State.Die) {
                transform.LookAt (new Vector3 (player.position.x, transform.position.y, player.position.z));
            }
        }
        for (int i = 0; i < gun.Length; i++) {
            gun[i].localPosition = Vector3.MoveTowards (gun[i].localPosition, gunStartPos[i], Time.deltaTime * recoilBackSpeed);
        }
    }

    void SetWalkAnim () {
        if (agent.velocity.magnitude > 0) {
            anim.SetBool ("walking", true);
        } else {
            anim.SetBool ("walking", false);
            lastPos = transform.position;
        }
    }

    public virtual void Move () {
        if (agent.isOnNavMesh == true) {
            if (Vector3.Distance (transform.position, player.position) <= ignorePlayerDistance) {
                switch (myPathMethod) {
                    case PathMethod.GoToPlayer:
                        GoToPlayer ();
                        agent.isStopped = (Vector3.Distance (player.position, transform.position) < distanceFromPlayer);
                        break;

                    case PathMethod.KeepDistanceFromPlayer:
                        KeepDistanceFromPlayer ();
                        break;

                    case PathMethod.DontMove:
                        //yeah well, dont move
                        break;
                }
            } else {
                agent.isStopped = true;
            }
        } else {
            DieEvent ();
        }
    }

    void KeepDistanceFromPlayer () {

        float distance = Vector3.Distance (transform.position, player.position);
        if (distance > distanceFromPlayer - (distanceFromPlayer / 10) && distance < distanceFromPlayer + (distanceFromPlayer / 10)) //if i'm close    if(5 units away from player > 10 / 10 = 1)
        {
            //dont move
            walkWaitTime = 0.2f;
            agent.isStopped = true;
        } else if (walkWaitTime <= 0) {
            agent.isStopped = false;
            GoAwayFromPlayer ();
        }
        walkWaitTime = Mathf.Max (-0.001f, walkWaitTime - Time.deltaTime);

    }

    void GoToPlayer () {
        agent.SetDestination (player.position);
        if (agent.isStopped == false && lookAtPlayerWhileWalking == true) {
            transform.LookAt (new Vector3 (player.position.x, transform.position.y, player.position.z));
        }
    }

    void GoAwayFromPlayer () {
        Vector3 awayFromPlayerDir = (transform.position - player.position).normalized;
        awayFromPlayerDir.y = 0;
        agent.SetDestination (player.position + (awayFromPlayerDir * distanceFromPlayer));
        if (lookAtPlayerWhileWalking == true) {
            transform.LookAt (new Vector3 (player.position.x, transform.position.y, player.position.z));
        }
    }

    public void DieEvent () {
        curState = State.Die;
        agent.enabled = false;
        if (IsInvoking ("DieEvent") == false) {
            Invoke ("DieEvent", Time.deltaTime);
        }
        if (GetComponent<Hitbox> ().impactDir != Vector3.zero) {
            transform.LookAt (transform.position + GetComponent<Hitbox> ().impactDir);
        }
        if (hitbox.enabled == true) {
            soundSpawner.SpawnEffect (dieSound);
            shakeCam.SmallShake ();
            hitbox.enabled = false;
        }
        timer += Time.deltaTime;
        if (timer < 0.05f) {
            transform.position -= transform.forward * 100 * Time.deltaTime;
        }
        if (timer > 0.25f) {
            shakeCam.SmallShake ();
            Camera.main.fieldOfView = 60;
            Destroy (gameObject);
        }
    }

    int curGun = 0;
    public virtual void Attack () {
        soundSpawner.SpawnEffect (shootSound);
        shakeCam.SmallShake ();
        gun[curGun].position -= gun[curGun].right * recoil;
        curGun = (int) Mathf.Repeat (curGun + 1, gun.Length);
        float curAngle = -angleRadius / 2;
        for (int i = 0; i < amountOfBullets; i++) {
            GameObject g = Instantiate (toShoot, shootPivot.position, transform.rotation * toShoot.transform.rotation * Quaternion.Euler (0, curAngle, 0));
            g.GetComponent<AutoRotate> ().speed = speedMuliplier;
            g.GetComponent<AutoRotate> ().eulerV3.y = rotateY / speedMuliplier;
            g.GetComponent<DestroyAfterSeconds> ().time = lifeTime;
            if (parentToMe == true) {
                g.transform.SetParent (transform);
            }
            g.GetComponent<Hurtbox> ().damage = damage;
            curAngle += angleRadius / amountOfBullets;
        }
    }

    Vector3 lastGunScale = Vector3.zero;
    bool hadIK = true;
    public virtual void GetHit () {
        if (hitbox.enabled == true) {
            if (IsInvoking ("StopGetHit") == false) {
                hadIK = anim.GetComponent<IKHoldGun> ().enabled;
            }
            anim.Play ("Flinch", 0);
            soundSpawner.SpawnEffect (getHitSound);
            anim.GetComponent<IKHoldGun> ().enabled = false;
            if (lastGunScale == Vector3.zero) {
                lastGunScale = gun[0].localScale;
            }
            for (int i = 0; i < gun.Length; i++) {
                gun[i].localScale = Vector3.zero;
            }
            Invoke ("StopGetHit", 0.2f);
        }
    }

    void StopGetHit () {
        if (anim.GetComponent<IKHoldGun> ().enabled == false) {
            anim.GetComponent<IKHoldGun> ().enabled = hadIK;
        }
        for (int i = 0; i < gun.Length; i++) {
            gun[i].localScale = lastGunScale;
        }
    }

    void CheckAttack () {
        bool helper = false;
        if (canAttackWhileWalking == false && anim.GetBool ("walking") == true) {
            helper = true;
        }
        if (Vector3.Distance (transform.position, player.position) > ignorePlayerDistance) {
            helper = true;
        }
        if (helper == false) {
            for (int i = 0; i < attackType.Length; i++) {
                switch (attackType[i]) {
                    case AttackType.Repeat:
                        AttackRepeat ();
                        break;
                    case AttackType.RepeatOnSight:
                        AttackRepeatOnSight ();
                        break;
                    case AttackType.ResponseFire:
                        AttackResponseFire ();
                        break;
                    case AttackType.Close:
                        AttackClose ();
                        break;
                    case AttackType.OnDeath:
                        AttackOnDeath ();
                        break;
                    case AttackType.Overridable:
                        AttackOverridable ();
                        break;
                }
            }
        } else if (IsInvoking ("AttackRepeatFunction") == true) {
            CancelInvoke ("AttackRepeatFunction");
        }
    }

    void AttackRepeat () {
        if (IsInvoking ("AttackRepeatFunction") == false) {
            InvokeRepeating ("AttackRepeatFunction", repeatRate, repeatRate);
        }
    }

    void AttackRepeatOnSight () {
        if (player.GetComponent<PlayerController> ().curState != PlayerController.State.Roll) //fix this line
        {
            RaycastHit hit;
            if (Physics.Raycast (transform.position + new Vector3 (0, 0.15f, 0), player.position - transform.position, out hit, Vector3.Distance (transform.position, player.position), LayerMask.GetMask ("Player", "Default"), QueryTriggerInteraction.Collide)) {
                if (hit.transform.tag == player.tag) {
                    if (IsInvoking ("AttackRepeatFunction") == false) {
                        InvokeRepeating ("AttackRepeatFunction", repeatRate, repeatRate);
                    }
                } else {
                    CancelInvoke ("AttackRepeatFunction");
                }
            } else {
                CancelInvoke ("AttackRepeatFunction");
            }
        }
    }
    void AttackRepeatFunction () {
        //basically this is the actual called function
        Attack ();
    }

    void AttackResponseFire () {
        if (player != null) {
            if (playerCon.gun != null && playerCon.gun.IsInvoking ("Shoot") == true) {
                if (IsInvoking ("AttackRepeatFunction") == false) {
                    InvokeRepeating ("AttackRepeatFunction", 0, repeatRate);
                }
            } else {
                CancelInvoke ("AttackRepeatFunction");
            }
        } else {
            CancelInvoke ("AttackRepeatFunction");
        }
    }

    void AttackClose () {
        if (Vector3.Distance (transform.position, player.position) < distanceFromPlayer) {
            if (IsInvoking ("AttackRepeatFunction") == false) {
                InvokeRepeating ("AttackRepeatFunction", repeatRate, repeatRate);
            }
        } else {
            CancelInvoke ("AttackRepeatFunction");
        }
    }
    void AttackOnDeath () {
        if (curState == State.Die) {
            if (IsInvoking ("AttackRepeatFunction") == false) {
                InvokeRepeating ("AttackRepeatFunction", 0, repeatRate);
            }
        }
    }

    public virtual void AttackOverridable () {
        //for inheritance
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemy : BaseEnemy
{

    bool isAttacking = false;
    [Header("Sniper related")]
    [SerializeField] LineHurtBox hurtbox;
    LineRenderer hurtboxLine;
    [SerializeField] Color aimColor;
    [SerializeField] Color lockColor;
    [SerializeField] Color shootColor;
    [SerializeField] float aimThickness = 0.05f;
    [SerializeField] float shootThickness = 0.3f;
    [SerializeField] float aimTime = 1;
    [SerializeField] float shootDelay = 0.3f;
    [SerializeField] float activeTime = 0.1f;
    int attackState = 0;
    [SerializeField] float lookAtPlayerSpeed = 10;
    [SerializeField] AudioClip warningSound;
    void Start()
    {
        StartBase();
        hurtboxLine = hurtbox.GetComponent<LineRenderer>();
        hurtbox.damage = damage;
        hurtbox.StartStuff();
    }

    void Update()
    {
        UpdateBase();
    }
    Coroutine curAttackCoroutine;
    public override void Attack()
    {
        curAttackCoroutine = StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        StopCoroutine("AttackCoroutine");
        if (agent.isActiveAndEnabled == true)
        {
            agent.isStopped = true;
        }
        isAttacking = true;
        attackState = 1;
        hurtbox.enabled = false;
        hurtboxLine.enabled = true;
        hurtboxLine.startColor = aimColor;
        hurtboxLine.endColor = aimColor;
        hurtboxLine.startWidth = aimThickness;
        hurtboxLine.endWidth = aimThickness;
        yield return new WaitForSeconds(aimTime);
        attackState = 2;
        hurtboxLine.startColor = lockColor;
        hurtboxLine.endColor = lockColor;
        soundSpawner.SpawnEffect(warningSound, 10, 1, 1, transform);
        yield return new WaitForSeconds(shootDelay);
        attackState = 3;
        hurtboxLine.startColor = shootColor;
        hurtboxLine.endColor = shootColor;
        hurtboxLine.startWidth = shootThickness;
        hurtboxLine.endWidth = shootThickness;
        hurtbox.enabled = true;
        soundSpawner.SpawnEffect(shootSound, 2, 1, 1, transform);
        shakeCam.MediumShake();
        gun[0].position -= gun[0].right * recoil; // he only has 1 gun anyways, I made this game in an extremely short time, so give me a break.
        yield return new WaitForSeconds(activeTime);
        attackState = 0;
        hurtbox.enabled = false;
        hurtboxLine.enabled = false;
        myPathMethod = PathMethod.KeepDistanceFromPlayer;
        isAttacking = false;
    }

    public override void AttackOverridable()
    {

        if (isAttacking == false)
        {

            if (agent.isActiveAndEnabled == true && agent.isStopped == false)
            {
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            }

            hurtboxLine.enabled = false;
            if (player.GetComponent<PlayerController>().curState != PlayerController.State.Roll)//fix this line
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + new Vector3(0, 0.15f, 0), player.position - transform.position, out hit, Vector3.Distance(transform.position, player.position)))
                {
                    if (hit.transform.tag == player.tag)
                    {
                        Attack();
                    }
                }
            }
        }
        else
        {
            switch (attackState)
            {
                case 0:
                    myPathMethod = PathMethod.DontMove;
                    break;
                case 1:
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - new Vector3(player.position.x, transform.position.y, player.position.z), Vector3.up) * Quaternion.Euler(0, 180, 0), Time.deltaTime * lookAtPlayerSpeed);
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + new Vector3(0, 0.15f, 0), player.position - transform.position, out hit, Vector3.Distance(transform.position, player.position)))
                    {
                        if (hit.transform.tag != player.tag)
                        {
                            StopCoroutine(curAttackCoroutine);
                            attackState = 0;
                            hurtbox.enabled = false;
                            hurtboxLine.enabled = false;
                            myPathMethod = PathMethod.KeepDistanceFromPlayer;
                            isAttacking = false;
                        }
                    }
                    break;
                case 2:

                    break;
            }
        }

    }
}

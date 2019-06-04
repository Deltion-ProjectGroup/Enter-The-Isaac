using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SergeantBehaviour : BaseEnemy
{
    [Header("Sergeant")]
    [SerializeField] AttackType phase2AttackType;
    [SerializeField] Renderer[] gunRends;
    bool phase1 = true;
    [SerializeField] float spawnTime = 2;
    Coroutine spawnCoroutine;
    [SerializeField] int enemiesToSpawn = 3;
    [SerializeField] GameObject _spawnParticle;
    [SerializeField] float spawnDistance = 5;
    [SerializeField] GameObject enemyToSpawn;
    [SerializeField] int maxEnemies = 3;
    [SerializeField] float initialDelay = 1;
    List<GameObject> spawnParticles = new List<GameObject>();
    Hitbox health;
    [SerializeField] float madHPPercent = 50;
    void Start()
    {
        StartBase();
        for (int i = 0; i < gunRends.Length; i++)
        {
            gunRends[i].enabled = false;
        }
        GetComponentInChildren<IKHoldGun>().enabled = false;
        health = GetComponent<Hitbox>();
    }

    void Update()
    {
        UpdateBase();
    }

    public override void GetHit()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        base.GetHit();
        DestroySpawnParticles();
        if (health.curHealth < health.maxHealth / (100 / madHPPercent))
        {
            GoToPhase2();
            GetComponentInChildren<IKHoldGun>().enabled = true;
        }
        else
        {
            GetComponentInChildren<IKHoldGun>().enabled = false;
        }
    }

    void DestroySpawnParticles()
    {
        for (int i = 0; i < spawnParticles.Count; i++)
        {
            Destroy(spawnParticles[i]);
        }
        spawnParticles = new List<GameObject>();
    }

    void GoToPhase2()
    {
        if (phase1 == true)
        {
            DestroySpawnParticles();
            attackType[0] = phase2AttackType;
            myPathMethod = PathMethod.GoToPlayer;
            GetComponentInChildren<IKHoldGun>().enabled = true;
            for (int i = 0; i < gunRends.Length; i++)
            {
                gunRends[i].enabled = true;
            }
            //stop spawning enemies
            anim.SetBool("calling", false);
            phase1 = false;
        }
    }

    public override void AttackOverridable()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle") == true && spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnCoroutine());
        }
    }

    IEnumerator SpawnCoroutine()
    {
        spawnParticles = new List<GameObject>();
        yield return new WaitForSeconds(initialDelay);
        //play animation, show spawn locations
        anim.Play("SpawnStart");
        anim.SetBool("calling", true);

        List<GameObject> spawnLocations = new List<GameObject>();
        Vector3 initialAngle = transform.eulerAngles;
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject g = Instantiate(_spawnParticle, transform.position + (transform.forward * spawnDistance), Quaternion.Euler(initialAngle));
            transform.Rotate(0, 360 / enemiesToSpawn, 0);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, spawnDistance, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
            {
                g.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }

            spawnLocations.Add(g);
            spawnParticles.Add(g);
            maxEnemies--;
            if (maxEnemies <= 0)
            {
                break;
            }
        }
        transform.eulerAngles = initialAngle;

        yield return new WaitForSeconds(spawnTime);
        anim.SetBool("calling", false);
        //spawn
        for (int i = 0; i < spawnLocations.Count; i++)
        {
            Instantiate(enemyToSpawn, spawnLocations[i].transform.position, transform.rotation);
        }
        for (int i = 0; i < spawnLocations.Count; i++)
        {
            spawnParticles.Remove(spawnLocations[i]);
            Destroy(spawnLocations[i]);
        }
        if (maxEnemies <= 0)
        {
            Invoke("GoToPhase2", 0.1f);
        }
        spawnCoroutine = null;
    }
}

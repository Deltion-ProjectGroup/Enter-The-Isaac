using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GunMagician : MonoBehaviour {
    [SerializeField] float flyHeight = 2;
    RippleEffect mainCamRipple;
    Vector3 startScale = Vector3.zero;
    ShakeCam shakeCam;
    Shake selfShake;
    Camera mainCam;
    Collider col;
    Transform player;
    bool isDoingSomething = false;
    [SerializeField] float currentSpeed = 1;
    [SerializeField] AnimationCurve currentSpeedOverHealth;
    Hitbox hitbox;
    [Header ("Teleport")]
    [SerializeField] GameObject teleportLandParticle;
    [SerializeField] GameObject teleportLocationsParent;
    [SerializeField] float teleportPlayerDistance = 5;
    [Header ("Laser")]
    [SerializeField] ParticleSystem speedLines;
    [SerializeField] LensFlare laserFlare;
    [SerializeField] GameObject laser;
    [SerializeField] GameObject anounceLaser;
    [SerializeField] float laserAimTime = 1;
    [SerializeField] float laserAnounceTime = 0.4f;
    [SerializeField] float laserShootTime = 2;
    [SerializeField] float laserRecoverTime = 0.2f;
    [SerializeField] float lookAtPlayerSpeed = 1;
    [Header ("Poof 'n Pop")]
    [SerializeField] GameObject poofpopProjectile;
    [SerializeField] float poofpopProjectileSpeed = 4;
    [SerializeField] Transform poofnpopSpawnLocation1;
    [SerializeField] Transform poofnpopSpawnLocation2;
    [SerializeField] int amountOfTeleports = 5;
    int teleportsLeft = 0;
    [Header ("Point 'n Shoot")]
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;
    [SerializeField] GameObject pointshootProjectile;
    [SerializeField] float pointshootRepeatSpeed = 0.1f;
    [SerializeField] int pointshootTimes = 7;
    int pointshootLeft = 0;
    [SerializeField] float pointShootProjectileSpeed = 10;
    float pointshootCurAngle = 0;
    [SerializeField] float pointShootAngle = 90;
    [SerializeField] float pointshootTotalAngle = 720;
    [SerializeField] float pointShootWindUpTime = 1;
    [Header ("Clap 'n Attack")]
    [SerializeField] GameObject clapAttackProjectile;
    [SerializeField] Transform clapAttackRing1;
    [SerializeField] Transform clapAttackRing2;
    [SerializeField] float clapAttackRing1Speed = 10;
    [SerializeField] float clapAttackRing2Speed = 20;
    [SerializeField] float clapAttackWindUpTime = 0.5f;
    [SerializeField] float clapAttackRecover = 0.5f;
    [Header ("Produce 'n Confuse")]
    [SerializeField] int amountToSpawn = 3;
    [SerializeField] float startAngle = 180;
    [SerializeField] float angleAppart = 20;
    float produceConfuseCurAngle = 0;
    [SerializeField] GameObject produceConfuseProjectile;
    [SerializeField] GameObject produceConfuseProjectileEnemy;
    [SerializeField] float produceConfuseWindUpTime = 0.4f;
    [SerializeField] float produceConfuseRecover = 0.4f;
    [SerializeField] float produceConfuseProjectileSpeed = 10;
    [SerializeField] float produceConfuseProjectileThrowYVelocity = 10;
    [SerializeField] float produceConfuseProjectileMass = 10;

    void Start () {
        flyHeight += transform.position.y;
        mainCamRipple = Camera.main.GetComponent<RippleEffect> ();
        shakeCam = GetComponent<ShakeCam> ();
        selfShake = GetComponent<Shake> ();
        mainCam = Camera.main;
        mainCam.GetComponent<Cam> ().offset = new Vector3 (0, 10, -10);
        startScale = transform.localScale;
        col = GetComponent<Collider> ();
        player = FindObjectOfType<PlayerController> ().transform;
        hitbox = GetComponent<Hitbox> ();

        InvokeRepeating ("CheckForAttack", 2 / currentSpeed, 2 / currentSpeed);
    }
    void Update () {
        if (Input.GetKeyDown (KeyCode.O) == true && isDoingSomething == false) {
            // Teleport(false);
            //LaserAttack();
            // PoofNPop();
            //PointNShoot();
            //ClapNAttack();
            // ProduceNConfuse();
        }
    }

    public void UpdateCurrentSpeed () {
        CancelInvoke ("CheckForAttack");
        currentSpeed = currentSpeedOverHealth.Evaluate (1 - (hitbox.curHealth / hitbox.maxHealth)) + 1;
        InvokeRepeating ("CheckForAttack", 2 / currentSpeed, 2 / currentSpeed);
    }

    public void CheckForAttack () {
        if (isDoingSomething == false) {
            UpdateCurrentSpeed ();
            int rand = Random.Range (0, 6);
            switch (rand) {
                case 0:
                    Teleport (false);
                    break;
                case 1:
                    LaserAttack ();
                    break;
                case 2:
                    PoofNPop ();
                    break;
                case 3:
                    PointNShoot ();
                    break;
                case 4:
                    ClapNAttack ();
                    break;
                case 5:
                    //ProduceNConfuse ();
                    break;
            }
        }
    }

    public void Teleport (bool ignoreDoElse) {
        if (ignoreDoElse == false) {
            isDoingSomething = true;
        }
        StartCoroutine (TeleportCoroutine (ignoreDoElse));
    }

    IEnumerator TeleportCoroutine (bool ignoreDoElse) {
        shakeCam.SmallShake ();
        transform.localScale = new Vector3 (transform.localScale.x * 0.3f, transform.localScale.y * 3, transform.localScale.z * 0.3f);
        yield return new WaitForSeconds (0.03f / currentSpeed);
        transform.localScale = Vector3.zero;
        yield return new WaitForSeconds (0.25f / currentSpeed);
        transform.position = GetRandomLocation ();
        mainCamRipple.Emit (new Vector2 (0.5f, 0.5f));
        yield return new WaitForSeconds (0.03f / currentSpeed);
        transform.localScale = startScale;
        transform.localScale = new Vector3 (transform.localScale.x * 0.3f, transform.localScale.y * 3, transform.localScale.z * 0.3f);
        shakeCam.MediumShake ();
        Instantiate (teleportLandParticle, transform.position, transform.rotation);
        yield return new WaitForSeconds (0.03f / currentSpeed);
        transform.localScale = startScale;
        yield return new WaitForSeconds (0.2f / currentSpeed);
        if (ignoreDoElse == false) {
            isDoingSomething = false;
        }
    }

    Vector3 GetRandomLocation () {
        int random = Random.Range (0, teleportLocationsParent.transform.childCount);
        if (transform.position == teleportLocationsParent.transform.GetChild (random).position) {
            random = (int) Mathf.Repeat (random + 1, teleportLocationsParent.transform.childCount);
        }
        for (int i = 0; i < 1; i++) {
            if (Vector3.Distance (teleportLocationsParent.transform.GetChild (random).position, player.position) < teleportPlayerDistance) {
                i--;
                random = (int) Mathf.Repeat (random + 1, teleportLocationsParent.transform.childCount);
            }
        }
        return teleportLocationsParent.transform.GetChild (random).position;
    }

    public void LaserAttack () {
        isDoingSomething = true;
        StartCoroutine (LaserCoroutine ());
    }

    IEnumerator LaserCoroutine () {
        //aim
        Vector3 startAngle = transform.eulerAngles;
        InvokeRepeating ("SmoothLookAtPlayer", 0, 0.01f);
        yield return new WaitForSeconds (laserAimTime / currentSpeed);
        //anounce
        laserFlare.enabled = true;
        shakeCam.CustomShake (Mathf.Min (0.25f, laserAnounceTime / currentSpeed), 0.3f);
        anounceLaser.SetActive (true);
        selfShake.CustomShake (Mathf.Min (0.25f, laserAnounceTime / currentSpeed), 1);
        yield return new WaitForSeconds (Mathf.Min (0.25f, laserAnounceTime / currentSpeed));
        //fire
        anounceLaser.SetActive (false);
        speedLines.Play ();
        shakeCam.CustomShake (laserShootTime / currentSpeed, 1);
        mainCam.fieldOfView = 80;
        laser.SetActive (true);
        yield return new WaitForSeconds (laserShootTime / currentSpeed);
        //stop & recover
        CancelInvoke ("SmoothLookAtPlayer");
        speedLines.Stop ();
        laserFlare.enabled = false;
        laser.SetActive (false);
        yield return new WaitForSeconds (laserRecoverTime / currentSpeed);
        //end
        transform.eulerAngles = startAngle;
        isDoingSomething = false;
    }

    void SmoothLookAtPlayer () {
        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (transform.position - new Vector3 (player.position.x, transform.position.y, player.position.z), Vector3.up) * Quaternion.Euler (0, 180, 0), Time.deltaTime * lookAtPlayerSpeed * currentSpeed);

    }

    public void PoofNPop () {
        isDoingSomething = true;
        teleportsLeft = amountOfTeleports;
        StartCoroutine (PoofNPopCoroutine ());
    }

    IEnumerator PoofNPopCoroutine () {
        Teleport (true);
        yield return new WaitForSeconds (0.5f / currentSpeed);
        //shoot
        int random = Random.Range (0, 2);
        Transform transToUse;
        if (random == 0) {
            transToUse = poofnpopSpawnLocation1;
        } else {
            transToUse = poofnpopSpawnLocation2;
        }
        for (int i = 0; i < transToUse.childCount; i++) {
            Transform t = Instantiate (poofpopProjectile, transToUse.GetChild (i).position, transform.rotation).transform;
            t.LookAt (transToUse.position);
            t.Rotate (0, 180, 0);
            t.GetComponent<AutoRotate> ().tranformV3.z = poofpopProjectileSpeed;
        }
        shakeCam.MediumShake ();
        selfShake.SmallShake ();
        yield return new WaitForSeconds (0.3f / currentSpeed);
        teleportsLeft--;
        if (teleportsLeft > 0) {
            StartCoroutine (PoofNPopCoroutine ());
        } else {
            isDoingSomething = false;
        }
    }

    public void PointNShoot () {
        pointshootLeft = pointshootTimes;
        isDoingSomething = true;
        pointshootCurAngle = pointShootAngle;
        shakeCam.CustomShake ((pointshootRepeatSpeed + pointShootWindUpTime) / currentSpeed, 0.05f);
        selfShake.CustomShake ((pointshootRepeatSpeed + pointShootWindUpTime) / currentSpeed, 1.5f);
        InvokeRepeating ("PointNShootShoot", (pointshootRepeatSpeed + pointShootWindUpTime) / currentSpeed, pointshootRepeatSpeed / currentSpeed);
    }

    void PointNShootShoot () {
        Instantiate (pointshootProjectile, leftHand.position, leftHand.rotation * pointshootProjectile.transform.rotation * Quaternion.Euler (0, pointshootCurAngle, 0)).GetComponent<AutoRotate> ().tranformV3.z = pointShootProjectileSpeed;
        Instantiate (pointshootProjectile, rightHand.position, rightHand.rotation * pointshootProjectile.transform.rotation * Quaternion.Euler (0, -pointshootCurAngle, 0)).GetComponent<AutoRotate> ().tranformV3.z = pointShootProjectileSpeed;
        pointshootLeft--;
        pointshootCurAngle += pointshootTotalAngle / pointshootTimes;
        //pointshootCurAngle = Random.Range(0,pointshootTotalAngle);
        shakeCam.SmallShake ();
        if (pointshootLeft <= 0) {
            CancelInvoke ("PointNShootShoot");
            isDoingSomething = false;
        }
    }

    public void ClapNAttack () {
        isDoingSomething = true;
        StartCoroutine (ClapAttackCoroutine ());
    }

    IEnumerator ClapAttackCoroutine () {
        yield return new WaitForSeconds (clapAttackWindUpTime / currentSpeed); //startup
        //attack
        for (int i = 0; i < clapAttackRing1.childCount; i++) {
            Instantiate (clapAttackProjectile, clapAttackRing1.GetChild (i).position, Quaternion.LookRotation (clapAttackRing1.GetChild (i).position - transform.position)).GetComponent<AutoRotate> ().tranformV3.z = clapAttackRing1Speed;
        }
        for (int i = 0; i < clapAttackRing2.childCount; i++) {
            Instantiate (clapAttackProjectile, clapAttackRing2.GetChild (i).position, Quaternion.LookRotation (clapAttackRing2.GetChild (i).position - transform.position)).GetComponent<AutoRotate> ().tranformV3.z = clapAttackRing2Speed;
        }
        mainCamRipple.Emit (new Vector2 (0.5f, 0.5f));
        shakeCam.HardShake ();

        yield return new WaitForSeconds (clapAttackRecover / currentSpeed); //recover
        isDoingSomething = false;
    }

    public void ProduceNConfuse () {
        isDoingSomething = true;
        produceConfuseCurAngle = startAngle;
        StartCoroutine (ProduceNConfuseCoroutine ());
    }

    IEnumerator ProduceNConfuseCoroutine () {
        yield return new WaitForSeconds (produceConfuseWindUpTime / currentSpeed);
        for (int i = 0; i < amountToSpawn; i++) {
            GameObject g = Instantiate (produceConfuseProjectile, leftHand.position, leftHand.rotation * produceConfuseProjectile.transform.rotation * Quaternion.Euler (0, produceConfuseCurAngle, 0));
            g.GetComponent<Rigidbody> ().velocity = new Vector3 (0, produceConfuseProjectileThrowYVelocity, 0);
            g.GetComponent<AutoRotate> ().tranformV3.z = produceConfuseProjectileSpeed;
            g.GetComponent<Rigidbody> ().mass = produceConfuseProjectileMass;
            g.GetComponent<SpawnOnDestroy> ().toSpawn = new GameObject[1];
            g.GetComponent<SpawnOnDestroy> ().toSpawn[0] = produceConfuseProjectileEnemy;
            produceConfuseCurAngle += angleAppart;
        }
        yield return new WaitForSeconds (produceConfuseRecover / currentSpeed);
        isDoingSomething = false;
    }
}
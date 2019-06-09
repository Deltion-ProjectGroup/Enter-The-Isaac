using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GunMagician : MonoBehaviour {
    [SerializeField] bool skipIntro = false;
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
    SoundSpawn soundSpawner;
    [Header ("Teleport")]
    [SerializeField] GameObject teleportLandParticle;
    [SerializeField] GameObject teleportLocationsParent;
    [SerializeField] float teleportPlayerDistance = 5;
    [SerializeField] AudioClip teleportInAudio;
    [SerializeField] AudioClip teleportOutAudio;
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
    [SerializeField] AudioClip laserAounceSound;
    [Header ("Poof 'n Pop")]
    [SerializeField] GameObject poofpopProjectile;
    [SerializeField] float poofpopProjectileSpeed = 4;
    [SerializeField] Transform poofnpopSpawnLocation1;
    [SerializeField] Transform poofnpopSpawnLocation2;
    [SerializeField] int amountOfTeleports = 5;
    int teleportsLeft = 0;
    [SerializeField] AudioClip poofPopShootSound;
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
    [SerializeField] AudioClip pointshootShootAudio;
    [Header ("Clap 'n Attack")]
    [SerializeField] GameObject clapAttackProjectile;
    [SerializeField] Transform clapAttackRing1;
    [SerializeField] Transform clapAttackRing2;
    [SerializeField] float clapAttackRing1Speed = 10;
    [SerializeField] float clapAttackRing2Speed = 20;
    [SerializeField] float clapAttackWindUpTime = 0.5f;
    [SerializeField] float clapAttackRecover = 0.5f;
    [SerializeField] AudioClip clapAudio;
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
    [SerializeField] float ignoreProduceConfuseTime = 20;
    float centerX = 0;
    [Header ("Intro")]
    [SerializeField] Transform introCamPos;
    [Header("Death")]
    [SerializeField] AudioClip deathSound;
    [SerializeField] GameObject deathCam;

    void Awake () {
        if (skipIntro == false) {
            enabled = false;
        }
    }
    void Start () {
        centerX = transform.position.x;
        soundSpawner = FindObjectOfType<SoundSpawn> ();
        Invoke ("IgnoreProduceConfuse", ignoreProduceConfuseTime);
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
        mainCam.GetComponent<Cam> ().enabled = false;
        FindObjectOfType<MusicManager> ().UpdateMusic (1);

        if (skipIntro == false) {
            shakeCam.CustomShake (0.4f, 2);
            InvokeRepeating ("SmoothIntroCam", 0, 0.01f);
            soundSpawner.SpawnEffect (laserAounceSound);
            FindObjectOfType<PlayerController> ().SetInControll (false);
            FindObjectOfType<PlayerController> ().keyUICounter.transform.parent.parent.gameObject.SetActive (false);
        } else {
            StartAttacking ();
            transform.parent.GetComponentInChildren<CollisionEvent> ().gameObject.SetActive (false);
            transform.parent.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
        }
    }

    public void SmoothIntroCam () {
        mainCam.transform.position = Vector3.MoveTowards (mainCam.transform.position, introCamPos.position, Time.deltaTime * 25);
        mainCam.transform.rotation = Quaternion.Lerp (mainCam.transform.rotation, introCamPos.rotation, Time.deltaTime * 5);
    }

    public void StartAttacking () {
        FindObjectOfType<PlayerController> ().SetInControll (true);
        mainCam.GetComponent<Cam> ().enabled = true;
        Teleport (false);
        FindObjectOfType<PlayerController> ().keyUICounter.transform.parent.parent.gameObject.SetActive (true);
        InvokeRepeating ("CheckForAttack", 2 / currentSpeed, 2 / currentSpeed);
        CancelInvoke ("SmoothIntroCam");
    }

    void IgnoreProduceConfuse () {
        //invoke function lol
    }

    public void Death () {
        StopAllCoroutines ();
        shakeCam.HardShake ();
        //laser
        laserFlare.enabled = false;
        laser.SetActive (false);
        anounceLaser.SetActive (false);
        speedLines.Stop ();
        CancelInvoke ("SmoothLookAtPlayer");
        //ProduceNConfuse
        GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
        for (int i = 0; i < enemies.Length; i++) {
            if (enemies[i] != gameObject) {
                Destroy (enemies[i]);
            }
        }
        //Teleport
        transform.localScale = startScale;
        //pointnshoot
        CancelInvoke ("PointNShootShoot");
        Hurtbox[] hurtboxes = GameObject.FindObjectsOfType<Hurtbox> ();
        for (int i = 0; i < hurtboxes.Length; i++) {
            hurtboxes[i].damage = 0;
        }
        CancelInvoke();

        //Game feel shit
        FindObjectOfType<MusicManager> ().UpdateMusic (4);
        FindObjectOfType<Pause>().enabled = false;
        FindObjectOfType<Pause>().isPaused = false;
        Time.timeScale = 0.3f;
        col.enabled = false;
        mainCamRipple.Emit();
        StartDeathCam();
        transform.parent.GetComponentInChildren<Canvas>(true).gameObject.SetActive(false);
        player.parent.GetComponentInChildren<Canvas>(true).gameObject.SetActive(false);
    }

    void StartDeathCam(){
        soundSpawner.SpawnEffect(deathSound);
        mainCam.GetComponent<Cam> ().offset = new Vector3 (0, 7, -6);
        deathCam.SetActive(true);
        Invoke("StopDeathCam",5f * Time.timeScale);
    }
    void StopDeathCam(){
        Time.timeScale = 1;
       deathCam.SetActive(false); 
       FindObjectOfType<Pause>().enabled = true;
       FindObjectOfType<MusicManager> ().UpdateMusic (5);
       player.parent.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
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
                    int numOfEnemies = GameObject.FindGameObjectsWithTag ("Enemy").Length;
                    if (IsInvoking ("IgnoreProduceConfuse") == false && numOfEnemies - 1 == 0) {
                        ProduceNConfuse ();
                        Invoke ("IgnoreProduceConfuse", ignoreProduceConfuseTime);
                    } else {
                        Teleport (false);
                    }
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
        soundSpawner.SpawnEffect (teleportOutAudio);
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
        soundSpawner.SpawnEffect (teleportInAudio);
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
        shakeCam.CustomShake (Mathf.Max (0.25f, laserAnounceTime / currentSpeed), 0.3f);
        anounceLaser.SetActive (true);
        selfShake.CustomShake (Mathf.Max (0.25f, laserAnounceTime / currentSpeed), 1);
        yield return new WaitForSeconds (Mathf.Max (0.25f, laserAnounceTime / currentSpeed));
        //fire
        selfShake.enabled = false;
        soundSpawner.SpawnEffect (laserAounceSound);
        anounceLaser.SetActive (false);
        speedLines.Play ();
        shakeCam.CustomShake (laserShootTime / currentSpeed, 1);
        mainCam.fieldOfView = 80;
        laser.SetActive (true);
        yield return new WaitForSeconds (laserShootTime / currentSpeed);
        //stop & recover
        selfShake.enabled = true;
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
        col.enabled = false;
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
        soundSpawner.SpawnEffect (poofPopShootSound);
        yield return new WaitForSeconds (0.3f / currentSpeed);
        teleportsLeft--;
        if (teleportsLeft > 0) {
            StartCoroutine (PoofNPopCoroutine ());
        } else {
            col.enabled = true;
            isDoingSomething = false;
        }
    }

    float soundPitchhelper = 0;
    public void PointNShoot () {
        pointshootLeft = pointshootTimes;
        isDoingSomething = true;
        pointshootCurAngle = pointShootAngle;
        shakeCam.CustomShake ((pointshootRepeatSpeed + pointShootWindUpTime) / currentSpeed, 0.05f);
        soundPitchhelper = 1;
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
        soundSpawner.SpawnEffect (pointshootShootAudio, 0.25f, soundPitchhelper, 0, transform);
        soundPitchhelper += 0.6f / pointshootTimes;
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
        soundSpawner.SpawnEffect (clapAudio);
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
        if (transform.position.x < centerX) {
            for (int i = 0; i < amountToSpawn; i++) {
                GameObject g = Instantiate (produceConfuseProjectile, leftHand.position, leftHand.rotation * produceConfuseProjectile.transform.rotation * Quaternion.Euler (0, produceConfuseCurAngle, 0));
                g.GetComponent<Rigidbody> ().velocity = new Vector3 (0, produceConfuseProjectileThrowYVelocity, 0);
                g.GetComponent<AutoRotate> ().tranformV3.z = produceConfuseProjectileSpeed;
                g.GetComponent<Rigidbody> ().mass = produceConfuseProjectileMass;
                g.GetComponent<SpawnOnDestroy> ().toSpawn = new GameObject[1];
                g.GetComponent<SpawnOnDestroy> ().toSpawn[0] = produceConfuseProjectileEnemy;
                produceConfuseCurAngle += angleAppart;
            }
        } else {
            for (int i = 0; i < amountToSpawn; i++) {
                GameObject g = Instantiate (produceConfuseProjectile, rightHand.position, rightHand.rotation * produceConfuseProjectile.transform.rotation * Quaternion.Euler (0, produceConfuseCurAngle + 180, 0));
                g.GetComponent<Rigidbody> ().velocity = new Vector3 (0, produceConfuseProjectileThrowYVelocity, 0);
                g.GetComponent<AutoRotate> ().tranformV3.z = produceConfuseProjectileSpeed;
                g.GetComponent<Rigidbody> ().mass = produceConfuseProjectileMass;
                g.GetComponent<SpawnOnDestroy> ().toSpawn = new GameObject[1];
                g.GetComponent<SpawnOnDestroy> ().toSpawn[0] = produceConfuseProjectileEnemy;
                produceConfuseCurAngle -= angleAppart;
            }
        }
        yield return new WaitForSeconds (produceConfuseRecover / currentSpeed);
        isDoingSomething = false;
    }
}
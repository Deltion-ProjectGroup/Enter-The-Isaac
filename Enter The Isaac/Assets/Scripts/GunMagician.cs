using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GunMagician : MonoBehaviour {
    [SerializeField] bool skipIntro = false;
    [SerializeField] bool dontAttack = false;
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
    Animator anim;
    [SerializeField] GameObject contactDamageBox;
    [Header ("Teleport")]
    [SerializeField] GameObject teleportInPartilce;
    [SerializeField] GameObject teleportOutParticle;
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
    [SerializeField] float minAnounceTime = 0.5f;
    [Space]
    float laserPointShootCurAngle = 0;
    [SerializeField] float laserPointShootFireRate = 0.3f;
    [SerializeField] float laserPointShootProjectileSpeed = 10;
    [SerializeField] float laserPointShootStartAngle = -40;
    [SerializeField] float laserPointShootAddedAnglePerShot = 7;
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
    int currentPointShootVersionL = 0;
    int currentPointShootVersionR = 0;
    [Header ("Clap 'n Attack")]
    [SerializeField] GameObject clapAttackProjectile;
    [SerializeField] GameObject clapParticle;
    [SerializeField] Transform clapAttackRing1;
    [SerializeField] Transform clapAttackRing2;
    [SerializeField] float clapAttackRing1Speed = 10;
    [SerializeField] float clapAttackRing2Speed = 20;
    [SerializeField] float clapAttackWindUpTime = 0.5f;
    [SerializeField] float clapAttackRecover = 0.5f;
    [SerializeField] AudioClip clapAudio;
    float clapAttackTwoStartSpeedMultiplier = 1;
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
    [SerializeField] AudioClip produceConfuseSnapSound;
    [SerializeField] GameObject produceConfuseSnapParticle;
    [SerializeField] GameObject produceConfuseHat;
    [Header ("Intro")]
    [SerializeField] Transform introCamPos;
    [Header ("Death")]
    [SerializeField] AudioClip deathSound;
    [SerializeField] GameObject deathCam;
    void Awake () {
        if (skipIntro == false) {
            enabled = false;
        }
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.O)) {
            PointNShoot ();
        }
    }
    void Start () {
        anim = transform.GetChild (0).GetComponent<Animator> ();
        centerX = transform.position.x;
        soundSpawner = FindObjectOfType<SoundSpawn> ();
        Invoke ("IgnoreProduceConfuse", ignoreProduceConfuseTime);
        flyHeight += transform.position.y;
        mainCamRipple = Camera.main.GetComponent<RippleEffect> ();
        shakeCam = GetComponent<ShakeCam> ();
        selfShake = GetComponent<Shake> ();
        mainCam = Camera.main;
        mainCam.GetComponent<Cam> ().offset += new Vector3 (0, 3, -1);
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
            transform.parent.GetComponentInChildren<Canvas> (true).gameObject.SetActive (true);
        }
    }

    public void SmoothIntroCam () {
        mainCam.transform.position = Vector3.MoveTowards (mainCam.transform.position, introCamPos.position, Time.deltaTime * 15);
        mainCam.transform.rotation = Quaternion.Lerp (mainCam.transform.rotation, introCamPos.rotation, Time.deltaTime * 5);
    }

    public void StartAttacking () {
        FindObjectOfType<PlayerController> ().SetInControll (true);
        mainCam.GetComponent<Cam> ().enabled = true;
        Teleport (false);
        FindObjectOfType<PlayerController> ().keyUICounter.transform.parent.parent.gameObject.SetActive (true);
        hitbox.enabled = true;
        if (dontAttack == false) {
            InvokeRepeating ("CheckForAttack", 2 / currentSpeed, 2 / currentSpeed);
        }
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
        CancelInvoke ();

        //Game feel shit
        contactDamageBox.SetActive (false);
        FindObjectOfType<MusicManager> ().UpdateMusic (4);
        FindObjectOfType<Pause> ().enabled = false;
        FindObjectOfType<Pause> ().isPaused = false;
        Time.timeScale = 0.3f;
        col.enabled = false;
        mainCamRipple.Emit ();
        StartDeathCam ();
        transform.parent.GetComponentInChildren<Canvas> (true).enabled = false;
        player.parent.Find ("Canvas").gameObject.SetActive (false);
    }

    void StartDeathCam () {
        soundSpawner.SpawnEffect (deathSound);
        mainCam.GetComponent<Cam> ().offset -= new Vector3 (0, 3, -1);
        deathCam.SetActive (true);
        anim.SetLayerWeight (1, 0);
        anim.SetLayerWeight (2, 0);
        anim.Play ("metarig|Death", 0, 0.6f);
        anim.speed = 0.1f;
        Invoke ("StopDeathCam", 3f * Time.timeScale);
    }
    void StopDeathCam () {
        Time.timeScale = 1;
        deathCam.SetActive (false);
        FindObjectOfType<Pause> ().enabled = true;
        FindObjectOfType<MusicManager> ().UpdateMusic (5);
        player.parent.Find ("Canvas").gameObject.SetActive (true);
        anim.speed = 1;
        transform.position += Vector3.up / 5;
        transform.Rotate (0, 180, 0);
    }

    public void UpdateCurrentSpeed () {
        CancelInvoke ("CheckForAttack");
        currentSpeed = currentSpeedOverHealth.Evaluate (1 - (hitbox.curHealth / hitbox.maxHealth)) + 1;
        anim.SetFloat ("curSpeed", currentSpeed);
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
        StopCoroutine ("TeleportCoroutine");
        StartCoroutine (TeleportCoroutine (ignoreDoElse));
    }

    IEnumerator TeleportCoroutine (bool ignoreDoElse) {
        anim.SetLayerWeight (1, 0);
        anim.SetLayerWeight (2, 0);
        shakeCam.SmallShake ();
        transform.localScale = new Vector3 (transform.localScale.x * 0.3f, transform.localScale.y * 3, transform.localScale.z * 0.3f);
        soundSpawner.SpawnEffect (teleportOutAudio);
        Instantiate (teleportOutParticle, transform.position, transform.rotation);
        yield return new WaitForSeconds (0.03f / currentSpeed);
        transform.localScale = Vector3.zero;
        yield return new WaitForSeconds (0.25f / currentSpeed);
        transform.position = GetRandomLocation ();
        mainCamRipple.Emit (new Vector2 (0.5f, 0.5f));
        yield return new WaitForSeconds (0.03f / currentSpeed);
        transform.localScale = startScale;
        transform.localScale = new Vector3 (transform.localScale.x * 0.3f, transform.localScale.y * 3, transform.localScale.z * 0.3f);
        anim.Play ("metarig|Teleport");
        shakeCam.MediumShake ();
        Instantiate (teleportInPartilce, transform.position, transform.rotation);
        soundSpawner.SpawnEffect (teleportInAudio);
        yield return new WaitForSeconds (0.03f / currentSpeed);
        transform.localScale = startScale;
        yield return new WaitForSeconds (0.2f / currentSpeed);
        if (ignoreDoElse == false) {
            isDoingSomething = false;
        }
        yield return new WaitForSeconds (0.4f);
        if (anim.GetCurrentAnimatorStateInfo (0).IsName ("metarig|Teleport")) {
            anim.Play ("metarig|Idle");
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
        laserPointShootCurAngle = laserPointShootStartAngle;
        StartCoroutine (LaserCoroutine ());
    }

    void LaserPointShoot () {
        Instantiate (pointshootProjectile, leftHand.position, pointshootProjectile.transform.rotation * Quaternion.Euler (180, laserPointShootCurAngle + transform.eulerAngles.y, 180)).GetComponent<AutoRotate> ().tranformV3.z = laserPointShootProjectileSpeed;
        laserPointShootCurAngle += laserPointShootAddedAnglePerShot;
        soundSpawner.SpawnEffect (pointshootShootAudio, 0.5f, Random.Range (0.8f, 1.2f), 0, transform);
    }

    IEnumerator LaserCoroutine () {
        //aim
        Vector3 startAngle = transform.eulerAngles;
        InvokeRepeating ("SmoothLookAtPlayer", 0, 0.01f);
        anim.SetLayerWeight (1, 1);
        anim.SetLayerWeight (2, 0);
        if (hitbox.curHealth < hitbox.maxHealth / 2) {
            anim.SetLayerWeight (1, 1);
            anim.Play ("metarig|Point 'n shoot (Wind up and Recovery Left Hand)", 1);
        }
        anim.Play ("metarig|Laser (Wind Up)");
        yield return new WaitForSeconds (laserAimTime / currentSpeed);
        //anounce
        laserFlare.enabled = true;
        shakeCam.CustomShake (Mathf.Max (minAnounceTime, laserAnounceTime / currentSpeed), 0.3f);
        anounceLaser.SetActive (true);
        selfShake.CustomShake (Mathf.Max (minAnounceTime, laserAnounceTime / currentSpeed), 1);
        yield return new WaitForSeconds (Mathf.Max (minAnounceTime, laserAnounceTime / currentSpeed));
        //fire
        anim.Play ("metarig|Laser (Attack)");
        selfShake.enabled = false;
        soundSpawner.SpawnEffect (laserAounceSound);
        anounceLaser.SetActive (false);
        speedLines.Play ();
        shakeCam.CustomShake (laserShootTime / currentSpeed, 1);
        mainCam.fieldOfView = 80;
        laser.SetActive (true);

        //laser pointshoot
        if (hitbox.curHealth < hitbox.maxHealth / 2) {
            InvokeRepeating ("LaserPointShoot", laserPointShootFireRate / currentSpeed, laserPointShootFireRate / currentSpeed);
            anim.SetLayerWeight (1, 1);
            anim.Play ("metarig|Point 'n shoot (Attack Left Hand)", 1);
        }
        yield return new WaitForSeconds (laserShootTime / currentSpeed);
        //stop & recover
        CancelInvoke ("LaserPointShoot");
        selfShake.enabled = true;
        CancelInvoke ("SmoothLookAtPlayer");
        speedLines.Stop ();
        laserFlare.enabled = false;
        laser.SetActive (false);
        yield return new WaitForSeconds (laserRecoverTime / currentSpeed);
        //end
        anim.Play ("metarig|Laser (Recovery)");
        anim.Play ("metarig|Idle", 1);
        anim.SetLayerWeight (2, 0);
        anim.SetLayerWeight (1, 0);
        transform.eulerAngles = startAngle;
        Teleport (false);
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
        anim.SetLayerWeight (1, 0);
        anim.SetLayerWeight (2, 0);
        anim.Play ("metarig|Poof 'n pop");
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
            anim.Play ("metarig|Poof 'n pop (GoBack)", 0, 1);
            isDoingSomething = false;
        }
    }

    float soundPitchhelper = 0;
    public void PointNShoot () {
        currentPointShootVersionL = Random.Range (0, 2);
        currentPointShootVersionR = Random.Range (0, 2);
       // currentPointShootVersionL = 1;
      //  currentPointShootVersionR = 1;
        anim.SetLayerWeight (1, 1);
        anim.SetLayerWeight (2, 1);
        pointshootLeft = pointshootTimes;
        isDoingSomething = true;
        pointshootCurAngle = pointShootAngle + 180;
        shakeCam.CustomShake ((pointshootRepeatSpeed + pointShootWindUpTime) / currentSpeed, 0.05f);
        soundPitchhelper = 1;
        selfShake.CustomShake ((pointshootRepeatSpeed + pointShootWindUpTime) / currentSpeed, 1.5f);
        Invoke ("StartPointShootAnim", (pointshootRepeatSpeed + pointShootWindUpTime) / currentSpeed / 2);
        InvokeRepeating ("PointNShootShoot", (pointshootRepeatSpeed + pointShootWindUpTime) / currentSpeed, pointshootRepeatSpeed / currentSpeed);
    }

    void StartPointShootAnim () {
        if (currentPointShootVersionR == 0) {
            anim.Play ("metarig|Point 'n shoot v1 (Wind up and Recovery)", 2);
        } else {
            anim.Play ("metarig|Point 'n shoot v2 (Wind up and Recovery)", 2);
        }
        if (currentPointShootVersionL == 0) {
            anim.Play ("metarig|Point 'n shoot (Wind up and Recovery Left Hand) 0", 1);
        } else {
            anim.Play ("metarig|Point 'n shoot (Wind up and Recovery Left Hand) 0", 1);//THIS IS WRONG
        }
    }

    void PointNShootShoot () {
        if (currentPointShootVersionL == 0) {
            Instantiate (pointshootProjectile, leftHand.position, pointshootProjectile.transform.rotation * Quaternion.Euler (0, -pointshootCurAngle, 0)).GetComponent<AutoRotate> ().tranformV3.z = pointShootProjectileSpeed;
            soundSpawner.SpawnEffect (pointshootShootAudio, 0.25f, soundPitchhelper, 0, transform);
        } else {
            Instantiate (pointshootProjectile, leftHand.position, pointshootProjectile.transform.rotation * Quaternion.Euler (0, Random.Range (0, pointshootTotalAngle), 0)).GetComponent<AutoRotate> ().tranformV3.z = pointShootProjectileSpeed;
            soundSpawner.SpawnEffect (pointshootShootAudio, 0.25f, Random.Range (0.8f, 1.2f), 0, transform);
        }
        if (currentPointShootVersionR == 0) {
            Instantiate (pointshootProjectile, rightHand.position, pointshootProjectile.transform.rotation * Quaternion.Euler (0, pointshootCurAngle, 0)).GetComponent<AutoRotate> ().tranformV3.z = pointShootProjectileSpeed;
            soundSpawner.SpawnEffect (pointshootShootAudio, 0.25f, soundPitchhelper, 0, transform);
        } else {
            Instantiate (pointshootProjectile, rightHand.position, pointshootProjectile.transform.rotation * Quaternion.Euler (0, Random.Range (0, pointshootTotalAngle), 0)).GetComponent<AutoRotate> ().tranformV3.z = pointShootProjectileSpeed;
            soundSpawner.SpawnEffect (pointshootShootAudio, 0.25f, Random.Range (0.8f, 1.2f), 0, transform);
        }
        pointshootLeft--;
        pointshootCurAngle += pointshootTotalAngle / pointshootTimes;
        shakeCam.SmallShake ();
        soundPitchhelper += 0.01f;
        soundPitchhelper = Mathf.Min (soundPitchhelper, 2);
        if (pointshootLeft <= 0) {
            CancelInvoke ("PointNShootShoot");
            anim.Play ("metarig|Idle", 2);
            anim.Play ("metarig|Idle", 1);
            isDoingSomething = false;
            anim.SetLayerWeight (1, 0);
            anim.SetLayerWeight (2, 0);
        }
    }

    public void ClapNAttack () {
        isDoingSomething = true;
        clapAttackTwoStartSpeedMultiplier = 1;
        StartCoroutine (ClapAttackCoroutine ());
    }

    IEnumerator ClapAttackCoroutine () {
        anim.Play ("metarig|Clap (Wind Up)");
        anim.SetLayerWeight (1, 0);
        anim.SetLayerWeight (2, 0);
        yield return new WaitForSeconds (clapAttackWindUpTime / clapAttackTwoStartSpeedMultiplier / currentSpeed); //startup
        //attack
        anim.Play ("metarig|Clap (Attack)");
        soundSpawner.SpawnEffect (clapAudio);
        Instantiate (clapParticle, transform.position, transform.rotation);
        clapAttackRing1.Rotate (0, 10, 0);
        clapAttackRing2.Rotate (0, 10, 0);
        for (int i = 0; i < clapAttackRing1.childCount; i++) {
            Instantiate (clapAttackProjectile, clapAttackRing1.GetChild (i).position, Quaternion.LookRotation (clapAttackRing1.GetChild (i).position - transform.position)).GetComponent<AutoRotate> ().tranformV3.z = clapAttackRing1Speed;
        }
        for (int i = 0; i < clapAttackRing2.childCount; i++) {
            Instantiate (clapAttackProjectile, clapAttackRing2.GetChild (i).position, Quaternion.LookRotation (clapAttackRing2.GetChild (i).position - transform.position)).GetComponent<AutoRotate> ().tranformV3.z = clapAttackRing2Speed;
        }
        mainCamRipple.Emit (new Vector2 (0.5f, 0.5f));
        shakeCam.HardShake ();

        yield return new WaitForSeconds (clapAttackRecover / clapAttackTwoStartSpeedMultiplier / currentSpeed); //recover
        float randomFloat = Random.Range (0, 100);
        float curHealthPercent = (1 - (hitbox.curHealth / hitbox.maxHealth)) + 1;
        if (randomFloat > (30 * curHealthPercent) - (clapAttackTwoStartSpeedMultiplier * 5)) {
            isDoingSomething = false;
            anim.Play ("metarig|Clap (Recovery)");
        } else {
            clapAttackTwoStartSpeedMultiplier += 1;
            clapAttackTwoStartSpeedMultiplier = Mathf.Min (clapAttackTwoStartSpeedMultiplier, 3);
            StartCoroutine (ClapAttackCoroutine ());
        }
    }

    public void ProduceNConfuse () {
        isDoingSomething = true;
        produceConfuseCurAngle = startAngle;
        StartCoroutine (ProduceNConfuseCoroutine ());

        anim.SetLayerWeight (1, 0);
        anim.SetLayerWeight (2, 0);
        anim.Play ("metarig|Produce 'n Confuse (Wind Up)");
    }

    void SetHatActive () {
        produceConfuseHat.SetActive (true);
        produceConfuseHat.transform.localScale = Vector3.zero;
    }

    IEnumerator ProduceNConfuseCoroutine () {
        AutoRotate autoRot = produceConfuseHat.GetComponent<AutoRotate> ();
        autoRot.scaleV3 = new Vector3 (0.4f, 0.4f, 0.4f);
        autoRot.speed = 3f * currentSpeed;
        Invoke ("SetHatActive", 0.34f / currentSpeed);
        yield return new WaitForSeconds (produceConfuseWindUpTime / currentSpeed);
        anim.Play ("metarig|Produce 'n Confuse (Snap)");
        soundSpawner.SpawnEffect (produceConfuseSnapSound);
        shakeCam.MediumShake ();
        mainCamRipple.Emit ();
        Instantiate (produceConfuseSnapParticle, leftHand.position, transform.rotation);

        for (int i = 0; i < amountToSpawn; i++) {
            GameObject g = Instantiate (produceConfuseProjectile, rightHand.position, produceConfuseProjectile.transform.rotation * Quaternion.Euler (0, produceConfuseCurAngle - 45, 0));
            g.GetComponent<Rigidbody> ().velocity = new Vector3 (0, produceConfuseProjectileThrowYVelocity, 0);
            g.GetComponent<AutoRotate> ().tranformV3.z = produceConfuseProjectileSpeed;
            g.GetComponent<Rigidbody> ().mass = produceConfuseProjectileMass;
            g.GetComponent<SpawnOnDestroy> ().toSpawn = new GameObject[1];
            g.GetComponent<SpawnOnDestroy> ().toSpawn[0] = produceConfuseProjectileEnemy;
            produceConfuseCurAngle += angleAppart;
        }
        produceConfuseHat.transform.localScale += Vector3.one / 5;
        autoRot.scaleV3 = Vector3.zero;
        autoRot.speed = 5f * currentSpeed;
        selfShake.HardShake ();
        yield return new WaitForSeconds (produceConfuseRecover / currentSpeed);
        produceConfuseHat.SetActive (false);
        isDoingSomething = false;
    }
}
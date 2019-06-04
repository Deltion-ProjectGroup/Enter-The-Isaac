using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GunMagician : MonoBehaviour
{
    [SerializeField] float flyHeight = 2;
    RippleEffect mainCamRipple;
    Vector3 startScale = Vector3.zero;
    ShakeCam shakeCam;
    Shake selfShake;
    Camera mainCam;
    Collider col;
    Transform player;
    bool isDoingSomething = false;
    [Header("Teleport")]
    [SerializeField] GameObject teleportLandParticle;
    [SerializeField] GameObject teleportLocationsParent;
    [Header("Laser")]
    [SerializeField] ParticleSystem speedLines;
    [SerializeField] LensFlare laserFlare;
    [SerializeField] GameObject laser;
    [SerializeField] GameObject anounceLaser;
    [SerializeField] float laserAimTime = 1;
    [SerializeField] float laserAnounceTime = 0.4f;
    [SerializeField] float laserShootTime = 2;
    [SerializeField] float laserRecoverTime = 0.2f;
    [SerializeField] float lookAtPlayerSpeed = 1;

    void Start()
    {
        flyHeight += transform.position.y;
        mainCamRipple = Camera.main.GetComponent<RippleEffect>();
        shakeCam = GetComponent<ShakeCam>();
        selfShake = GetComponent<Shake>();
        mainCam = Camera.main;
        startScale = transform.localScale;
        col = GetComponent<Collider>();
        player = FindObjectOfType<PlayerController>().transform;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) == true && isDoingSomething == false)
        {
            // Teleport();
            LaserAttack();
        }
    }
    public void Teleport()
    {
        isDoingSomething = true;
        StartCoroutine(TeleportCoroutine());
    }

    IEnumerator TeleportCoroutine()
    {
        col.enabled = false;
        shakeCam.SmallShake();
        transform.localScale = new Vector3(transform.localScale.x * 0.3f, transform.localScale.y * 3, transform.localScale.z * 0.3f);
        yield return new WaitForSeconds(0.03f);
        transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(0.25f);
        transform.position = GetRandomLocation();
        mainCamRipple.Emit(new Vector2(0.5f, 0.5f));
        yield return new WaitForSeconds(0.03f);
        transform.localScale = startScale;
        transform.localScale = new Vector3(transform.localScale.x * 0.3f, transform.localScale.y * 3, transform.localScale.z * 0.3f);
        shakeCam.MediumShake();
        Instantiate(teleportLandParticle, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.03f);
        transform.localScale = startScale;
        yield return new WaitForSeconds(0.2f);
        col.enabled = true;
        isDoingSomething = false;
    }

    Vector3 GetRandomLocation()
    {
        int random = Random.Range(0, teleportLocationsParent.transform.childCount);
        if (transform.position == teleportLocationsParent.transform.GetChild(random).position)
        {
            random = (int)Mathf.Repeat(random + 1, teleportLocationsParent.transform.childCount);
        }
        return teleportLocationsParent.transform.GetChild(random).position;
    }

    public void LaserAttack()
    {
        isDoingSomething = true;
        StartCoroutine(LaserCoroutine());
    }

    IEnumerator LaserCoroutine()
    {
        //aim
        InvokeRepeating("SmoothLookAtPlayer", 0, 0.01f);
        yield return new WaitForSeconds(laserAimTime);
        //anounce
        laserFlare.enabled = true;
        shakeCam.CustomShake(laserAnounceTime, 0.3f);
        anounceLaser.SetActive(true);
        selfShake.CustomShake(laserAnounceTime, 1);
        yield return new WaitForSeconds(laserAnounceTime);
        //fire
        anounceLaser.SetActive(false);
        speedLines.Play();
        shakeCam.CustomShake(laserShootTime, 1);
        mainCam.fieldOfView = 80;
        laser.SetActive(true);
        yield return new WaitForSeconds(laserShootTime);
        //stop & recover
        CancelInvoke("SmoothLookAtPlayer");
        speedLines.Stop();
        laserFlare.enabled = false;
        laser.SetActive(false);
        yield return new WaitForSeconds(laserRecoverTime);
        //end
        isDoingSomething = false;
    }

    void SmoothLookAtPlayer()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - new Vector3(player.position.x, transform.position.y, player.position.z), Vector3.up) * Quaternion.Euler(0, 180, 0), Time.deltaTime * lookAtPlayerSpeed);

    }

}

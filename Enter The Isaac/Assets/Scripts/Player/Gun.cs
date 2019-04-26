using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class Gun : MonoBehaviour
{
    [SerializeField] string input = "Fire1";
    [SerializeField] string reloadInput = "Fire2";
    [SerializeField] string scrollInput = "Mouse ScrollWheel";
    GunType gunType;
    [SerializeField] GunType[] guns;
    List<int> ammoStore = new List<int>();
    List<int> magazineStore = new List<int>();
    int curGun = 0;
    GunType lastGunType = null;
    Shake camShake;
    int curAmmo = 0;
    int totalMaxAmmo = 0;
    SoundSpawn soundSpawner;
    [SerializeField] PlayerController player;
    Vector3 startPos;
    [SerializeField] float recoilSpeed = 10;
    //dingen om Maurits te helpen
    [Space]
    [Header("Event activates when you press shoot")]
    [Header("Maurits, kijk hier!")]
    public UnityEvent inputEvent;
    [Header("Event activates when spawning a bullet")]
    public UnityEvent spawnEvent;
    [Header("Delegates inputDel en SpawnDel activeren ook, ze zijn public")]
    public GameObject lastSpawnedBullet;
    public delegate void gunDelegate();
    public gunDelegate inputDel;//<-------------- Maurits dit zijn ze
    public gunDelegate spawnDel;//<-------------- En hier
    [Header("Delete later, for presentation")]
    [SerializeField] Material[] mat;
    [SerializeField] Text[] uiElements;

    void Start()
    {
        startPos = transform.localPosition;
        gunType = guns[curGun];
        ammoStore.Clear();
        for (int i = 0; i < guns.Length; i++)
        {
            ammoStore.Add(guns[i].magazineSize);
        }
        for (int i = 0; i < guns.Length; i++)
        {
            magazineStore.Add(guns[i].maxAmmo);
        }
    }
    void StartGun()
    {
        camShake = Camera.main.GetComponent<Shake>();
        totalMaxAmmo = magazineStore[curGun];
        curAmmo = ammoStore[curGun];
        soundSpawner = FindObjectOfType<SoundSpawn>();
        Instantiate(gunType.gunModel, transform.GetChild(0).position, transform.GetChild(0).rotation, transform);
        Destroy(transform.GetChild(0).gameObject);
    }

    void SetPresentationUI()
    {
        //hardcoded because this is used for debugging.
        uiElements[0].text = "" + curAmmo;
        uiElements[1].text = "" + gunType.magazineSize;
        uiElements[2].text = "" + gunType.maxAmmo;
        uiElements[3].text = gunType.name;
        uiElements[4].text = totalMaxAmmo + "/";
    }

    public void CheckInput()
    {
        if (Input.GetButtonDown(input) == true && IsInvoking("Reload") == false && IsInvoking("IgnoreInput") == false)
        {
            InvokeRepeating("Shoot", 0, gunType.fireRate);
        }
        if (Input.GetButtonDown(reloadInput))
        {
            CancelInvoke("Shoot");
            Invoke("Reload", gunType.reloadTime);
            GetComponent<Renderer>().material = mat[1];
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * recoilSpeed);
        SwitchGun();
    }

    void SwitchGun()
    {
        if (Input.GetAxis(scrollInput) != 0)
        {
            ammoStore[curGun] = curAmmo;
            curGun += (int)(Input.GetAxis(scrollInput) * 10);
            if (curGun < 0)
            {
                curGun = guns.Length - 1;
            }
            else if (curGun > guns.Length - 1)
            {
                curGun = 0;
            }
            gunType = guns[curGun];
            GetComponent<Renderer>().material = mat[0];
        }
    }

    void IgnoreInput()
    {
        //this is an invoke function
    }

    void Update()
    {
        //updates gun, for when picking up a new one, or on start
        if (lastGunType != gunType)
        {
            StartGun();
            lastGunType = gunType;
        }

        if (IsInvoking("Shoot") == true)
        {
            if (IsInvoking("Reload") == true)
            {
                CancelInvoke("Shoot");
            }
        }
        SetPresentationUI();
        //rotating back
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * 10);
    }

    public void Shoot()
    {
        if (Input.GetAxis(input) != 0)
        {
            if (curAmmo >= 1)
            {
                ShootEvents();
            }
            else
            {
                //stop shooting and reload
                CancelInvoke("Shoot");
                //should you reload automatically? If so, slash the if here
                if (Input.GetButtonDown(input) == true)
                {
                    Invoke("Reload", gunType.reloadTime);
                    GetComponent<Renderer>().material = mat[1];
                }
            }
        }
        else
        {
            CancelInvoke("Shoot");
        }
    }

    void ShootEvents()
    {

        if (soundSpawner != null)
        {
            soundSpawner.SpawnEffect(gunType.sound);
        }
        for (int i = 0; i < gunType.bulletCount; i++)
        {
            float accuracy = Random.Range(-gunType.accuracy / 2, gunType.accuracy / 2);
            GameObject bullet = Instantiate(gunType.projectileModel, transform.position, Quaternion.Euler(0, transform.parent.eulerAngles.y + accuracy, 0));
            if (Vector3.Distance(transform.position, player.crosshair.position) > 1)
            {
                //basically this code feels more precise, exept when the crosshair is close to the player
                bullet.transform.LookAt(player.crosshair.position);
            }
            bullet.transform.localEulerAngles = new Vector3(0,bullet.transform.localEulerAngles.y,bullet.transform.localEulerAngles.z);
            bullet.transform.rotation *= Quaternion.Euler(0, accuracy + 180, 0);
            bullet.transform.position -= bullet.transform.forward * gunType.forwardStart;
            float rngSpeed = Random.Range(gunType.bulletSpeed.x, gunType.bulletSpeed.y);
            bullet.GetComponent<BulletMove>().speed = rngSpeed;
            bullet.GetComponent<BulletMove>().ricoshet = gunType.ricochet;
            if (bullet.GetComponent<BulletMoveDecelerate>() != null)
            {
                bullet.GetComponent<BulletMoveDecelerate>().decelerationSpeed = gunType.bulletDecelerationSpeed;
            }
            bullet.GetComponent<Hurtbox>().damage = gunType.dmg;
            bullet.GetComponent<Hurtbox>().team = 0;
            bullet.GetComponent<Hurtbox>().destroyOnHit = !gunType.pierce;
            bullet.GetComponent<BulletMove>().destroyOnRayHit = !gunType.pierce;
            Destroy(bullet, gunType.lifeTime);
            lastSpawnedBullet = bullet;
            spawnEvent.Invoke();
            if (spawnDel != null)
            {
                spawnDel();//<--------------------------- Maurits hier activeert spawnDel, in de spawn forloop voor de bullets.
            }
            lastSpawnedBullet = null;
        }
        if (gunType.muzzleFlash != null)
        {
            Instantiate(gunType.muzzleFlash, transform.position, transform.rotation, transform);
        }
        Camera.main.fieldOfView = gunType.camFov;
        transform.localPosition = startPos + (Vector3.forward * gunType.recoil);
        transform.localEulerAngles = new Vector3(0, 180, 0);
        transform.Rotate(Random.Range(-gunType.recoil, gunType.recoil) * 30, Random.Range(-gunType.recoil, gunType.recoil) * 30, 0);
        player.moveV3 += player.transform.forward * gunType.kickBack;
        camShake.CustomShake(gunType.screenShakeTime, gunType.screenshakeStrength);
        curAmmo -= 1;
        if (curAmmo < 1)
        {
            GetComponent<Renderer>().material = mat[2];
        }
        //so you can't spam the bullets
        Invoke("IgnoreInput", gunType.fireRate);
        inputEvent.Invoke();
        if (inputDel != null)
        {
            inputDel();//<------------------------------ Maurtis hier activeert inputDel
        }

    }

    public void Reload()
    {
        GetComponent<Renderer>().material = mat[0];
        if (totalMaxAmmo > gunType.magazineSize)
        {
            totalMaxAmmo -= gunType.magazineSize - curAmmo;
            curAmmo = gunType.magazineSize;
        }
        else if (totalMaxAmmo != 0)
        {
            curAmmo = totalMaxAmmo;
            totalMaxAmmo = 0;
        }
        magazineStore[curGun] = totalMaxAmmo;
    }
}

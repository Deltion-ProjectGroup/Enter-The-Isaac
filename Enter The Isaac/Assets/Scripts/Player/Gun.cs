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
    GunType gunClone;

    [SerializeField] GunType[] guns;
    List<int> ammoStore = new List<int>();
    List<int> magazineStore = new List<int>();
    int curGun = 0;
    GunType lastGunType = null;
    Shake camShake;
    int curAmmo = 0;
    int totalMaxAmmo = 0;
    SoundSpawn soundSpawner;
    [SerializeField] Vector3 angle = new Vector3(0, 180, 0);
    [SerializeField] PlayerController player;
    Vector3 startPos;
    [SerializeField] float recoilSpeed = 10;
    //dingen om Maurits te helpen
    [Space]
    [Header("Event activates when you press shoot")]
    public UnityEvent inputEvent;
    [Header("Event activates when spawning a bullet")]
    public UnityEvent spawnEvent;
    public GameObject lastSpawnedBullet;
    public delegate void gunDelegate();
    public gunDelegate gunDel;
    gunDelegate lastGunDel;//used to check if gunDel has to update the stats;
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
        gunClone = Instantiate(guns[curGun]);
    }
    void StartGun()
    {
        //for stat changes
        Destroy(gunClone);
        gunClone = Instantiate(guns[curGun]);
        if (gunDel != null)
        {
            gunDel();
        }

        //normal stuff
        camShake = Camera.main.GetComponent<Shake>();
        totalMaxAmmo = magazineStore[curGun];
        curAmmo = ammoStore[curGun];
        soundSpawner = FindObjectOfType<SoundSpawn>();
        Instantiate(gunType.gunModel, transform.GetChild(0).position, transform.GetChild(0).rotation, transform);
        Destroy(transform.GetChild(0).gameObject);

        //update checkers
        lastGunDel = gunDel;
        lastGunType = gunType;
    }

    void SetPresentationUI()
    {
        //hardcoded because this is used for debugging.
        uiElements[0].text = "" + curAmmo;
        uiElements[1].text = "" + gunClone.magazineSize;
        uiElements[2].text = "" + gunClone.maxAmmo;
        uiElements[3].text = gunType.name;
        uiElements[4].text = totalMaxAmmo + "/";
    }

    public void CheckInput()
    {
        if (Input.GetButtonDown(input) == true && IsInvoking("Reload") == false && IsInvoking("IgnoreInput") == false)
        {
            InvokeRepeating("Shoot", 0, gunClone.fireRate);
        }
        if (Input.GetButtonDown(reloadInput))
        {
            CancelInvoke("Shoot");
            Invoke("Reload", gunClone.reloadTime);
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
        }
        //uodates gun when a stat has changed
        if (gunDel != lastGunDel)
        {
            StartGun();
            print("it's happening!");
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
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(angle), Time.deltaTime * 10);
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
                    Invoke("Reload", gunClone.reloadTime);
                    GetComponent<Renderer>().material = mat[1];
                }
            }
        }
        else
        {
            CancelInvoke("Shoot");
        }
    }

    void BaseShootEvents()
    {
        if (soundSpawner != null)
        {
            soundSpawner.SpawnEffect(gunClone.sound);
        }
        if (gunClone.muzzleFlash != null)
        {
            Instantiate(gunClone.muzzleFlash, transform.position, transform.rotation, transform);
        }
        Camera.main.fieldOfView = gunClone.camFov;
        transform.localPosition = startPos + (Vector3.forward * gunClone.recoil);
        transform.localEulerAngles = angle;
        transform.Rotate(Random.Range(-gunClone.recoil, gunClone.recoil) * 30, Random.Range(-gunClone.recoil, gunClone.recoil) * 30, 0);
        player.moveV3 += player.transform.forward * gunType.kickBack;
        camShake.CustomShake(gunClone.screenShakeTime, gunClone.screenshakeStrength);
        //this is so you can't spam the bullets
        Invoke("IgnoreInput", gunClone.fireRate);
        inputEvent.Invoke();
    }

    void NormalBulletEvents()
    {
        for (int i = 0; i < gunClone.bulletCount; i++)
        {
            float accuracy = Random.Range(-gunClone.accuracy / 2, gunClone.accuracy / 2);
            GameObject bullet = Instantiate(gunClone.projectileModel, transform.position, Quaternion.Euler(0, transform.parent.eulerAngles.y + accuracy, 0));
            if (Vector3.Distance(transform.position, player.crosshair.position) > 1)
            {
                //basically this code feels more precise, exept when the crosshair is close to the player
                bullet.transform.LookAt(player.crosshair.position);
            }

            bullet.transform.rotation *= Quaternion.Euler(0, accuracy + 180, 0);
            bullet.transform.Rotate(-bullet.transform.localEulerAngles.x, 0, 0);
            bullet.transform.position -= bullet.transform.forward * gunClone.forwardStart;

            float rngSpeed = Random.Range(gunClone.bulletSpeed.x, gunClone.bulletSpeed.y);
            bullet.GetComponent<BulletMove>().speed = rngSpeed;
            bullet.GetComponent<BulletMove>().ricoshet = gunClone.ricochet;

            if (bullet.GetComponent<BulletMoveDecelerate>() != null)
            {
                bullet.GetComponent<BulletMoveDecelerate>().decelerationSpeed = gunClone.bulletDecelerationSpeed;
            }

            bullet.GetComponent<Hurtbox>().damage = gunClone.dmg;
            bullet.GetComponent<Hurtbox>().team = 0;
            bullet.GetComponent<Hurtbox>().destroyOnHit = !gunClone.pierce;
            bullet.GetComponent<BulletMove>().destroyOnRayHit = !gunClone.pierce;
            Destroy(bullet, gunClone.lifeTime);
            lastSpawnedBullet = bullet;
            spawnEvent.Invoke();
            lastSpawnedBullet = null;
            if (gunClone.parentToGun == true)
            {
                bullet.transform.SetParent(transform, true);
                bullet.transform.localEulerAngles = new Vector3(0, bullet.transform.localEulerAngles.y, bullet.transform.localEulerAngles.z);
            }
        }
        curAmmo -= 1;
        if (curAmmo < 1)
        {
            GetComponent<Renderer>().material = mat[2];
        }
    }

    void LaserBulletEvents()
    {
        StopAllCoroutines();
        float accuracy = Random.Range(-gunClone.accuracy / 2, gunClone.accuracy / 2);
        GameObject bullet = Instantiate(gunClone.projectileModel, transform.position, transform.rotation);
        if (Vector3.Distance(transform.position, player.crosshair.position) > 1)
        {
            //basically this code feels more precise, exept when the crosshair is close to the player
            bullet.transform.LookAt(player.crosshair.position);
        }

        bullet.transform.rotation *= Quaternion.Euler(0, accuracy, 0);
        bullet.transform.Rotate(-bullet.transform.localEulerAngles.x, 0, 0);
        bullet.transform.position -= bullet.transform.forward * gunClone.forwardStart;

        bullet.transform.position -= bullet.transform.forward * -gunClone.forwardStart;
        float rngSpeed = Random.Range(gunClone.bulletSpeed.x, gunClone.bulletSpeed.y);

        LineHurtBox line = bullet.GetComponent<LineHurtBox>();
        line.pierce = gunClone.pierce;
        line.damage = gunClone.dmg;
        line.activeFrames = gunClone.activeFrames;
        line.damageFrames = gunClone.damageFrames;
        line.team = 0;
        line.destroyOnHit = false;
        line.pierce = gunClone.pierce;

        lastSpawnedBullet = bullet;
        spawnEvent.Invoke();
        lastSpawnedBullet = null;
        if (gunClone.parentToGun == true)
        {
            bullet.transform.SetParent(transform, true);
            bullet.transform.localEulerAngles = new Vector3(0, bullet.transform.localEulerAngles.y, bullet.transform.localEulerAngles.z);
            bullet.transform.rotation *= Quaternion.Euler(0, accuracy, 0);
        }
    }
    void ShootEvents()
    {

        BaseShootEvents();

        if (gunClone.projectileModel.GetComponent<BulletMove>() != null)
        {
            NormalBulletEvents();
        }
        else if (gunClone.projectileModel.GetComponent<LineHurtBox>() != null)
        {
            LaserBulletEvents();//inheretance just didn't fit, I'd have to activate other scripts, which is way too much effort, in comarison to just using functions
        }

    }

    public void Reload()
    {
        GetComponent<Renderer>().material = mat[0];
        if (totalMaxAmmo > gunClone.magazineSize)
        {
            totalMaxAmmo -= gunClone.magazineSize - curAmmo;
            curAmmo = gunClone.magazineSize;
        }
        else if (totalMaxAmmo != 0)
        {
            curAmmo = totalMaxAmmo;
            totalMaxAmmo = 0;
        }
        magazineStore[curGun] = totalMaxAmmo;
    }
}

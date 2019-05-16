using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class Gun : MonoBehaviour
{
    GunType gunType;
    [HideInInspector] public GunType gunClone;
    [HideInInspector] public float shootInput = 0;
    [HideInInspector] public bool shootFirstFrame = false;
    public GunType[] guns;
    //List<int> ammoStore = new List<int>();
    //List<int> magazineStore = new List<int>();
    int curGun = 0;
    GunType lastGunType = null;
    Shake camShake;
    [HideInInspector] public int curAmmo = 0;
    int totalMaxAmmo = 0;
    SoundSpawn soundSpawner;
    public Vector3 angle = new Vector3(0, 0, 0);
    [SerializeField] PlayerController player;
    [HideInInspector] public Vector3 startPos;
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
    [SerializeField] Text[] uiElements;

    void Start()
    {
        startPos = transform.localPosition;
        gunType = guns[curGun];

        gunClone = Instantiate(guns[curGun]);
        if (player != null)
        {
            gunType = player.guns[player.curGun];
        }
    }
    public void StartGun()
    {
        if (player != null)
        {
            gunType = player.guns[player.curGun];
        }
        //for stat changes
        PlayerController lastPlayer = player;
        player = FindObjectOfType<PlayerController>();
        Destroy(gunClone);
        gunClone = Instantiate(player.guns[player.curGun]);
        gunDelegate last = gunDel;
        if (player.gunDel != null)
        {
            gunDel = player.gunDel;
        }
        else
        {
            gunDel = null;
        }
        Gun lastGun = player.gun;
        player.gun = this;
        if (gunDel != null)
        {
            gunDel();
        }
        player.gun = lastGun;
        gunDel = last;
        player = lastPlayer;

        //normal stuff
        camShake = Camera.main.GetComponent<Shake>();
        if (player != null)
        {
            totalMaxAmmo = player.magazineStore[player.curGun];
            curAmmo = player.ammoStore[player.curGun];
        }
        else
        {
            //aka if constructorhat
            totalMaxAmmo = 0;
            curAmmo = gunClone.maxAmmo;
        }
        soundSpawner = FindObjectOfType<SoundSpawn>();
        Instantiate(gunType.gunModel, transform.GetChild(0).position, transform.GetChild(0).rotation, transform);
        Destroy(transform.GetChild(0).gameObject);

        //update checkers
        lastGunDel = gunDel;
        lastGunType = gunType;
    }

    /*void SetPresentationUI()
    {
        //hardcoded because this is used for debugging.
        uiElements[0].text = "" + curAmmo;
        uiElements[1].text = "" + gunClone.magazineSize;
        uiElements[2].text = "" + gunClone.maxAmmo;
        uiElements[3].text = gunType.name;
        uiElements[4].text = totalMaxAmmo + "/";
    } */

    public void GetShootInput()
    {
        if (IsInvoking("Reload") == false && IsInvoking("IgnoreInput") == false && IsInvoking("Shoot") == false)
        {
            InvokeRepeating("Shoot", 0, gunClone.fireRate);
            if (player != null && player.onShootDel != null)
            {
                player.onShootDel();
            }
        }
    }
    public void GetReloadInput()
    {
        CancelInvoke("Shoot");
        Invoke("Reload", gunClone.reloadTime);
        transform.Rotate(0,0,90);
    }

    public void GetSwitchGunInput(float scrollInput)
    {
        if (player != null)
        {
            player.ammoStore[player.curGun] = curAmmo;
        }
        curGun += (int)(scrollInput * 10);
        if (curGun < 0)
        {
            curGun = guns.Length - 1;
        }
        else if (curGun > guns.Length - 1)
        {
            curGun = 0;
        }
        gunType = guns[curGun];

    }

    void IgnoreInput()
    {
        //this is an invoke function
    }

    void Update()
    {
        if (player != null)
        {
            gunType = player.guns[player.curGun];
        }
        //updates gun, for when picking up a new one, or on start
        if (lastGunType != gunType)
        {
            StartGun();
        }
        //uodates gun when a stat has changed
        if (gunDel != lastGunDel)
        {
            StartGun();
        }

        if (IsInvoking("Shoot") == true)
        {
            if (IsInvoking("Reload") == true)
            {
                CancelInvoke("Shoot");
            }
        }
        //going back from recoil back
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * recoilSpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(angle), Time.deltaTime * 20);
    }

    public void Shoot()
    {
        if (shootInput != 0 && GameObject.FindGameObjectWithTag("LaserHold") == null)
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
                if (shootFirstFrame == true)
                {
                    Invoke("Reload", gunClone.reloadTime);
                    transform.Rotate(0,0,90);
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
        if (soundSpawner != null && gunClone.sound != null)
        {
            soundSpawner.SpawnEffect(gunClone.sound);
        }
        if (gunClone.muzzleFlash != null)
        {
            Instantiate(gunClone.muzzleFlash, transform.GetChild(0).Find("SpawnPoint").position, gunClone.muzzleFlash.transform.rotation * transform.rotation, transform);
        }
        Camera.main.fieldOfView = gunClone.camFov;
        transform.localPosition = startPos + (Vector3.forward * gunClone.recoil);
        transform.localEulerAngles = angle;
        transform.Rotate(Random.Range(-gunClone.recoil, gunClone.recoil) * 30, Random.Range(-gunClone.recoil, gunClone.recoil) * 30, 0);
        if (player != null)
        {
            player.moveV3 += player.transform.forward * gunType.kickBack;
        }
        camShake.CustomShake(gunClone.screenShakeTime, gunClone.screenshakeStrength);
        //this is so you can't spam the bullets
        Invoke("IgnoreInput", gunClone.fireRate);
        if (inputEvent != null)
        {
            inputEvent.Invoke();
        }
    }

    void NormalBulletEvents()
    {
        for (int i = 0; i < gunClone.bulletCount; i++)
        {
            float accuracy = Random.Range(-gunClone.accuracy / 2, gunClone.accuracy / 2);
            GameObject bullet = Instantiate(gunClone.projectileModel, transform.GetChild(0).Find("SpawnPoint").position, Quaternion.Euler(0, accuracy + 180, 0));
            if (transform.parent != null && transform.parent.GetComponent<PlayerController>() != null)
            {
                if (Vector3.Distance(transform.position, player.crosshair.position) > 1)
                {
                    //basically this code feels more precise, exept when the crosshair is close to the player
                    bullet.transform.LookAt(player.crosshair.position);
                }
            }
            else
            {
                bullet.transform.rotation = transform.rotation;
                bullet.transform.Rotate(0, 180, 0);
            }

            bullet.transform.rotation *= Quaternion.Euler(0, accuracy + 180, 0);
            bullet.transform.Rotate(-bullet.transform.localEulerAngles.x, 0, 0);

            if (Physics.Raycast(bullet.transform.position - (bullet.transform.forward * -gunClone.forwardStart), -bullet.transform.forward, gunClone.forwardStart) == false)
            {
                //bullet.transform.position -= bullet.transform.forward * gunClone.forwardStart;
            }

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
            if (spawnEvent != null)
            {
                spawnEvent.Invoke();
            }
            lastSpawnedBullet = null;
            if (gunClone.parentToGun == true)
            {
                bullet.transform.SetParent(transform, true);
                bullet.transform.localEulerAngles = new Vector3(0, bullet.transform.localEulerAngles.y, bullet.transform.localEulerAngles.z);
            }
            if (gunClone.destroyOnInputUp == true)
            {
                //this should only happen when the gun is being shot by the player
                if (transform.parent != null && transform.parent.GetComponent<PlayerController>() != null)
                {
                    bullet.AddComponent<DestroyOnButtonUp>().destroyInput = player.shootInput;
                }
                bullet.tag = "LaserHold";
                CancelInvoke("LaserHoldAmmo");
                InvokeRepeating("LaserHoldAmmo", 0, 1f);
            }
        }
        curAmmo -= 1;

    }

    void LaserBulletEvents()
    {
        StopAllCoroutines();
        float accuracy = Random.Range(-gunClone.accuracy / 2, gunClone.accuracy / 2);
        GameObject bullet = Instantiate(gunClone.projectileModel, transform.GetChild(0).Find("SpawnPoint").position, transform.rotation * Quaternion.Euler(0, 180, 0));

        bullet.transform.rotation *= Quaternion.Euler(0, accuracy, 0);

        bullet.transform.position -= bullet.transform.forward * -gunClone.forwardStart;

        LineHurtBox line = bullet.GetComponent<LineHurtBox>();
        line.pierce = gunClone.pierce;
        line.damage = gunClone.dmg;
        line.activeFrames = gunClone.activeFrames;
        line.damageFrames = gunClone.damageFrames;
        line.team = 0;
        line.destroyOnHit = false;
        line.pierce = gunClone.pierce;

        if (gunClone.destroyOnInputUp == true)
        {
            //this should only happen when the gun is being shot by the player
            if (transform.parent != null && transform.parent.GetComponent<PlayerController>() != null)
            {
                bullet.AddComponent<DestroyOnButtonUp>().destroyInput = player.shootInput;
            }
            bullet.tag = "LaserHold";
            CancelInvoke("LaserHoldAmmo");
            InvokeRepeating("LaserHoldAmmo", 0, 1f);
        }
        else
        {
            curAmmo -= 1;

            Destroy(bullet.gameObject, gunClone.lifeTime);
        }
        lastSpawnedBullet = bullet;
        if (spawnEvent != null)
        {
            spawnEvent.Invoke();
        }
        lastSpawnedBullet = null;
        if (gunClone.parentToGun == true)
        {
            bullet.transform.SetParent(transform, true);
        }
        else if (transform.parent != null && transform.parent.GetComponent<PlayerController>() != null)
        {
            if (Vector3.Distance(transform.position, player.crosshair.position) > 1)
            {
                bullet.transform.LookAt(player.crosshair.position);
            }
            bullet.transform.Rotate(-bullet.transform.localEulerAngles.x, 0, 0);
        }
    }

    void LaserHoldAmmo()
    {
        if (GameObject.FindGameObjectWithTag("LaserHold") == null)
        {
            CancelInvoke("LaserHoldAmmo");
        }
        else
        {
            curAmmo -= 1;
            if (curAmmo < 0)//best solution,in case you had 1 ammo left
            {
                curAmmo = 0;
                Destroy(GameObject.FindGameObjectWithTag("LaserHold"));
                CancelInvoke("LaserHoldAmmo");
            }
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
        if (player != null)
        {
            player.magazineStore[player.curGun] = totalMaxAmmo;
        }
    }
}

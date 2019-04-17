using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] string input = "Fire1";
    [SerializeField] string reloadInput = "Fire2";
    [SerializeField] GunType gunType;
    GunType lastGunType = null;
    Shake camShake;
    [SerializeField] int curAmmo = 0;
    int totalMaxAmmo = 0;
    SoundSpawn soundSpawner;
    [SerializeField] PlayerController player;
    [Header("Delete later, for presentation")]
    [SerializeField] Material[] mat;

    void StartGun()
    {
        camShake = Camera.main.GetComponent<Shake>();
        totalMaxAmmo = gunType.maxAmmo;
        curAmmo = gunType.magazineSize;
        soundSpawner = FindObjectOfType<SoundSpawn>();
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
    }

    public void Shoot()
    {
        if (Input.GetAxis(input) != 0)
        {
            if (curAmmo >= gunType.bulletCount)
            {
                // print("pew");
                if (soundSpawner != null)
                {
                    soundSpawner.SpawnEffect(gunType.sound);
                }
                for (int i = 0; i < gunType.bulletCount; i++)
                {
                    Camera.main.fieldOfView = gunType.camFov;
                    float accuracy = Random.Range(-gunType.accuracy / 2, gunType.accuracy / 2);
                    GameObject bullet = Instantiate(gunType.projectileModel, transform.position, Quaternion.Euler(0, transform.parent.eulerAngles.y + accuracy, 0));
                    bullet.transform.position -= bullet.transform.forward * gunType.forwardStart;
                    float rngSpeed = Random.Range(gunType.bulletSpeed.x, gunType.bulletSpeed.y);
                    bullet.GetComponent<BulletMove>().speed = rngSpeed;
                    if (bullet.GetComponent<BulletMoveDecelerate>() != null)
                    {
                        bullet.GetComponent<BulletMoveDecelerate>().decelerationSpeed = gunType.bulletDecelerationSpeed;
                    }
                    bullet.GetComponent<Hurtbox>().damage = gunType.dmg;
                    bullet.GetComponent<Hurtbox>().team = 0;
                    Destroy(bullet, gunType.lifeTime);
                }
                if (gunType.muzzleFlash != null)
                {
                    Instantiate(gunType.muzzleFlash, transform.position, transform.rotation, transform);
                }
                player.moveV3 += player.transform.forward * gunType.kickBack;
                camShake.CustomShake(gunType.screenShakeTime, gunType.screenshakeStrength);
                curAmmo -= gunType.bulletCount;
                totalMaxAmmo -= gunType.bulletCount;
                if (curAmmo < gunType.bulletCount)
                {
                    GetComponent<Renderer>().material = mat[2];
                }
                //so you cant spam the bullets
                Invoke("IgnoreInput", gunType.fireRate);
            }
            else
            {
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

    public void Reload()
    {
        GetComponent<Renderer>().material = mat[0];
        if (totalMaxAmmo > gunType.magazineSize)
        {
            curAmmo = gunType.magazineSize;
            totalMaxAmmo -= gunType.magazineSize;
        }
        else if (totalMaxAmmo != 0)
        {
            curAmmo = totalMaxAmmo;
            totalMaxAmmo = 0;
        }
    }
}

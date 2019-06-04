using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructorHat : HATBase
{
    [SerializeField] GameObject turretPrefab;
    [SerializeField] float spawnDistance = 5;
    [SerializeField] GunType testGun;
    GameObject gHelp;
    int iHelp = 0;
    [SerializeField] int maxTurrets = 5;
    [SerializeField] float divider = 1;
    Shake shake;
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        player.transform.GetChild(0).GetComponent<IKTest1>().enabled = false;
        player.transform.GetChild(0).GetComponent<IKHoldGun>().enabled = false;
        player.onNoGunShootDel += SpawnGunTurret;
        player.gun.gameObject.SetActive(false);
        player.gun = null;
        player.gunDel += NoKickBack;
        shake = Camera.main.GetComponent<Shake>();
    }

    void NoKickBack()
    {
        player.gun.gunClone.recoil = 0;
        player.gun.gunClone.kickBack = 0;
        player.gun.gunClone.camFov = 70;
        player.gun.gunClone.screenShakeTime = 0;
        player.gun.gunClone.screenshakeStrength = 0;
    }

    void SpawnGunTurret()
    {
        testGun = player.guns[player.curGun];
        if (Input.GetButtonDown(player.shootInput) == true)
        {
            if (shake != null)
            {
            }
            int ammoUsed = Mathf.RoundToInt(player.guns[player.curGun].magazineSize / (maxTurrets * divider));
            print(ammoUsed);
            if (player.ammoStore[player.curGun] > ammoUsed)
            {
                shake.CustomShake(0.1f, 0.5f);
                player.ammoStore[player.curGun] -= ammoUsed;
                iHelp = ammoUsed;
                GameObject g = Instantiate(turretPrefab, player.transform.position + -player.transform.forward * spawnDistance, player.transform.rotation);
                gHelp = g;
                Invoke("RestSpawnTurret", Time.deltaTime);
            }
        }
        else
        {
            //player.Reload();
        }
    }

    void RestSpawnTurret()
    {
        Gun gun = gHelp.transform.GetChild(0).gameObject.AddComponent<Gun>();
        gun.myGun = new GunType();
        gun.myGun = testGun;
        gun.curAmmo = iHelp;//iHelp = ammoUsed
        gHelp = null;
    }

    public override void StopHat()
    {
        player.transform.GetChild(0).GetComponent<IKTest1>().enabled = true;
        player.transform.GetChild(0).GetComponent<IKHoldGun>().enabled = true;

    }
}

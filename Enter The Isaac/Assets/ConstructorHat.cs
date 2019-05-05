using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructorHat : HATBase
{
    [SerializeField] GameObject turretPrefab;
    [SerializeField] float spawnDistance = 5;
    [SerializeField] GunType testGun;
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        player.transform.GetChild(0).GetComponent<IKTest1>().enabled = false;
        player.transform.GetChild(0).GetComponent<IKHoldGun>().enabled = false;
        player.onNoGunShootDel += SpawnGunTurret;
        player.gun.gameObject.SetActive(false);
        player.gun = null;
    }

    void SpawnGunTurret()
    {
        if (Input.GetButtonDown(player.shootInput) == true)
        {
            GameObject g = Instantiate(turretPrefab, player.transform.position + -player.transform.forward * spawnDistance, player.transform.rotation);
            Gun gun = g.transform.GetChild(0).gameObject.AddComponent<Gun>();
            gun.guns = new GunType[1];
            gun.guns[0] = testGun;
          //  gun.GetSwitchGunInput(1);
        }
    }

    public override void StopHat()
    {
        player.transform.GetChild(0).GetComponent<IKTest1>().enabled = true;
        player.transform.GetChild(0).GetComponent<IKHoldGun>().enabled = true;

    }
}

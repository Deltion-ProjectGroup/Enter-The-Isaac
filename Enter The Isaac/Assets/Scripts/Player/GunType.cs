using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
firerate
damage bonus
magazine size (hoe vaak je mag schoeten voor herladen)
max ammo
reload time
bullet count
accuracy
bullet speed
projectile model
gun model
recoil (hoe ver je arm naar achter ketst als je schiet)
sound effect
screenshake
 */

[CreateAssetMenu(fileName = "New Gun", menuName = "GunType")]
public class GunType : ScriptableObject
{
    public float fireRate = 0;
    public float dmg = 0;
    public int magazineSize = 0;
    public int maxAmmo = 0;
    public float reloadTime = 0;
    public int bulletCount = 0;//how many spawn at the same time
    public float accuracy = 0;
    public float bulletSpeed = 0;
    public GameObject projectileModel;//incomplete
    public GameObject gunModel;//incomplete
    public float recoil = 0;//incomplete
    public AudioClip sound;//incomplete
    public float screenShakeTime = 0;
    public float screenshakeStrength = 0;

}

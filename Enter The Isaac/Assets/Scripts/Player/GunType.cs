using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "GunType")]
public class GunType : ScriptableObject
{
    [Header("Laser Only")]
    public int activeFrames = 0;
    public int damageFrames = 0;
    [Header("Shotgun Only")]
    public float bulletDecelerationSpeed = 10;
    [Header("Balancable Stats")]
    public float fireRate = 0;
    public float dmg = 0;
    public int magazineSize = 0;
    public int maxAmmo = 0;
    public float reloadTime = 0;
    public int bulletCount = 0;//how many spawn at the same time
    public float accuracy = 0;
    public float lifeTime = 5;
    public float forwardStart = 1;
    public bool pierce = false;
    public bool ghostBullet = false;
    public bool destroyOnInputUp = false;//incomplete
    public bool ricochet = false;//does not work with laser type guns
    public bool parentToGun = false;
    public Vector2 bulletSpeed = Vector2.zero;
    public GameObject projectileModel;
    public GameObject gunModel;
    [Header("Game Feel")]
    public float recoil = 0;
    public AudioClip sound;
    public GameObject[] muzzleFlash;
    public float screenShakeTime = 0;
    public float screenshakeStrength = 0;
    public float camFov = 50;
    public float kickBack = 0;

}

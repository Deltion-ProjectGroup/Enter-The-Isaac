﻿using System.Collections;
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
    public float lifeTime = 5;
    public float forwardStart = 1;
    public bool pierce = false;
    public bool ricochet = false;//does not work with laser type guns
    public bool parentToGun = false;//incomplete
    public Vector2 bulletSpeed = Vector2.zero;
    public float bulletDecelerationSpeed = 10;
    public GameObject projectileModel;
    public GameObject gunModel;
    public float recoil = 0;//works partially, no ik yet
    public AudioClip sound;
    public GameObject muzzleFlash;
    public float screenShakeTime = 0;
    public float screenshakeStrength = 0;
    public float camFov = 50;
    public float kickBack = 0;
    public int activeFrames = 0;
    public int damageFrames = 0;

}
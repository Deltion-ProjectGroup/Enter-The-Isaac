﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunUI : MonoBehaviour
{
    PlayerController player;
    [SerializeField] Text[] curGunName;
    [SerializeField] Text[] curGunAmmo;
    [SerializeField] Text[] curGunMagazine;
    [SerializeField] Text[] curGunMaxAmmo;
    [SerializeField] Text[] curGunMagazineLeft;
    [SerializeField] UIPercentBar bar;
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    void LateUpdate()
    {
        //this code does not make any logical sense because I'm dumb today, how are you? #brainfarts #whostilluseshashtags
        SetNameText();
        SetMagazineText();
        SetMagazineLeftText();
        SetMaxAmmoText();
        SetAmmoText();
        SetBar();
    }
    void SetBar(){
        if(player.IsInvoking("NoSwitchGun") == false){
        bar.SetPercent(player.magazineStore[player.curGun] / (float)player.gun.gunClone.maxAmmo * 100);
        }
    }

    void SetNameText()
    {
        string toSet = player.guns[player.curGun].name;
        for (int i = 0; i < curGunName.Length; i++)
        {
            curGunName[i].text = toSet;
        }
    }
    void SetAmmoText()
    {
        string toSet = player.guns[player.curGun].magazineSize + "";
        if (player.gun != null)
        {
            toSet = player.gun.gunClone.magazineSize + "";
        }
        for (int i = 0; i < curGunAmmo.Length; i++)
        {
            curGunAmmo[i].text = " / " + toSet;
        }
    }
    void SetMagazineText()
    {
        string toSet = player.magazineStore[player.curGun] + "";
        for (int i = 0; i < curGunMagazine.Length; i++)
        {
            curGunMagazine[i].text = toSet;
        }
    }
    void SetMaxAmmoText()
    {
        string toSet = player.guns[player.curGun].magazineSize + "";
        if (player.gun != null)
        {
            toSet = player.gun.curAmmo + "/";
        }
        for (int i = 0; i < curGunMaxAmmo.Length; i++)
        {
            curGunMaxAmmo[i].text = toSet;
        }
    }
    void SetMagazineLeftText()
    {
        string toSet = player.gun.gunClone.maxAmmo.ToString();
        for (int i = 0; i < curGunMagazineLeft.Length; i++)
        {
            curGunMagazineLeft[i].text = toSet;
        }
    }
}

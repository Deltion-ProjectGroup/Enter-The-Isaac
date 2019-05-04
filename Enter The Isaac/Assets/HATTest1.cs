using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HATTest1 : HATBase
{
    [SerializeField] float multiplyFireRate = 1.5f;
    [SerializeField] float multiplyDmg = 1.5f;
    [SerializeField] float divideHP = 2;
    void Start()
    {
        print("start lagann");
        player.gun.gunDel += DmgFRateHP;
        player.hitbox.GetComponent<Hitbox>().maxHealth /= divideHP;
        player.hitbox.GetComponent<Hitbox>().AddHealth(0);//apply maxhealth basically
    }

    void DmgFRateHP()
    {
        player.gun.gunClone.fireRate /= multiplyFireRate;
        player.gun.gunClone.dmg *= multiplyDmg;
    }

    public override void StopHat(){
        print("stop lagann");
        player.gun.gunDel -= DmgFRateHP;
        player.gun.gunClone.fireRate *= multiplyFireRate;
        player.gun.gunClone.dmg /= multiplyDmg;
        player.hitbox.GetComponent<Hitbox>().maxHealth *= divideHP;
        player.hitbox.GetComponent<Hitbox>().AddHealth(0);
        
    }
}

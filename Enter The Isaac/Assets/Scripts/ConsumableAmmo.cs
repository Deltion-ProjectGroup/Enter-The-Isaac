using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableAmmo : ConsumableBase
{
    void Start()
    {
        Initailize();
    }

    public override void Use()
    {
        FindObjectOfType<SoundSpawn>().SpawnEffect(soundEffect);
        if (player.gun != null)//aka if constructor hat
        {
            player.gun.curAmmo = Mathf.Min(player.gun.curAmmo + value, player.gun.gunClone.magazineSize);
        }
        player.magazineStore[player.curGun] = Mathf.Min(player.magazineStore[player.curGun] + value, player.guns[player.curGun].maxAmmo);
        Destroy(gameObject);
    }
}

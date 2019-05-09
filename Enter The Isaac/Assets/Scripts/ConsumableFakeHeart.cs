using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableFakeHeart : ConsumableBase
{
    void Start(){
        Initailize();
    }

    public override void Use()
    {
        FindObjectOfType<SoundSpawn>().SpawnEffect(soundEffect);
        Hitbox hit = player.hitbox.GetComponent<Hitbox>();
        hit.AddHealth(999);
        hit.fakeHealth += value;
        hit.fakeHealth = Mathf.Min(hit.fakeHealth,40 - hit.maxHealth);
        Destroy(gameObject);
    }
}

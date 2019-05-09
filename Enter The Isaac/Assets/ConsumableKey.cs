using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableKey : ConsumableBase
{
    void Start()
    {
        Initailize();
    }

    public override void Use()
    {
        FindObjectOfType<SoundSpawn>().SpawnEffect(soundEffect);
        player.keys = Mathf.Min(player.keys + value,99);
        Destroy(gameObject);
    }
}

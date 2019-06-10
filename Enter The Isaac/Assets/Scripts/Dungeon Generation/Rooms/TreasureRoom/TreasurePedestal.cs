using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasurePedestal : Spawner
{
    // Start is called before the first frame update
    void Start()
    {
        SpawnObject(GetRandomItem());
    }
    public GameObject GetRandomItem()
    {
        ChanceManager chanceManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChanceManager>();
        ChanceManager.ItemPoolHolder itemPoolHolder = null;
        switch(Random.Range(0, 2))
        {
            case 0:
                itemPoolHolder = chanceManager.itemPool;
                break;

            case 1:
                itemPoolHolder = chanceManager.weaponPool;
                break;
        }
        return itemPoolHolder.GetItemFromPool();
    }
}

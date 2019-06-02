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
        InGameManager inGameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InGameManager>();
        InGameManager.ItemPoolHolder itemPoolHolder = null;
        switch(Random.Range(0, 2))
        {
            case 0:
                itemPoolHolder = inGameManager.itemPool;
                break;

            case 1:
                itemPoolHolder = inGameManager.weaponPool;
                break;
        }
        return itemPoolHolder.GetItemFromPool();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasurePedestal : Spawner
{
    // Start is called before the first frame update
    void Start()
    {
        SpawnObject(GetItemFromPool());
    }
    public GameObject GetItemFromPool()
    {
        InGameManager inGameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InGameManager>();
        int randomNum = Random.Range(1, inGameManager.maxPercentage + 1);
        foreach(InGameManager.ItemPoolData data in inGameManager.itemPool)
        {
            print("YES");
            if (randomNum >= data.percentageBorders.x && randomNum <= data.percentageBorders.y)
            {
                if(data.items.Length > 0)
                {
                    return (data.items[Random.Range(0, data.items.Length)]);
                }
                break;
            }
        }
        return null;
    }
}

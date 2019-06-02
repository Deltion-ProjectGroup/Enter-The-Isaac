using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : BaseRoom
{
    public Spawner[] itemSpawners;
    public Spawner[] weaponSpawners;
    public Spawner[] consumableSpawners;
    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null)
    {
        base.Initialize(owner, parentRoom_, entrance);
        creator.shopCount++;
        creator.roomCount++;

        InitializeShop();
    }

    public void InitializeShop()
    {
        InGameManager inGameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InGameManager>();
        foreach (Spawner spawner in consumableSpawners)
        {
            GameObject item = spawner.SpawnObject(inGameManager.consumablePool.GetItemFromPool());
            ShopItem itemData = item.AddComponent<ShopItem>();
            itemData.Initialize(0);
        }
        foreach (Spawner spawner in itemSpawners)
        {
            GameObject item = spawner.SpawnObject(inGameManager.itemPool.GetItemFromPool());
            ShopItem itemData = item.AddComponent<ShopItem>();
            itemData.Initialize(0);
        }
        foreach (Spawner spawner in weaponSpawners)
        {
            GameObject item = spawner.SpawnObject(inGameManager.weaponPool.GetItemFromPool());
            ShopItem itemData = item.AddComponent<ShopItem>();
            itemData.Initialize(0);
        }
    }

    public override void SpawnRoom(DungeonConnectionPoint.ConnectionDirection wantedDir, Transform doorPoint)
    {
        creator.SpawnDungeonPartAlt(creator.hallways, wantedDir, gameObject, doorPoint, RoomTypes.Hallway);
    }
    public override void OnDestroyed()
    {
        base.OnDestroyed();
        creator.shopCount--;
        creator.roomCount--;
    }
}

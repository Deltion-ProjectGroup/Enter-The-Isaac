﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : BaseRoom
{
    public GameObject priceObject;
    public int heightFromObject;

    public Spawner[] itemSpawners;
    public Spawner[] weaponSpawners;
    public Spawner[] consumableSpawners;
    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null, GameObject pointConnectingTo = null)
    {
        base.Initialize(owner, parentRoom_, entrance, pointConnectingTo);
        creator.shopCount++;
        creator.roomCount++;

        InitializeShop();
    }

    public void InitializeShop()
    {
        ChanceManager chanceManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChanceManager>();
        foreach (Spawner spawner in consumableSpawners)
        {
            GameObject item = spawner.SpawnObject(chanceManager.consumablePool.GetInstanceFromPool());
            ShopItem itemData = item.AddComponent<ShopItem>();
            Vector2 cost = Vector2.zero;
            cost.x = item.GetComponent<Item>().minValue;
            cost.y = item.GetComponent<Item>().maxValue;
            itemData.Initialize((int)Random.Range(cost.x, cost.y + 1));
        }
        foreach (Spawner spawner in itemSpawners)
        {
            GameObject item = spawner.SpawnObject(chanceManager.itemPool.GetInstanceFromPool());
            ShopItem itemData = item.AddComponent<ShopItem>();
            Vector2 cost = Vector2.zero;
            cost.x = item.GetComponent<Item>().minValue;
            cost.y = item.GetComponent<Item>().maxValue;
            itemData.Initialize((int)Random.Range(cost.x, cost.y + 1));
        }
        foreach (Spawner spawner in weaponSpawners)
        {
            GameObject item = spawner.SpawnObject(chanceManager.weaponPool.GetInstanceFromPool());
            ShopItem itemData = item.AddComponent<ShopItem>();
            Vector2 cost = Vector2.zero;
            cost.x = item.GetComponent<Item>().minValue;
            cost.y = item.GetComponent<Item>().maxValue;
            itemData.Initialize((int)Random.Range(cost.x, cost.y + 1));
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

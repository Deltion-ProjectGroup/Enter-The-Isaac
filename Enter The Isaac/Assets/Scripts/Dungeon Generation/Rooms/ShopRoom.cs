using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopRoom : BaseRoom
{
    public AudioClip buySound;
    public AudioClip noBuySound;

    public GameObject priceObject;
    public Vector3 priceOffsetFromSpawner;

    public Spawner[] itemSpawners;
    public Spawner[] weaponSpawners;
    public Spawner[] consumableSpawners;


    public List<GameObject> allItemsInRoom = new List<GameObject>();
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
            GameObject pricetag = Instantiate(priceObject, spawner.transform.position + priceOffsetFromSpawner, priceObject.transform.rotation, GameObject.FindGameObjectWithTag("Manager").GetComponent<InGameUIManager>().worldSpaceCanvas.transform);
            itemData.Initialize((int)Random.Range(cost.x, cost.y + 1), pricetag, this, buySound, noBuySound);
            pricetag.GetComponent<Text>().text = itemData.cost.ToString();
            allItemsInRoom.Add(item);
        }
        foreach (Spawner spawner in itemSpawners)
        {
            GameObject item = spawner.SpawnObject(chanceManager.itemPool.GetInstanceFromPool());
            ShopItem itemData = item.AddComponent<ShopItem>();
            Vector2 cost = Vector2.zero;
            cost.x = item.GetComponent<Item>().minValue;
            cost.y = item.GetComponent<Item>().maxValue;
            GameObject pricetag = Instantiate(priceObject, spawner.transform.position + priceOffsetFromSpawner, priceObject.transform.rotation, GameObject.FindGameObjectWithTag("Manager").GetComponent<InGameUIManager>().worldSpaceCanvas.transform);
            itemData.Initialize((int)Random.Range(cost.x, cost.y + 1), pricetag, this, buySound, noBuySound);
            pricetag.GetComponent<Text>().text = itemData.cost.ToString();
            allItemsInRoom.Add(item);
        }
        foreach (Spawner spawner in weaponSpawners)
        {
            GameObject item = spawner.SpawnObject(chanceManager.weaponPool.GetInstanceFromPool());
            ShopItem itemData = item.AddComponent<ShopItem>();
            Vector2 cost = Vector2.zero;
            cost.x = item.GetComponent<Item>().minValue;
            cost.y = item.GetComponent<Item>().maxValue;
            GameObject pricetag = Instantiate(priceObject, spawner.transform.position + priceOffsetFromSpawner, priceObject.transform.rotation, GameObject.FindGameObjectWithTag("Manager").GetComponent<InGameUIManager>().worldSpaceCanvas.transform);
            itemData.Initialize((int)Random.Range(cost.x, cost.y + 1), pricetag, this, buySound, noBuySound);
            pricetag.GetComponent<Text>().text = itemData.cost.ToString();
            allItemsInRoom.Add(item);
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
        foreach(GameObject item in allItemsInRoom)
        {
            if(item.GetComponent<ShopItem>().attachedValueHolder != null)
            {
                DestroyImmediate(item.GetComponent<ShopItem>().attachedValueHolder);
            }
        }
    }
}

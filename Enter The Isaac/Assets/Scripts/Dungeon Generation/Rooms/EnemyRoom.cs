using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoom : BaseRoom
{
    public EnemySpawnManager spawnManager;

    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null, GameObject pointConnectingTo = null)
    {
        base.Initialize(owner, parentRoom_, entrance, pointConnectingTo);
        creator.roomCount++;
        creator.enemyRooms.Add(gameObject);
    }
    public override void SpawnRoom(DungeonConnectionPoint.ConnectionDirection wantedDir, Transform doorPoint)
    {
        creator.SpawnDungeonPartAlt(creator.hallways, wantedDir, gameObject, doorPoint, RoomTypes.Hallway);
    }
    public override void OnDestroyed()
    {
        creator.enemyRooms.Remove(gameObject);
        base.OnDestroyed();
        creator.roomCount--;
    }
    public virtual void TriggerRoom()
    {
        ToggleAllDoors();
        foreach(DungeonConnectionPoint connectionPoint in allConnectionPoints)
        {
            connectionPoint.pointConnectedTo.GetComponent<DungeonConnectionPoint>().ownerRoom.gameObject.GetComponent<BoxCollider>().isTrigger = false;
        }
        spawnManager.onClearWaves += OnCompleteRoom;
        spawnManager.ActivateSpawners();
        print("TOGGLED");
    }
    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!enteredBefore && spawnManager)
            {
                TriggerRoom();
            }
        }
        base.OnTriggerEnter(other);
    }
    public virtual void OnCompleteRoom()
    {
        print("COMPL");
        foreach (DungeonConnectionPoint connectionPoint in allConnectionPoints)
        {
            connectionPoint.pointConnectedTo.GetComponent<DungeonConnectionPoint>().ownerRoom.gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }
        ToggleAllDoors();
    }
}

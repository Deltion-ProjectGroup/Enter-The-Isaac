using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoom : BaseRoom
{
    public bool activateOnEnter = true;
    public EnemySpawnManager spawnManager;

    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null)
    {
        base.Initialize(owner, parentRoom_, entrance);
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
        spawnManager.onClearWaves += OnCompleteRoom;
        spawnManager.ActivateSpawners();
        activateOnEnter = false;
        print("TOGGLED");
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if(activateOnEnter && spawnManager)
            {
                TriggerRoom();
            }
        }
    }
    public virtual void OnCompleteRoom()
    {
        print("COMPL");
        ToggleAllDoors();
    }
}

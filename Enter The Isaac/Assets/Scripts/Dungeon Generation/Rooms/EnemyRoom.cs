using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoom : BaseRoom
{
    [HideInInspector]public bool completed = false;
    [HideInInspector]public EnemySpawnManager spawnManager;

    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null)
    {
        base.Initialize(owner, parentRoom_, entrance);
        creator.roomCount++;
    }
    public override void SpawnRoom(DungeonConnectionPoint.ConnectionDirection wantedDir, Transform doorPoint)
    {
        creator.SpawnDungeonPartAlt(creator.hallways, wantedDir, gameObject, doorPoint, RoomTypes.Hallway);
    }
    public override void OnDestroyed()
    {
        base.OnDestroyed();
        creator.roomCount--;
    }
    public void OnEnteredRoom()
    {
        if (!completed)
        {
            spawnManager.onClearWaves += OnCompleteRoom;
            spawnManager.ActivateSpawners();
            completed = true;
            print("TOGGLED");
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnEnteredRoom();
        }
    }
    public virtual void OnCompleteRoom()
    {
        ToggleAllDoors();
    }
}

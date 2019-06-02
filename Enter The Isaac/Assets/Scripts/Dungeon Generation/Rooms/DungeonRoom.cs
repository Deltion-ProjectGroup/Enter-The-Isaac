using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DungeonRoom : BaseRoom
{
    bool completed = false;
    public void Update()
    {

    }
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
            foreach(GameObject door in allDoors)
            {
                //door.GetComponent<DungeonDoor>().ToggleDoor();
            }
            completed = true;
            print("TOGGLED");
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            OnEnteredRoom();
        }
    }
}

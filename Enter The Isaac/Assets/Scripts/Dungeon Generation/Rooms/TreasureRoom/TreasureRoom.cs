using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoom : BaseRoom
{
    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null, GameObject pointConnectingTo = null)
    {
        base.Initialize(owner, parentRoom_, entrance, pointConnectingTo);
        creator.treasureCount++;
        creator.roomCount++;
    }
    public override void SpawnRoom(DungeonConnectionPoint.ConnectionDirection wantedDir, Transform doorPoint)
    {
        creator.SpawnDungeonPartAlt(creator.hallways, wantedDir, gameObject, doorPoint, RoomTypes.Hallway);
    }
    public override void OnDestroyed()
    {
        base.OnDestroyed();
        creator.treasureCount--;
        creator.roomCount--;
    }
}

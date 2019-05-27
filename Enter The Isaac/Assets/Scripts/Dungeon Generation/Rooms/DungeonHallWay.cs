using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonHallWay : BaseRoom
{
    public override void SpawnRoom(DungeonConnectionPoint.ConnectionDirection wantedDir, Transform doorPoint)
    {
        creator.SpawnDungeonPartAlt(creator.rooms, wantedDir, gameObject, doorPoint, RoomTypes.Normal);
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : BaseRoom
{
    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null)
    {
        base.Initialize(owner, parentRoom_, entrance);
        creator.shopCount++;
        creator.roomCount++;
    }
    public override void SpawnRoom(DungeonConnectionPoint.ConnectionDirection wantedDir, Transform doorPoint)
    {
        throw new System.NotImplementedException();
    }
    public override void OnDestroyed()
    {
        base.OnDestroyed();
        creator.shopCount--;
        creator.roomCount--;
    }
}

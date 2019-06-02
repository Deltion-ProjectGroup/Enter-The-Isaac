using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : EnemyRoom
{
    public bool hasCutscene;
    public Sprite bossImage;
    public string bossName;

    public GameObject roomWarper;

    public override void SpawnRoom(DungeonConnectionPoint.ConnectionDirection wantedDir, Transform doorPoint)
    {
        throw new System.NotImplementedException();
    }
    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null)
    {
        base.Initialize(owner, parentRoom_, entrance);
        creator.bossCount++;
        creator.roomCount++;
    }
    public override void OnDestroyed()
    {
        base.OnDestroyed();
        creator.bossCount--;
        creator.roomCount--;
    }
    public override void OnCompleteRoom()
    {
        base.OnCompleteRoom();
        roomWarper.SetActive(true);
    }
}

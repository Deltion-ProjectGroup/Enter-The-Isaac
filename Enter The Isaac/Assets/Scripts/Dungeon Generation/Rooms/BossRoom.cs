using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : BaseRoom
{
    public GameObject roomWarper;
    public GameObject door;
    public GunMagician boss;

    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null, GameObject pointConnectingTo = null)
    {
        base.Initialize(owner, parentRoom_, entrance, pointConnectingTo);
        creator.bossCount++;
        creator.roomCount++;
        roomWarper.GetComponent<Teleporter>().levelToLoad = GameObject.FindGameObjectWithTag("Manager").GetComponent<InGameManager>().nextLevel;
        boss.onDeath += OnCompleteRoom;
    }
    public override void SpawnRoom(DungeonConnectionPoint.ConnectionDirection wantedDir, Transform doorPoint)
    {

    }
    public override void OnDestroyed()
    {
        base.OnDestroyed();
        creator.bossCount--;
        creator.roomCount--;
    }
    public void OnCompleteRoom()
    {
        boss.onDeath -= OnCompleteRoom;
        door.GetComponent<DungeonDoor>().ToggleDoor();
        roomWarper.SetActive(true);
    }
    public void TriggerRoom()
    {
        door.GetComponent<DungeonDoor>().ToggleDoor();
    }
}

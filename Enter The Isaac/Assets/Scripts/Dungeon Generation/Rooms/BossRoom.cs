using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : EnemyRoom
{
    public bool hasCutscene;
    public Sprite bossImage;
    public string bossName;
    public GameObject button;

    public GameObject roomWarper;

    public override void TriggerRoom()
    {
        if (button)
        {
            button.SetActive(false);
        }
        base.TriggerRoom();
    }
    public override void OnTriggerEnter(Collider other)
    {

    }
    public override void SpawnRoom(DungeonConnectionPoint.ConnectionDirection wantedDir, Transform doorPoint)
    {

    }
    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null)
    {
        base.Initialize(owner, parentRoom_, entrance);
        creator.bossCount++;
        creator.roomCount++;
        roomWarper.GetComponent<Teleporter>().levelToLoad = GameObject.FindGameObjectWithTag("Manager").GetComponent<InGameManager>().nextLevel;
        allDoors = new List<GameObject>();
        allDoors.Add(entrancePoint.objectToReplace.GetComponentInChildren<DungeonDoor>().gameObject);
        while (availableConnectionPoints.Count > 0)
        {
            creator.RemoveConnectionPoint(availableConnectionPoints[0]);
        }
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

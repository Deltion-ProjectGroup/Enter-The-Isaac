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
        creator.roomsPlayerCouldBeIn.Add(gameObject);


        enteredBefore = true;
        List<GameObject> occludersToUncull = new List<GameObject>();
        foreach (DungeonConnectionPoint connectionPoint in allConnectionPoints)
        {
            GameObject roomConnectedToThisPoint = connectionPoint.pointConnectedTo.GetComponent<DungeonConnectionPoint>().ownerRoom.gameObject;
            if (!roomConnectedToThisPoint.GetComponent<BaseRoom>().roomHolder.activeSelf)
            {
                occludersToUncull.Add(roomConnectedToThisPoint);
            }
        }
        if (occludersToUncull.Count > 0)
        {
            print("YEAS");
            StartCoroutine(culler.ClearOccludeRooms(occludersToUncull));
        }
    }
    public override void SpawnRoom(DungeonConnectionPoint.ConnectionDirection wantedDir, Transform doorPoint)
    {

    }
    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null, GameObject pointConnectingTo = null)
    {

        culler = GameObject.FindGameObjectWithTag("DungeonCreator").GetComponentInChildren<CustomOccluder>();
        allConnectionPoints = new List<DungeonConnectionPoint>(availableConnectionPoints);
        creator = owner;
        if (parentRoom_ != null)
        {
            parentRoom = parentRoom_;
            roomDistanceFromStart = parentRoom.GetComponent<BaseRoom>().roomDistanceFromStart;
            if (type != RoomTypes.Hallway)
            {
                roomDistanceFromStart++;
            }
        }
        if (entrance)
        {
            entrancePoint = entrance;
            availableConnectionPoints.Remove(entrance);
            if (type == RoomTypes.Normal)
            {
                print("NORMAL");
            }
            if (pointConnectingTo != null)
            {
                pointConnectingTo.GetComponent<DungeonConnectionPoint>().pointConnectedTo = entrancePoint.gameObject;
            }
        }
        if (type == RoomTypes.End)
        {
            creator.endRooms.Add(gameObject);
        }
        creator.entireDungeon.Add(gameObject);

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

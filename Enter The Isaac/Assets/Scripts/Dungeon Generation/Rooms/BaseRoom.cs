using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRoom : MonoBehaviour
{
    CustomOccluder culler;

    public GameObject occluder;
    public GameObject roomHolder;

    public int roomDistanceFromStart;
    public GameObject parentRoom;
    public int replacedTimes;


    public DungeonCreator creator;
    public List<DungeonConnectionPoint> availableConnectionPoints;
    public List<GameObject> allDoors;
    public DungeonConnectionPoint entrancePoint;
    [HideInInspector] public List<DungeonConnectionPoint> allConnectionPoints;
    public RoomTypes type;

    public virtual void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null, GameObject pointConnectingTo = null)
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
    }

    public virtual IEnumerator SpawnNextRoom()
    {
        if (availableConnectionPoints.Count > 0)
        {
            creator.openProcesses += availableConnectionPoints.Count;
        }
        else
        {
            creator.openProcesses++;
        }
        yield return null;
        int backupCount = availableConnectionPoints.Count;
        if (availableConnectionPoints.Count > 0)
        {
            for (int i = backupCount - 1; i >= 0; i--)
            {
                creator.openProcesses--;
                DungeonConnectionPoint.ConnectionDirection wantedDoorDirection = DungeonConnectionPoint.ConnectionDirection.Left;
                switch (availableConnectionPoints[i].direction)
                {
                    case DungeonConnectionPoint.ConnectionDirection.Up:
                        wantedDoorDirection = DungeonConnectionPoint.ConnectionDirection.Down;
                        break;

                    case DungeonConnectionPoint.ConnectionDirection.Down:
                        wantedDoorDirection = DungeonConnectionPoint.ConnectionDirection.Up;
                        break;

                    case DungeonConnectionPoint.ConnectionDirection.Left:
                        wantedDoorDirection = DungeonConnectionPoint.ConnectionDirection.Right;
                        break;

                    case DungeonConnectionPoint.ConnectionDirection.Right:
                        wantedDoorDirection = DungeonConnectionPoint.ConnectionDirection.Left;
                        break;
                }
                SpawnRoom(wantedDoorDirection, availableConnectionPoints[i].transform);
            }
        }
        else
        {
            creator.openProcesses--;
            if (creator.openProcesses == 0)
            {
                creator.CheckRoomCount();
            }
        }
    }
    public virtual void OnDestroyed()
    {
        creator.entireDungeon.Remove(gameObject);
        if (creator.endRooms.Contains(gameObject))
        {
            creator.endRooms.Remove(gameObject);
        }
    }
    public abstract void SpawnRoom(DungeonConnectionPoint.ConnectionDirection wantedDir, Transform doorPoint);
    public bool HasCollision(bool notIncludeThis = false)
    {
        Vector3 colliderHalfExtends = transform.GetComponent<BoxCollider>().size;
        colliderHalfExtends.x *= transform.lossyScale.x;
        colliderHalfExtends.y *= transform.lossyScale.y;
        colliderHalfExtends.z *= transform.lossyScale.z;
        colliderHalfExtends /= 2;
        bool returnValue = Physics.CheckBox(transform.position + GetComponent<BoxCollider>().center, colliderHalfExtends, transform.rotation, creator.roomLayer);
        if (returnValue)
        {
            if (notIncludeThis)
            {
                Collider[] hits = Physics.OverlapBox(transform.position + GetComponent<BoxCollider>().center, colliderHalfExtends, transform.rotation, creator.roomLayer);
                if (hits.Length >= 2)
                {
                    if(type == RoomTypes.Boss)
                    {
                        foreach (Collider hit in hits)
                        {
                            print(hit.gameObject);
                        }
                        throw new System.Exception("k");
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }
    public void ToggleAllDoors()
    {
        foreach (GameObject door in allDoors)
        {
            door.GetComponent<DungeonDoor>().ToggleDoor();
        }
    }
    public enum RoomTypes { Normal, End, Hallway, Shop, Event, Boss, Treasure, Spawn }
    private void OnDrawGizmos()
    {
        /*
        Vector3 colliderHalfExtends = transform.GetComponent<BoxCollider>().size;
        colliderHalfExtends.x *= transform.lossyScale.x;
        colliderHalfExtends.y *= transform.lossyScale.y;
        colliderHalfExtends.z *= transform.lossyScale.z;
        Gizmos.DrawMesh(cube, transform.position + GetComponent<BoxCollider>().center, transform.rotation, colliderHalfExtends);
        */
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        List<GameObject> occludersToUncull = new List<GameObject>();
        foreach(DungeonConnectionPoint connectionPoint in allConnectionPoints)
        {
            GameObject roomConnectedToThisPoint = connectionPoint.pointConnectedTo.GetComponent<DungeonConnectionPoint>().ownerRoom.gameObject;
            if (!roomConnectedToThisPoint.GetComponent<BaseRoom>().roomHolder.activeSelf)
            {
                occludersToUncull.Add(roomConnectedToThisPoint);
            }
        }
        if(occludersToUncull.Count > 0)
        {
            print("YEAS");
            StartCoroutine(culler.ClearOccludeRooms(occludersToUncull.ToArray()));
        }
        if(creator.RoomPlayerWasIn != null)
        {
            occludersToUncull = new List<GameObject>();
            BaseRoom previousRoom = creator.RoomPlayerWasIn.GetComponent<BaseRoom>();
            foreach (DungeonConnectionPoint connectionPoint in previousRoom.allConnectionPoints)
            {
                GameObject roomConnectedToThisPoint = connectionPoint.pointConnectedTo.GetComponent<DungeonConnectionPoint>().ownerRoom.gameObject;
                if (roomConnectedToThisPoint != gameObject)
                {
                    occludersToUncull.Add(roomConnectedToThisPoint);
                }
            }
            if(occludersToUncull.Count > 0)
            {
                StartCoroutine(culler.OccludeRooms(occludersToUncull.ToArray()));
            }
        }
        creator.RoomPlayerWasIn = gameObject;
    }
    public virtual void OnTriggerExit(Collider other)
    {
        List<GameObject> occluderToCull = new List<GameObject>();

    }
}

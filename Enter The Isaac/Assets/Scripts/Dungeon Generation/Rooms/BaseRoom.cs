using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRoom : MonoBehaviour
{
    public int roomDistanceFromStart;
    public GameObject parentRoom;
    public int replacedTimes;


    public DungeonCreator creator;
    public List<DungeonConnectionPoint> availableConnectionPoints;
    public List<GameObject> allDoors;
    public DungeonConnectionPoint entrancePoint;
    public RoomTypes type;

    public virtual void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, DungeonConnectionPoint entrance = null)
    {
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
    public enum RoomTypes { Normal, End, Hallway, Shop, Event, Boss, Treasure, Spawn }
}

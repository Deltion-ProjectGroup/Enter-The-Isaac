using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Chance;
using UnityEngine.AI;

public class DungeonCreator : MonoBehaviour
{
    public NavMeshSurface navMeshHolder;
    public int iterations;
    public int maxReplacementTimes;
    public GameObject startRoom;

    [Header("Generation Data")]
    public int minRoomCount;
    public int maxRoomCount;
    public int minShopRoomCount;
    public int minTreasureRoomCount;
    public int minBossRoomCount, minBossRoomDistance;
    public int maxEventRoomCount, maxEventRoomPerFloor;

    public bool removePreviousDungeon;


    public GameObject[] walls;
    [Header("Room Data")]
    public List<GameObject> startRooms;
    public List<GameObject> rooms;

    public List<GameObject> hallways;
    public List<GameObject> bossRooms;
    public List<GameObject> shopRooms;
    public List<GameObject> treasureRooms;
    public List<GameObject> eventRooms;
    public LayerMask roomLayer;

    public int openProcesses;
    public int roomCount;
    public int shopCount;
    public int bossCount;
    public int treasureCount;
    public static int eventRoomCount;
    public int eventRoomsThisFloor;

    public IntChanceHolder roomPercentages;


    [Header("Spawn Odds")]
    [Range(0, 100)]
    public int shopChance;
    [Range(0, 100)]
    public int eventRoomChance;
    [Range(0, 100)]
    public int bossRoomChance;
    [Range(0, 100)]
    public int treasureRoomChance;

    public List<GameObject> endRooms;
    public List<GameObject> entireDungeon;
    public List<GameObject> enemyRooms;

    public delegate void DelegateVoid();
    public DelegateVoid onGenerationComplete;

    public GameObject RoomPlayerWasIn;

    public void Awake()
    {
        roomPercentages.Initialize();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SceneManager.LoadScene("MainGameScene");
        }
    }
    // Generates the dungeon
    public void GenerateDungeon() 
    {
        iterations++;
        if (removePreviousDungeon)
        {
            ClearDungeon();
        }
        startRoom = Instantiate(startRooms[Random.Range(0, startRooms.Count)]);
        startRoom.GetComponent<BaseRoom>().Initialize(this);
        StartCoroutine(startRoom.GetComponent<BaseRoom>().SpawnNextRoom());
    }
    public void ClearDungeon()
    {
        eventRoomsThisFloor = 0;
        eventRoomCount = 0;
        roomCount = 0;
        openProcesses = 0;
        shopCount = 0;
        bossCount = 0;
        treasureCount = 0;
        foreach (GameObject dungeonPart in entireDungeon) /// Destroys the current dungeon if removePreviousDungeon is true;
        {
            DestroyImmediate(dungeonPart);
        }
        entireDungeon = new List<GameObject>();
        endRooms = new List<GameObject>();
        enemyRooms = new List<GameObject>();
    }
    public Vector3 GetLocationData(Transform thisObject, Transform selectedConnectionPoint, Transform connectionPointToSnapTo)
    {
        return connectionPointToSnapTo.position + (thisObject.position - selectedConnectionPoint.position);
    }
    public void SpawnDungeonPartAlt(List<GameObject> allRooms, DungeonConnectionPoint.ConnectionDirection requiredDirection, GameObject parentRoom, Transform pointToConnectTo, BaseRoom.RoomTypes roomType)
    {
        if(roomCount < maxRoomCount)
        {
            if(roomType == BaseRoom.RoomTypes.Normal)
            {
                List<GameObject> shouldReplace = ReplaceWithSpecialRoom(parentRoom);

                if (shouldReplace != null)
                {
                    allRooms = shouldReplace;
                }
            }

            List<GameObject> possibleRooms = new List<GameObject>();

            foreach (GameObject room in allRooms)
            {
                foreach (DungeonConnectionPoint connectionPoint in room.GetComponent<BaseRoom>().availableConnectionPoints)
                {
                    if (connectionPoint.direction == requiredDirection)
                    {
                        possibleRooms.Add(room);
                        break;
                    }
                }
            }

            if(possibleRooms.Count > 0)
            {
                GameObject roomToSpawn = possibleRooms[Random.Range(0, possibleRooms.Count)];
                List<DungeonConnectionPoint> availableConnectionPoints = new List<DungeonConnectionPoint>();
                for (int i = 0; i < roomToSpawn.GetComponent<BaseRoom>().availableConnectionPoints.Count; i++)
                {
                    DungeonConnectionPoint thisConnectionPoint = roomToSpawn.GetComponent<BaseRoom>().availableConnectionPoints[i];
                    if (thisConnectionPoint.direction == requiredDirection)
                    {
                        availableConnectionPoints.Add(thisConnectionPoint);
                    }
                }
                DungeonConnectionPoint selectedConnectionPoint = availableConnectionPoints[Random.Range(0, availableConnectionPoints.Count)];
                int selectedDoorId = selectedConnectionPoint.id;
                GameObject spawnedRoom = Instantiate(roomToSpawn, GetLocationData(roomToSpawn.transform, selectedConnectionPoint.transform, pointToConnectTo), roomToSpawn.transform.rotation);
                for (int i = 0; i < spawnedRoom.GetComponent<BaseRoom>().availableConnectionPoints.Count; i++)
                {
                    DungeonConnectionPoint thisConnectionPoint = spawnedRoom.GetComponent<BaseRoom>().availableConnectionPoints[i];
                    if (thisConnectionPoint.id == selectedDoorId)
                    {
                        selectedConnectionPoint = thisConnectionPoint;
                        break;
                    }
                }

                spawnedRoom.GetComponent<BaseRoom>().Initialize(this, parentRoom, selectedConnectionPoint, pointToConnectTo.gameObject);
                spawnedRoom.GetComponent<BaseRoom>().entrancePoint.pointConnectedTo = pointToConnectTo.gameObject;
                CheckPartCollision(spawnedRoom, selectedConnectionPoint.direction);
            }
            else
            {
                RemoveConnectionPoint(pointToConnectTo.GetComponent<DungeonConnectionPoint>());
                if (openProcesses == 0)
                {
                    CheckRoomCount();
                }
            }
        }
        else
        {
            RemoveConnectionPoint(pointToConnectTo.GetComponent<DungeonConnectionPoint>());
            if(openProcesses == 0)
            {
                CheckRoomCount();
            }
        }
    }
    public void CheckPartCollision(GameObject roomToCheck, DungeonConnectionPoint.ConnectionDirection entranceDirection)
    {
        if (roomToCheck.GetComponent<BaseRoom>().HasCollision(true))
        {
            ReplaceRoom(roomToCheck, entranceDirection);
        }
        else
        {
            StartCoroutine(roomToCheck.GetComponent<BaseRoom>().SpawnNextRoom());
        }
    }
    public void CheckRoomCount()
    {
        if(roomCount < minRoomCount)
        {
            ProceedGeneration();
        }
        else
        {
            if(bossCount < minBossRoomCount)
            {
                ForcedReplaceRoom(endRooms, minBossRoomCount - bossCount, bossRooms, minBossRoomDistance);
            }
            if(shopCount < minShopRoomCount)
            {
                ForcedReplaceRoom(endRooms, minShopRoomCount - shopCount, shopRooms);
            }
            if (treasureCount < minTreasureRoomCount)
            {
                ForcedReplaceRoom(endRooms, minTreasureRoomCount - treasureCount, treasureRooms);
            }
            if (bossCount < minBossRoomCount || shopCount < minShopRoomCount || treasureCount < minTreasureRoomCount)
            {
                GenerateDungeon();
            }
            else
            {
                StartCoroutine(InitializeEnemyRooms());
            }
        }
    }
    public IEnumerator InitializeEnemyRooms()
    {
        List<GameObject> enemyRoomsCopy = new List<GameObject>(enemyRooms);
        int backupCount = enemyRoomsCopy.Count;
        print(enemyRoomsCopy.Count);
        foreach(ChanceIntData data in roomPercentages.intPoolData)
        {
            float percentage = (float)data.chance / (float)roomPercentages.maxPercentage;
            int remainingRoomsToChange = (int)(backupCount * percentage);

            print(remainingRoomsToChange);
            while(enemyRoomsCopy.Count > 0 && remainingRoomsToChange > 0)
            {
                GameObject selectedRoom = enemyRoomsCopy[Random.Range(0, enemyRoomsCopy.Count)];
                print(selectedRoom);
                selectedRoom.GetComponent<EnemyRoom>().spawnManager.requiredWaveAmount = data.amount;
                enemyRoomsCopy.Remove(selectedRoom);
                remainingRoomsToChange--;
                yield return null;
            }
        }
        if(enemyRoomsCopy.Count > 0)
        {
            while(enemyRoomsCopy.Count > 0)
            {
                enemyRoomsCopy[0].GetComponent<EnemyRoom>().spawnManager.requiredWaveAmount = roomPercentages.intPoolData[roomPercentages.intPoolData.Length - 1].amount;
                enemyRoomsCopy.RemoveAt(0);
            }
        }
        foreach(GameObject enemyRoom in enemyRooms)
        {
            yield return null;
            enemyRoom.GetComponent<EnemyRoom>().spawnManager.GenerateCustomWaves();
        }
        yield return null;
        StartCoroutine(Finalize());
    }
    public IEnumerator Finalize()
    {
        foreach(GameObject dungeonPart in entireDungeon)
        {
            if(dungeonPart.GetComponent<BaseRoom>().type != BaseRoom.RoomTypes.Spawn)
            {
                dungeonPart.transform.SetParent(dungeonPart.GetComponent<BaseRoom>().entrancePoint.pointConnectedTo.transform);
            }
            yield return null;
        }
        yield return null;
        navMeshHolder.BuildNavMesh();
        yield return null;
        onGenerationComplete();
    }
    public void ProceedGeneration()
    {
        if(endRooms.Count > 0)
        {
            GameObject roomToReplace = endRooms[Random.Range(0, endRooms.Count)];
            Transform backupTransform = roomToReplace.GetComponent<BaseRoom>().entrancePoint.pointConnectedTo.transform;
            GameObject backupParent = roomToReplace.GetComponent<BaseRoom>().parentRoom;
            DungeonConnectionPoint.ConnectionDirection backupDirection = roomToReplace.GetComponent<BaseRoom>().entrancePoint.direction;
            ReplaceRoom(roomToReplace, backupDirection);
        }
        else
        {
            GenerateDungeon();
        }
    }
    public void RemoveConnectionPoint(DungeonConnectionPoint connectionPointToRemove)
    {
        if (connectionPointToRemove.gameObject.GetAbsoluteParent().GetComponent<BaseRoom>().type == BaseRoom.RoomTypes.Hallway)
        {
            GameObject hallway = connectionPointToRemove.objectToReplace.transform.gameObject.GetAbsoluteParent();
            connectionPointToRemove = hallway.GetComponent<BaseRoom>().entrancePoint.pointConnectedTo.GetComponent<DungeonConnectionPoint>();
            hallway.GetComponent<BaseRoom>().OnDestroyed();
            DestroyImmediate(hallway);
        }
        GameObject newWall = Instantiate(walls[Random.Range(0, walls.Length)], connectionPointToRemove.objectToReplace.transform.position, connectionPointToRemove.objectToReplace.transform.rotation, connectionPointToRemove.objectToReplace.transform.parent);
        connectionPointToRemove.ownerRoom.availableConnectionPoints.Remove(connectionPointToRemove);
        connectionPointToRemove.ownerRoom.allConnectionPoints.Remove(connectionPointToRemove);
        if (connectionPointToRemove.ownerRoom is EnemyRoom)
        {
            connectionPointToRemove.ownerRoom.allDoors.Remove(connectionPointToRemove.objectToReplace.GetComponentInChildren<DungeonDoor>().gameObject);
        }
        if (connectionPointToRemove.ownerRoom.availableConnectionPoints.Count <= 0 && connectionPointToRemove.ownerRoom.replacedTimes <= maxReplacementTimes)
        {
            connectionPointToRemove.ownerRoom.type = BaseRoom.RoomTypes.End;
            endRooms.Add(connectionPointToRemove.ownerRoom.gameObject);
        }
        DestroyImmediate(connectionPointToRemove.objectToReplace);
    }
    public void ReplaceRoom(GameObject roomToReplace, DungeonConnectionPoint.ConnectionDirection entranceDirection, List<GameObject> staticOptions = null)
    {
        int backupReplacedAmount = roomToReplace.GetComponent<BaseRoom>().replacedTimes;
        BaseRoom.RoomTypes backupType = roomToReplace.GetComponent<BaseRoom>().type;
        GameObject backupParentConnectionPoint = roomToReplace.GetComponent<BaseRoom>().entrancePoint.pointConnectedTo;
        GameObject backupParent = roomToReplace.GetComponent<BaseRoom>().parentRoom;
        roomToReplace.GetComponent<BaseRoom>().OnDestroyed();
        DestroyImmediate(roomToReplace);

        List<GameObject> possibleAvailableOptions = new List<GameObject>();
        if(staticOptions != null)
        {
            possibleAvailableOptions = staticOptions;
        }
        else
        {
            if (backupType == BaseRoom.RoomTypes.Hallway)
            {
                possibleAvailableOptions = hallways;
            }
            else
            {
                possibleAvailableOptions = rooms;
            }
        }

        List<GameObject> availableOptions = new List<GameObject>();
        foreach(GameObject option in possibleAvailableOptions)
        {
            foreach(DungeonConnectionPoint connectionPoint in option.GetComponent<BaseRoom>().availableConnectionPoints)
            {
                if(connectionPoint.direction == entranceDirection)
                {
                    availableOptions.Add(option);
                    break;
                }
            }
        }

        GameObject finalRoom = null;
        DungeonConnectionPoint selectedConnectionPoint = null;
        int selectedDoorId = 0;
        while(availableOptions.Count > 0)
        {
            GameObject option = availableOptions[Random.Range(0, availableOptions.Count)];
            availableOptions.Remove(option);
            List<DungeonConnectionPoint> availableConnectionPoints = new List<DungeonConnectionPoint>();
            foreach (DungeonConnectionPoint connectionPoint in option.GetComponent<BaseRoom>().availableConnectionPoints)
            {
                if (connectionPoint.direction == entranceDirection)
                {
                    availableConnectionPoints.Add(connectionPoint);
                }
            }
            selectedConnectionPoint = availableConnectionPoints[Random.Range(0, availableConnectionPoints.Count)];
            selectedDoorId = selectedConnectionPoint.id;

            finalRoom = Instantiate(option, GetLocationData(option.transform, selectedConnectionPoint.transform, backupParentConnectionPoint.transform), option.transform.rotation);

            for (int i = 0; i < finalRoom.GetComponent<BaseRoom>().availableConnectionPoints.Count; i++)
            {
                DungeonConnectionPoint thisConnectionPoint = finalRoom.GetComponent<BaseRoom>().availableConnectionPoints[i];
                if (thisConnectionPoint.id == selectedDoorId)
                {
                    selectedConnectionPoint = thisConnectionPoint;
                    break;
                }
            }

            finalRoom.GetComponent<BaseRoom>().Initialize(this, backupParent, selectedConnectionPoint, backupParentConnectionPoint);
            finalRoom.GetComponent<BaseRoom>().entrancePoint.pointConnectedTo = backupParentConnectionPoint;
            finalRoom.GetComponent<BaseRoom>().replacedTimes = backupReplacedAmount;
            if (finalRoom.GetComponent<BaseRoom>().HasCollision(true))
            {
                finalRoom.GetComponent<BaseRoom>().OnDestroyed();
                DestroyImmediate(finalRoom);
                finalRoom = null;
                continue;
            }
            else
            {
                //print("THIS ONE DID NOT COLLIDE C:");
                break;
            }
        }
        if (finalRoom != null)
        {
            finalRoom.GetComponent<BaseRoom>().replacedTimes++;

            StartCoroutine(finalRoom.GetComponent<BaseRoom>().SpawnNextRoom());
        }
        else
        {
            RemoveConnectionPoint(backupParentConnectionPoint.GetComponent<DungeonConnectionPoint>());
            if (openProcesses == 0)
            {
                CheckRoomCount();
            }
        }
    }

    public void ForcedReplaceRoom(List<GameObject> roomsToReplace, int requiredTimes, List<GameObject> staticOptions = null, int minDistanceFromStart = 0)
    {
        bool replaced = false;
        for(int counter = 0; counter < requiredTimes; counter++)
        {
            foreach (GameObject roomToReplace in roomsToReplace)
            {
                if (roomToReplace.GetComponent<BaseRoom>().roomDistanceFromStart >= minDistanceFromStart)
                {
                    roomToReplace.SetActive(false);
                    DungeonConnectionPoint.ConnectionDirection entranceDirection = roomToReplace.GetComponent<BaseRoom>().entrancePoint.direction;

                    List<GameObject> possibleAvailableOptions = new List<GameObject>();
                    if (staticOptions != null)
                    {
                        possibleAvailableOptions = staticOptions;
                    }
                    else
                    {
                        if (roomToReplace.GetComponent<BaseRoom>().type == BaseRoom.RoomTypes.Hallway)
                        {
                            possibleAvailableOptions = hallways;
                        }
                        else
                        {
                            possibleAvailableOptions = rooms;
                        }
                    }

                    List<GameObject> availableOptions = new List<GameObject>();
                    foreach (GameObject option in possibleAvailableOptions)
                    {
                        foreach (DungeonConnectionPoint connectionPoint in option.GetComponent<BaseRoom>().availableConnectionPoints)
                        {
                            if (connectionPoint.direction == entranceDirection)
                            {
                                availableOptions.Add(option);
                                break;
                            }
                        }
                    }

                    GameObject finalRoom = null;
                    DungeonConnectionPoint selectedConnectionPoint = null;
                    int selectedDoorId = 0;
                    foreach (GameObject option in availableOptions)
                    {
                        List<DungeonConnectionPoint> availableConnectionPoints = new List<DungeonConnectionPoint>();
                        foreach (DungeonConnectionPoint connectionPoint in option.GetComponent<BaseRoom>().availableConnectionPoints)
                        {
                            if (connectionPoint.direction == entranceDirection)
                            {
                                availableConnectionPoints.Add(connectionPoint);
                            }
                        }
                        selectedConnectionPoint = availableConnectionPoints[Random.Range(0, availableConnectionPoints.Count)];
                        selectedDoorId = selectedConnectionPoint.id;

                        finalRoom = Instantiate(option, GetLocationData(option.transform, selectedConnectionPoint.transform, roomToReplace.GetComponent<BaseRoom>().entrancePoint.pointConnectedTo.transform), option.transform.rotation);

                        for (int i = 0; i < finalRoom.GetComponent<BaseRoom>().availableConnectionPoints.Count; i++)
                        {
                            DungeonConnectionPoint thisConnectionPoint = finalRoom.GetComponent<BaseRoom>().availableConnectionPoints[i];
                            if (thisConnectionPoint.id == selectedDoorId)
                            {
                                selectedConnectionPoint = thisConnectionPoint;
                                break;
                            }
                        }

                        finalRoom.GetComponent<BaseRoom>().Initialize(this, roomToReplace.GetComponent<BaseRoom>().parentRoom, selectedConnectionPoint, roomToReplace.GetComponent<BaseRoom>().entrancePoint.pointConnectedTo);
                        finalRoom.GetComponent<BaseRoom>().entrancePoint.pointConnectedTo = roomToReplace.GetComponent<BaseRoom>().entrancePoint.pointConnectedTo;
                        if (finalRoom.GetComponent<BaseRoom>().HasCollision(true))
                        {
                            finalRoom.GetComponent<BaseRoom>().OnDestroyed();
                            DestroyImmediate(finalRoom);
                            finalRoom = null;
                            continue;
                        }
                        else
                        {
                            roomsToReplace.Remove(roomToReplace);
                            roomToReplace.GetComponent<BaseRoom>().OnDestroyed();
                            DestroyImmediate(roomToReplace);
                            //print("THIS ONE DID NOT COLLIDE C:");
                            break;
                        }
                    }
                    if (finalRoom != null)
                    {
                        finalRoom.GetComponent<BaseRoom>().replacedTimes++;
                        //StartCoroutine(finalRoom.GetComponent<BaseRoom>().SpawnNextRoom());
                        replaced = true;
                        break;
                    }
                    else
                    {
                        roomToReplace.SetActive(true);
                    }
                }
            }
            if (!replaced)
            {
                return;
            }
        }
    }

    public List<GameObject> ReplaceWithSpecialRoom(GameObject parentRoom_)
    {
        int randomNum = Random.Range(1, 101);
        if(bossCount < minBossRoomCount && parentRoom_.GetComponent<BaseRoom>().roomDistanceFromStart >= minBossRoomDistance && randomNum <= bossRoomChance)
        {
            return bossRooms;
        }
        randomNum = Random.Range(1, 101);
        if (randomNum <= shopChance && shopCount < minShopRoomCount)
        {
            return shopRooms;
        }
        randomNum = Random.Range(1, 101);
        if (randomNum <= treasureRoomChance && treasureCount < minTreasureRoomCount)
        {
            return treasureRooms;
        }
        randomNum = Random.Range(1, 101);
        if (eventRoomsThisFloor < maxEventRoomPerFloor && randomNum <= eventRoomChance && eventRoomCount < maxEventRoomCount)
        {
            return eventRooms;
        }
        return null;
    }
}

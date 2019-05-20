using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class DungeonRoom : BaseRoom
{
    [SerializeField]int selectedPossibility;
    [SerializeField]int currentWave;
    public PossibleWave[] waveSpawnPossibilities;
    public delegate void VoidDelegate();
    public VoidDelegate onClearRoom;
    public List<GameObject> aliveEnemies;


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentWave = 0;
            aliveEnemies = new List<GameObject>();
            OnEnteredRoom();
        }
    }
    public override void Initialize(DungeonCreator owner, GameObject parentRoom_ = null, GameObject entrance = null)
    {
        base.Initialize(owner, parentRoom_, entrance);
        creator.roomCount++;
    }
    public override void SpawnRoom(DungeonDoor.DoorDirection wantedDir, Transform doorPoint)
    {
        creator.SpawnDungeonPartAlt(creator.hallways, wantedDir, gameObject, doorPoint, RoomTypes.Hallway);
    }
    public override void OnDestroyed()
    {
        base.OnDestroyed();
        creator.roomCount--;
        if (creator.endRooms.Contains(gameObject))
        {
            creator.endRooms.Remove(gameObject);
        }
    }
    public void OnEnteredRoom()
    {
        if( waveSpawnPossibilities.Length > 0)
        {
            selectedPossibility = Random.Range(0, waveSpawnPossibilities.Length);
            onClearRoom += SpawnWave;
            SpawnWave();
        }
    }
    public void CheckEnemies()
    {
        print("CHECKED");
        for(int i= aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (!aliveEnemies[i])
            {
                aliveEnemies.RemoveAt(i);
            }
        }
        if(aliveEnemies.Count <= 0)
        {
            onClearRoom();
        }
    }
    public void SpawnWave()
    {
        if(currentWave >= waveSpawnPossibilities[selectedPossibility].waves.Length) //If done spawning waves
        {
            onClearRoom -= SpawnWave;
            print("COMPLETED ROOM");
        }
        else
        {
            foreach (SpawnData spawnable in waveSpawnPossibilities[selectedPossibility].waves[currentWave].spawnData)
            {
                GameObject enemy = spawnable.spawner.SpawnObject(spawnable.enemy);
                enemy.GetComponent<Hitbox>().onDeath += RemoveEnemy;
                aliveEnemies.Add(enemy);
            }
            currentWave++;
        }
    }
    public void RemoveEnemy(GameObject enemy)
    {
        aliveEnemies.Remove(enemy);
        CheckEnemies();
    }
    [System.Serializable]
    public struct SpawnData
    {
        public Spawner spawner;
        public GameObject enemy;
    }
    [System.Serializable]
    public struct WaveData
    {
        public SpawnData[] spawnData;
    }
    [System.Serializable]
    public struct PossibleWave
    {
        public WaveData[] waves;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chance;

public class EnemySpawnManager : MonoBehaviour
{
    int selectedPossibility;
    int currentWave;
    public int requiredWaveAmount;
    public int minSpawnersToUse, maxSpawnersToUse;
    public int minEnemiesPerSpawner, maxEnemiesPerSpawner;
    public int minEnemiesPerRoom, maxEnemiesPerRoom;
    public float minEnemySpawnDelay, maxEnemySpawnDelay;
    int totalEnemiesToSpawn = 0;
    public ItemPoolHolder enemyPool;
    public Spawner[] enemySpawners;
    public List<WaveData> waves = new List<WaveData>();
    public delegate void VoidDelegate();
    public VoidDelegate onClearWaves;
    [SerializeField] List<GameObject> aliveEnemies = new List<GameObject>();
    int openSpawnProcesses;
    [SerializeField] bool checkEnemiesInUpdate = false;


    public void GenerateCustomWaves()
    {
        enemyPool.Initialize();
        List<Spawner> availableSpawners = new List<Spawner>(enemySpawners);
        //Generates the minimal requirements to be set
        for (int currentWaveAmount = 0; currentWaveAmount < requiredWaveAmount; currentWaveAmount++)
        {
            WaveData newWave = new WaveData();
            newWave.spawnData = new List<SpawnData>();
            int spawnerAmountToUse = Random.Range(minSpawnersToUse, maxSpawnersToUse + 1);
            for(int spawnerAmount = 0; spawnerAmount < spawnerAmountToUse; spawnerAmount++)
            {
                Spawner selectedSpawner = availableSpawners[Random.Range(0, availableSpawners.Count)];
                SpawnData newSpawnerSpawnData = new SpawnData();
                newSpawnerSpawnData.enemySpawnData = new List<EnemySpawnData>();
                newSpawnerSpawnData.spawner = selectedSpawner;
                for(int spawnerEnemyCount = 0; spawnerEnemyCount < minEnemiesPerSpawner; spawnerEnemyCount++)
                {
                    EnemySpawnData enemyToSpawnData = new EnemySpawnData();
                    enemyToSpawnData.enemy = enemyPool.GetInstanceFromPool();
                    enemyToSpawnData.delayBeforeSpawn = Random.Range(minEnemySpawnDelay, maxEnemySpawnDelay + 1);
                    newSpawnerSpawnData.enemySpawnData.Add(enemyToSpawnData);
                    totalEnemiesToSpawn++;
                }
                newWave.spawnData.Add(newSpawnerSpawnData);
            }
            waves.Add(newWave);
        }
        int remainingEnemyAmount = Random.Range(0 , maxEnemiesPerRoom - totalEnemiesToSpawn);

        for(int counter = remainingEnemyAmount; counter > 0; counter--)
        {

        }
    }
    void Start(){
        InvokeRepeating("UpdateEveryHalfASecond",0,0.5f);
    }
    void UpdateEveryHalfASecond()
    {
        if (checkEnemiesInUpdate == true && IsInvoking("IgnoreSpawn") == false)
        {
            CheckEnemies();
        }
    }
    public void ActivateSpawners()
    {
        SpawnWave();
    }

    void IgnoreSpawn(){
        //A Casper function, I check if this is invoking. If not, you can spawn.
    }
    public void CheckEnemies()
    {
        if (aliveEnemies.Count <= 0)
        {
            SpawnWave();
        }
    }
    public void SpawnWave()
    {
        if (currentWave >= waves.Count) //If done spawning waves
        {
            if (onClearWaves != null)
            {
                onClearWaves();
            }
            print("COMPLETED ROOM");
        }
        else
        {
            foreach (SpawnData spawnable in waves[currentWave].spawnData)
            {
                StartCoroutine(SpawnQeue(spawnable));
            }
            currentWave++;
        }
    }
    public IEnumerator SpawnQeue(SpawnData spawnData)
    {
        openSpawnProcesses++;
        foreach (EnemySpawnData enemieSpawn in spawnData.enemySpawnData)
        {
            Invoke("IgnoreSpawn",enemieSpawn.delayBeforeSpawn);
            yield return new WaitForSeconds(enemieSpawn.delayBeforeSpawn);
            GameObject enemy = spawnData.spawner.SpawnObject(enemieSpawn.enemy);
            enemy.GetComponent<Hitbox>().onDeath += RemoveEnemy;
            aliveEnemies.Add(enemy);
        }
        openSpawnProcesses--;
    }
    public void RemoveEnemy(GameObject enemy)
    {
        print("REMOVED");
        aliveEnemies.Remove(enemy);
        if (openSpawnProcesses == 0)
        {
            CheckEnemies();
        }
    }
    [System.Serializable]
    public struct EnemySpawnData
    {
        public GameObject enemy;
        public float delayBeforeSpawn;
    }
    [System.Serializable]
    public struct SpawnData
    {
        public Spawner spawner;
        public List<EnemySpawnData> enemySpawnData;
        public int maxSpawnAmount;
    }
    [System.Serializable]
    public struct WaveData
    {
        public List<SpawnData> spawnData;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chance;

public class EnemySpawnManager : MonoBehaviour
{
    int currentWave;
    [HideInInspector]public int requiredWaveAmount;
    [SerializeField]int minSpawnersToUse, maxSpawnersToUse;
    [SerializeField]int minEnemiesPerSpawner, maxEnemiesPerSpawner;
    int maxEnemiesPerRoom;
    [SerializeField]float minEnemySpawnDelay, maxEnemySpawnDelay;
    int totalEnemiesToSpawn = 0;
    [SerializeField]ItemPoolHolder enemyPool;
    [SerializeField]Spawner[] enemySpawners;
    public List<WaveData> waves = new List<WaveData>();
    public delegate void VoidDelegate();
    public VoidDelegate onClearWaves;
    [SerializeField] List<GameObject> aliveEnemies = new List<GameObject>();
    int openSpawnProcesses;
    [SerializeField] bool checkEnemiesInUpdate = false;


    public void GenerateCustomWaves()
    {
        if(enemySpawners.Length > 0)
        {
            enemyPool.Initialize();
            maxEnemiesPerRoom = maxEnemiesPerSpawner * maxSpawnersToUse * requiredWaveAmount;
            List<SpawnerInfo> waveInfo = new List<SpawnerInfo>();
            //Generates the minimal requirements to be set
            for (int currentWaveAmount = 0; currentWaveAmount < requiredWaveAmount; currentWaveAmount++)
            {
                WaveData newWave = new WaveData();
                newWave.spawnData = new List<SpawnData>();
                int spawnerAmountToUse = Random.Range(minSpawnersToUse, maxSpawnersToUse + 1);
                List<Spawner> availableSpawnersToSelect = new List<Spawner>(enemySpawners);
                for (int spawnerAmount = 0; spawnerAmount < spawnerAmountToUse; spawnerAmount++)
                {
                    if(availableSpawnersToSelect.Count > 0)
                    {
                        Spawner selectedSpawner = availableSpawnersToSelect[Random.Range(0, availableSpawnersToSelect.Count)];
                        availableSpawnersToSelect.Remove(selectedSpawner);
                        SpawnData newSpawnerSpawnData = new SpawnData();
                        newSpawnerSpawnData.enemySpawnData = new List<EnemySpawnData>();
                        newSpawnerSpawnData.spawner = selectedSpawner;
                        for (int spawnerEnemyCount = 0; spawnerEnemyCount < minEnemiesPerSpawner; spawnerEnemyCount++)
                        {
                            EnemySpawnData enemyToSpawnData = new EnemySpawnData(enemyPool.GetInstanceFromPool(), Random.Range(minEnemySpawnDelay, maxEnemySpawnDelay + 1));
                            newSpawnerSpawnData.enemySpawnData.Add(enemyToSpawnData);
                            totalEnemiesToSpawn++;
                        }
                        newWave.spawnData.Add(newSpawnerSpawnData);
                    }
                }
                waves.Add(newWave);
            }
            if (maxEnemiesPerSpawner != minEnemiesPerSpawner && totalEnemiesToSpawn < maxEnemiesPerRoom)
            {
                for (int i = 0; i < waves.Count; i++)
                {
                    SpawnerInfo thisWaveInfo = new SpawnerInfo();
                    thisWaveInfo.waveIndex = i;
                    thisWaveInfo.availableSpawners = new List<SpawnData>(waves[i].spawnData);
                    waveInfo.Add(thisWaveInfo);
                }
            }
            if (waveInfo.Count > 0)
            {
                int remainingEnemyAmount = Random.Range(0, maxEnemiesPerRoom - totalEnemiesToSpawn);

                for (int counter = remainingEnemyAmount; counter > 0; counter--)
                {
                    SpawnerInfo selectedWave = waveInfo[Random.Range(0, waveInfo.Count)];
                    SpawnData selectedSpawner = selectedWave.availableSpawners[Random.Range(0, selectedWave.availableSpawners.Count)];
                    selectedSpawner.enemySpawnData.Add(new EnemySpawnData(enemyPool.GetInstanceFromPool(), Random.Range(minEnemySpawnDelay, maxEnemySpawnDelay)));
                    totalEnemiesToSpawn++;
                    if (selectedSpawner.enemySpawnData.Count >= maxEnemiesPerSpawner)
                    {
                        selectedWave.availableSpawners.Remove(selectedSpawner);
                        if (selectedWave.availableSpawners.Count <= 0)
                        {
                            waveInfo.Remove(selectedWave);
                            if (waveInfo.Count <= 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
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
            print("SPAWNED");
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
    public class EnemySpawnData
    {
        public GameObject enemy;
        public float delayBeforeSpawn;

        public EnemySpawnData(GameObject enemyToSpawn, float delayBeforeSpawn_)
        {
            enemy = enemyToSpawn;
            delayBeforeSpawn = delayBeforeSpawn_;
        }
    }
    [System.Serializable]
    public class SpawnData
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
    class SpawnerInfo
    {
        public int waveIndex;
        public List<SpawnData> availableSpawners = new List<SpawnData>();
    }
}

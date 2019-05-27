using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    int selectedPossibility;
    int currentWave;
    public PossibleWave[] waveSpawnPossibilities;
    public delegate void VoidDelegate();
    public VoidDelegate onClearRoom;
    List<GameObject> aliveEnemies = new List<GameObject>();
    int openSpawnProcesses;
    [SerializeField] bool checkEnemiesInUpdate = false;

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
        if (waveSpawnPossibilities.Length > 0)
        {
            selectedPossibility = Random.Range(0, waveSpawnPossibilities.Length);
            SpawnWave();
        }
    }

    void IgnoreSpawn(){
        //A Casper function, I check if this is invoking. If not, you can spawn.
    }
    public void CheckEnemies()
    {
        //print("CHECKED");
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (!aliveEnemies[i])
            {
                aliveEnemies.RemoveAt(i);
            }
        }
        if (aliveEnemies.Count <= 0)
        {
            SpawnWave();
        }
    }
    public void SpawnWave()
    {
        if (currentWave >= waveSpawnPossibilities[selectedPossibility].waves.Length) //If done spawning waves
        {
            if (onClearRoom != null)
            {
                onClearRoom();
            }
            //print("COMPLETED ROOM");
        }
        else
        {
            foreach (SpawnData spawnable in waveSpawnPossibilities[selectedPossibility].waves[currentWave].spawnData)
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
        public EnemySpawnData[] enemySpawnData;
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

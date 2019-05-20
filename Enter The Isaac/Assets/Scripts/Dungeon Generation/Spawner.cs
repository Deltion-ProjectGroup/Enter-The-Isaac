using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform spawnPoint;
    public void SpawnObject(GameObject objectToSpawn)
    {
        Instantiate(objectToSpawn, spawnPoint.position, Quaternion.identity, transform);
    }
}

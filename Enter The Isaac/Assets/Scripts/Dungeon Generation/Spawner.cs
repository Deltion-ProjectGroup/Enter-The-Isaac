using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject SpawnObject(GameObject objectToSpawn)
    {
        return Instantiate(objectToSpawn, spawnPoint.position, Quaternion.identity, transform);
    }
}

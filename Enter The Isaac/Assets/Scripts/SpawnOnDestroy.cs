using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject[] toSpawn;
    void OnDestroy() {
        for (int i = 0; i < toSpawn.Length; i++)
        {
            Instantiate(toSpawn[i],transform.position,transform.rotation);
        }
    }
}

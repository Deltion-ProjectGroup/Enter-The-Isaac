using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public GameObject localPlayer;
    public DungeonCreator dungeonCreator;
    public GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        dungeonCreator.onGenerationComplete = SpawnPlayer;
        dungeonCreator.GenerateDungeon();
    }

    public void SpawnPlayer()
    {
        localPlayer = Instantiate(playerPrefab, dungeonCreator.startRoom.GetComponent<StartRoom>().spawnPoint.position, dungeonCreator.startRoom.GetComponent<StartRoom>().spawnPoint.rotation);
    }
}

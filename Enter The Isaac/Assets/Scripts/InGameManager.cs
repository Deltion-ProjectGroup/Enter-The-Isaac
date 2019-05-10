using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    public static int floor = 0;
    public GameObject localPlayer;
    public DungeonCreator dungeonCreator;
    public GameObject playerPrefab;
    public string nextLevel;
    // Start is called before the first frame update
    void Start()
    {
        floor++;
        dungeonCreator.onGenerationComplete = SpawnPlayer;
        dungeonCreator.GenerateDungeon();
    }

    public void SpawnPlayer()
    {
        localPlayer = Instantiate(playerPrefab, dungeonCreator.startRoom.GetComponent<StartRoom>().spawnPoint.position, dungeonCreator.startRoom.GetComponent<StartRoom>().spawnPoint.rotation);
    }
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }
    public void ResetGame()
    {
        floor = 0;
    }
}

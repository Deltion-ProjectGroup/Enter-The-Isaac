using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    [Header("ItemPoolData")]
    public ItemPoolData[] itemPool;
    [HideInInspector]
    public int maxPercentage;

    public static int floor = 0;
    public GameObject localPlayer;
    public DungeonCreator dungeonCreator;
    public GameObject playerPrefab;
    public string nextLevel;
    // Start is called before the first frame update
    void Awake()
    {
        floor++;
        dungeonCreator.onGenerationComplete = SpawnPlayer;
        dungeonCreator.GenerateDungeon();
        for(int i = 0; i < itemPool.Length; i++)
        {
            ItemPoolData data = itemPool[i];
            data.percentageBorders.x = maxPercentage + 1;
            maxPercentage += data.chance;
            data.percentageBorders.y = maxPercentage;
        }
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
    [System.Serializable]
    public class ItemPoolData
    {
        public int chance;
        public Vector2 percentageBorders;
        public GameObject[] items;
    }
}

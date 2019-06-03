using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    [Header("ItemPoolData")]
    public ItemPoolHolder weaponPool;
    public ItemPoolHolder itemPool;
    public ItemPoolHolder consumablePool;
    [HideInInspector]
    public int maxPercentage;

    public static int floor = 0;
    public GameObject localPlayer;
    public DungeonCreator dungeonCreator;
    public GameObject playerPrefab;
    public int nextLevel, hubIndex;
    // Start is called before the first frame update
    void Awake()
    {
        floor++;
        dungeonCreator.onGenerationComplete = SpawnPlayer;
        dungeonCreator.GenerateDungeon();
        weaponPool.Initialize();
        itemPool.Initialize();
        consumablePool.Initialize();
    }

    public void SpawnPlayer()
    {
        localPlayer = Instantiate(playerPrefab, dungeonCreator.startRoom.GetComponent<StartRoom>().spawnPoint.position, dungeonCreator.startRoom.GetComponent<StartRoom>().spawnPoint.rotation);
        GameObject actualPlayer = GameObject.FindGameObjectWithTag("Player");
        GameObject.FindGameObjectWithTag("Database").GetComponent<SaveDatabase>().LoadPlayerData(actualPlayer.GetComponent<PlayerController>(), actualPlayer.GetComponent<Inventory>(), actualPlayer.GetComponent<Hitbox>());
    }
    public void ResetGame()
    {
        floor = 0;
    }
    public void ReturnToHub()
    {
        SceneManager.LoadScene(hubIndex);
    }
    [System.Serializable]
    public class ItemPoolData
    {
        public int chance;
        public Vector2 percentageBorders;
        public GameObject[] items;
    }
    [System.Serializable]
    public class ItemPoolHolder
    {
        public ItemPoolData[] itemPoolData;
        public int maxPercentage;

        public void Initialize()
        {
            for (int i = 0; i < itemPoolData.Length; i++)
            {
                ItemPoolData data = itemPoolData[i];
                data.percentageBorders.x = maxPercentage + 1;
                maxPercentage += data.chance;
                data.percentageBorders.y = maxPercentage;
            }
        }
        public GameObject GetItemFromPool()
        {
            int randomNum = Random.Range(1, maxPercentage + 1);
            foreach (ItemPoolData data in itemPoolData)
            {
                if (randomNum >= data.percentageBorders.x && randomNum <= data.percentageBorders.y)
                {
                    if (data.items.Length > 0)
                    {
                        return (data.items[Random.Range(0, data.items.Length)]);
                    }
                    break;
                }
            }
            return null;
        }
    }
}

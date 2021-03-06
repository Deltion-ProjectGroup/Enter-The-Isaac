﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{

    public static int floor = 1;
    public GameObject localPlayer;
    public DungeonCreator dungeonCreator;
    public GameObject playerPrefab;
    public int nextLevel, hubIndex;
    // Start is called before the first frame update
    void Start()
    {
        dungeonCreator.onGenerationComplete = SpawnPlayer;
        StartCoroutine(dungeonCreator.GenerateDungeon());
    }

    public void SpawnPlayer()
    {
        print("SPAWNED");
        localPlayer = Instantiate(playerPrefab, dungeonCreator.startRoom.GetComponent<StartRoom>().spawnPoint.position, dungeonCreator.startRoom.GetComponent<StartRoom>().spawnPoint.rotation);
        GameObject actualPlayer = GameObject.FindGameObjectWithTag("Player");
        GameObject.FindGameObjectWithTag("Database").GetComponent<SaveDatabase>().LoadPlayerData(actualPlayer.GetComponent<PlayerController>(), actualPlayer.GetComponent<Inventory>(), actualPlayer.GetComponentInChildren<Hitbox>());
        print("LOADED");
    }
    public void ResetGame()
    {
        floor = 0;
    }
    public void ReturnToHub()
    {
        SceneManager.LoadScene(hubIndex);
    }
}

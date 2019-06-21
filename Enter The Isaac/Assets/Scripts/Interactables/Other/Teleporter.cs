using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    public int levelToLoad;
    public void OnTriggerEnter(Collider hit)
    {
        if(hit.transform.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("Database").GetComponent<SaveDatabase>().SavePlayerData(hit.GetComponent<PlayerController>(), hit.GetComponent<Inventory>(), hit.GetComponentInChildren<Hitbox>());
            GameObject.FindGameObjectWithTag("SceneManager").GetComponent<InGameSceneManager>().LoadSceneAsyncStart(levelToLoad);
        }
    }
}

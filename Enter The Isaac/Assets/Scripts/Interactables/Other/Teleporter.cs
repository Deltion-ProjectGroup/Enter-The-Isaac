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
            GameObject.FindGameObjectWithTag("Database").GetComponent<SaveDatabase>().SavePlayerData(hit.gameObject.GetComponent<PlayerController>(), hit.gameObject.GetComponent<Inventory>(), hit.gameObject.GetComponentInChildren<Hitbox>());
            SceneManager.LoadScene(levelToLoad);
        }
    }
}

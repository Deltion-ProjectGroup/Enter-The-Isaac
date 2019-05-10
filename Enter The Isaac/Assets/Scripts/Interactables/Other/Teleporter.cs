using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public void OnTriggerEnter(Collider hit)
    {
        if(hit.transform.tag == "Player")
        {
            InGameManager inGameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<InGameManager>();
            inGameManager.LoadNextLevel();
        }
    }
}

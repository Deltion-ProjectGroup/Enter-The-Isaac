using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnButtonUp : MonoBehaviour
{
    public string destroyInput = "Fire1";

    void Update()
    {
        if(Input.GetAxis(destroyInput) <= 0 && Time.timeScale != 0){
            Destroy(gameObject);
        }
    }
}

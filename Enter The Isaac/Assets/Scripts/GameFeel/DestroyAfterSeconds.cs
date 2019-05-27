using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float time = 1;
    void Start()
    {
        Invoke("DestroyMe",time);
    }

    public void DestroyMe(){
        Destroy(gameObject);
    }
}

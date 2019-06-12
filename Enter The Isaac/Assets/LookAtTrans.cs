using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTrans : MonoBehaviour
{
    [SerializeField] Transform lookAtTrans;
    [SerializeField] bool lookAtPlayer = false;
    [SerializeField] float rotSpeed = 5;
    [SerializeField] Vector3 offsetV3 = Vector3.zero;
    void Start()
    {
        if(lookAtTrans == null && lookAtPlayer == true){
            lookAtTrans = FindObjectOfType<PlayerController>().transform;
        }
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookAtTrans.position - transform.position,Vector3.up) * Quaternion.Euler(offsetV3),Time.deltaTime * rotSpeed);
    }
}

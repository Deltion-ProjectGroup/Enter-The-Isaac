using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreRotation : MonoBehaviour
{

    Vector3 startRot;
    [SerializeField] Transform relativeTo;

    void Start()
    {
        startRot = transform.eulerAngles;
    }

    void LateUpdate()
    {
        transform.eulerAngles = startRot;
        if(relativeTo != null){
             transform.eulerAngles = startRot + relativeTo.eulerAngles;
        }
    }
}

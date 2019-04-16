using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
   public float speed = 10;

    void Update()
    {
        transform.position -= transform.TransformDirection(0,0,Time.deltaTime * speed);
    }
}

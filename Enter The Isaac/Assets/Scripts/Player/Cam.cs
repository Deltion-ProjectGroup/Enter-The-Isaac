using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{

    [SerializeField] Transform[] target;
    [SerializeField] Vector3 offset;
    [SerializeField] float speed = 10;
    void Start()
    {
      //  transform.position = target.position + offset;
    }

    void Update()
    {
        Vector3 centerPos = Vector3.zero;
        for (int i = 0; i < target.Length; i++)
        {
            centerPos += target[i].position;
        }
        centerPos /= target.Length;
        transform.position = Vector3.Lerp(transform.position, centerPos + offset, Time.deltaTime * speed);
    }
}

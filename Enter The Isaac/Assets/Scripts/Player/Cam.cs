using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] float speed = 10;
    void Start()
    {
        transform.position = target.position + offset;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * speed);
    }
}

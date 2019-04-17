using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float speed = 10;
    Vector3 lastPos;
    [SerializeField] bool destroyOnRayHit = true;

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        Move();
        RayCollide();
    }

    public virtual void Move()
    {
        transform.position -= transform.TransformDirection(0, 0, Time.deltaTime * speed);
    }

    public void RayCollide()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.position - lastPos, out hit, Vector3.Distance(transform.position, lastPos)))
        {
            transform.position = hit.point;
            if(destroyOnRayHit == true){
                Destroy(gameObject,Time.deltaTime * 5);
                speed = 0;
            }
        }
        lastPos = transform.position;
    }
}

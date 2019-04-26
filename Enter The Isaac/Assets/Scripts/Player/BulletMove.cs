using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float speed = 10;
    Vector3 lastPos;
    public bool destroyOnRayHit = true;

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
        Debug.DrawRay(lastPos, lastPos - transform.position, Color.red, Time.deltaTime);
        if (Physics.SphereCast(lastPos, 0.1f, lastPos - transform.position, out hit, Vector3.Distance(transform.position, lastPos)))
        {
            if (hit.transform.tag != "Player")
            {
                if (destroyOnRayHit == true)
                {
                    transform.position = hit.point;
                    Destroy(gameObject, Time.deltaTime * 3);
                    speed = 0;
                }
            }
        }
        lastPos = transform.position;
    }
}

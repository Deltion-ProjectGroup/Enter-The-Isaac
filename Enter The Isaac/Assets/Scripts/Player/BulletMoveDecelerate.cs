using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMoveDecelerate : BulletMove
{

    [HideInInspector] public float decelerationSpeed = 10;

    void Update()
    {
        Move();
        RayCollide();
    }

    public override void Move()
    {
        speed = Mathf.Lerp(speed,0,Time.deltaTime * decelerationSpeed);
        transform.position -= transform.TransformDirection(0, 0, Time.deltaTime * speed);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float speed = 10;
    Vector3 lastPos;
    public bool destroyOnRayHit = true;
    [HideInInspector] public bool ricoshet = false;
    [SerializeField] bool ignoreCollisionDestroy = true;
    void Start()
    {
        lastPos = transform.position;
    }

    void FixedUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        Move();
        RayCollide();
    }

    public virtual void Move()
    {
        lastPos = transform.position;
        transform.position -= transform.TransformDirection(0, 0, Time.deltaTime * speed);
    }

    public void RayCollide()
    {
        RaycastHit hit;
        if (Physics.SphereCast(lastPos, 0.1f, -(lastPos - transform.position), out hit, Vector3.Distance(transform.position, lastPos),~LayerMask.GetMask("Ignore Raycast","Bullet")))
        {
            if (hit.transform.tag != "Player")
            {
                if (destroyOnRayHit == true)
                {
                    transform.position = hit.point;
                    Destroy(gameObject, Time.deltaTime * 3);
                    speed = 0;
                }
                else if (hit.transform.GetComponent<Hitbox>() == null && ricoshet == true)
                {
                    transform.eulerAngles = Quaternion.FromToRotation(Vector3.forward, Vector3.Reflect(-transform.forward, new Vector3(hit.normal.z, hit.normal.y, hit.normal.x))).eulerAngles;
                    transform.position = hit.point;
                }
            }
        }

    }

    

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" && destroyOnRayHit == true && ignoreCollisionDestroy == false)
        {
            Destroy(gameObject, Time.deltaTime * 3);
            speed = 0;
        }
    }
}

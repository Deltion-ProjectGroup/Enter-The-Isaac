using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] float losDistance = 1;
    [SerializeField] float lodRadius = 30;
    Gun gun;
    [SerializeField] float startupTime = 0.3f;
    [SerializeField] float lifeTime = 10;
    void Start()
    {
        Invoke("StartLater", startupTime);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + transform.GetComponent<Renderer>().bounds.extents.y, hit.point.z);
        }
        Destroy(gameObject,lifeTime);
    }

    void StartLater()
    {
        gun = transform.GetChild(0).GetComponent<Gun>();
        //gun.gunDel = FindObjectOfType<Gun>().gunDel;
        print("still havent done gun delegate to the player.");
        InvokeRepeating("LOS", 0, gun.gunClone.fireRate);
        gun.gunClone.screenShakeTime = 0;
        gun.gunClone.screenshakeStrength = 0;
        gun.gunClone.camFov = 70;
        gun.gunClone.recoil = 0;
    }

    void Update()
    {
        if (gun != null)
        {
            gun.GetShootInput();
            if (gun.shootFirstFrame == true)
            {
                gun.shootFirstFrame = false;
            }
        }
    }

    void LOS()
    {
        RaycastHit[] hit;
        hit = Physics.SphereCastAll(transform.position, lodRadius, transform.forward, losDistance, LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Collide);
        if (hit.Length > 0)
        {
            Transform closestEnemy = hit[0].transform;
            for (int i = 1; i < hit.Length; i++)
            {
                if (Vector3.Distance(transform.position, hit[i].transform.position) < Vector3.Distance(transform.position, closestEnemy.position))
                {
                    closestEnemy = hit[i].transform;
                }
            }
            if (gun.curAmmo > 0)
            {
                Shoot(closestEnemy);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            gun.shootInput = 0;
            gun.shootFirstFrame = false;
            gun.CancelInvoke("Shoot");
        }
    }

    void Shoot(Transform enemy)
    {
        gun.transform.LookAt(enemy.position);
        gun.transform.Rotate(0, 180, 0);
        gun.angle = gun.transform.localEulerAngles;
        gun.shootInput = 1;
        gun.shootFirstFrame = true;
    }
}

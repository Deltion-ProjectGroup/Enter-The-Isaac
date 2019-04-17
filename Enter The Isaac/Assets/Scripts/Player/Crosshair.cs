using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{

    Camera mainCam;
    [SerializeField] Transform player;
    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        PlanarHit();
       // RayHit();
    }

    void RayHit()
    {
        Ray ray = mainCam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Default")))
        {
             transform.position = new Vector3(hit.point.x,hit.point.y,hit.point.z);
        }
    }

    void PlanarHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane hPlane = new Plane(Vector3.up, new Vector3(0,player.position.y,0));
        float distance = 0;
        if (hPlane.Raycast(ray, out distance))
        {
            transform.position = ray.GetPoint(distance);
        }
    }
}

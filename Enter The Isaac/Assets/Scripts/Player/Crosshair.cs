﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{

    Camera mainCam;
    [SerializeField] Transform player;
    public bool mouseControlled = true;
    [SerializeField] string inputHor = "Horizontal";
    [SerializeField] string inputVert = "Vertical";
    void Start()
    {
        mainCam = Camera.main;
        mouseControlled = true;
    }

    void Update()
    {
        CheckInputType();
        if (mouseControlled == true)
        {
            PlanarHit();
            // RayHit();
        }
        else
        {
            ButtonAim();
        }
    }

    void CheckInputType()
    {
        if (mouseControlled == true)
        {
            if (Vector2.SqrMagnitude(new Vector2(Input.GetAxis(inputHor), Input.GetAxis(inputVert))) != 0)
            {
                mouseControlled = false;
            }
        }
        else if (Vector2.SqrMagnitude(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))) != 0)
        {
            mouseControlled = true;
        }
    }

    void ButtonAim()
    {
        Vector3 inputToCam = new Vector3(Input.GetAxis(inputHor),0,Input.GetAxis(inputVert));
        inputToCam.y += mainCam.transform.eulerAngles.y;
        if (Vector2.SqrMagnitude(new Vector2(Input.GetAxis(inputHor), Input.GetAxis(inputVert))) > 0)
        {
            transform.position = player.position + new Vector3(inputToCam.x * 30, transform.position.y, inputToCam.z * 30);
        }
        else
        {
            transform.position = player.position + (-player.forward * 30);
        }
        transform.position = new Vector3(transform.position.x, player.position.y, transform.position.z);
    }

    void RayHit()
    {
        Ray ray = mainCam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Default")))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }
    }

    void PlanarHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane hPlane = new Plane(Vector3.up, new Vector3(0, player.position.y, 0));
        float distance = 0;
        if (hPlane.Raycast(ray, out distance))
        {
            transform.position = ray.GetPoint(distance);
        }
    }
}

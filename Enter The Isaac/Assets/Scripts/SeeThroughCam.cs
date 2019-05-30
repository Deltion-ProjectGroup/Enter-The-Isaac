using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThroughCam : MonoBehaviour
{

    [SerializeField] List<Transform> objects;
    Camera cam;
    Camera mainCam;

    void Start()
    {
        cam = GetComponent<Camera>();
        mainCam = Camera.main;
    }

    void FixedUpdate()
    {
        cam.enabled = false;
        cam.fieldOfView = mainCam.fieldOfView;
        for (int i = 0; i < objects.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, objects[i].position - transform.position, out hit, Vector3.Distance(transform.position, objects[i].position), ~LayerMask.GetMask("SpecialRender")) == true)
            {
                if (hit.transform.root.name != "Player")
                {
                    cam.enabled = true;
                }
            }
        }
    }
}

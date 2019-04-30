using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThroughCam : MonoBehaviour
{

    [SerializeField] List<Transform> objects;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        cam.enabled = false;
        for (int i = 0; i < objects.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, objects[i].position - transform.position, out hit, Vector3.Distance(transform.position, objects[i].position), ~LayerMask.GetMask("SpecialRender")) == true)
            {
                cam.enabled = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{

    [SerializeField] Transform[] target;
    [SerializeField] Vector3 offset;
    [SerializeField] float speed = 10;
    Camera cam;
    float startFOV = 0;
    [SerializeField] float fovResetSpeed = 3;
    [SerializeField] float followMouseAmount = 4;
    [SerializeField] float followMouseIgnore = 1;

    void Start()
    {
        cam = GetComponent<Camera>();
        startFOV = cam.fieldOfView;
        ToCenterPos(999999);
    }

    void Update()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, startFOV, Time.deltaTime * fovResetSpeed);
        ToCenterPos(speed);
    }

    void ToCenterPos(float lerpSpeed)
    {
        Vector3 centerPos = Vector3.zero;
        for (int i = 0; i < target.Length; i++)
        {
            centerPos += target[i].position;
        }
        centerPos /= target.Length;
        Vector3 mousePos = new Vector3((Input.mousePosition.x - Screen.width) / Screen.width, 0, (Input.mousePosition.y - Screen.height) / Screen.height);
        mousePos += new Vector3(0.5f, 0, 0.5f);
        if (Vector3.Magnitude(mousePos) > followMouseIgnore)
        {
            centerPos += mousePos * followMouseAmount;
        }
        transform.position = Vector3.Lerp(transform.position, centerPos + offset, Time.deltaTime * lerpSpeed);
    }
}

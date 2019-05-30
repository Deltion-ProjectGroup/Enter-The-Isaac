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
    Vector3 startRot;
    [SerializeField] float rotAmount = 30;
    [SerializeField] Crosshair crosshair;//this is used for controller mousefollow.
    PlayerController player;
    [SerializeField] bool ignoreFOVChange = false;


    void Start()
    {
        startRot = transform.eulerAngles;
        cam = GetComponent<Camera>();
        startFOV = cam.fieldOfView;
        ToCenterPos(999999);
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (ignoreFOVChange == false)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, startFOV, Time.deltaTime * fovResetSpeed);
        }
        else
        {
            cam.fieldOfView = startFOV;
        }
        if (player.curState != PlayerController.State.Roll && player.curState != PlayerController.State.GetHit)
        {
            ToCenterPos(speed);
        }
        else
        {
            ToCenterPos(speed / 2);
        }
    }

    void LateUpdate()
    {
        ignoreFOVChange = (PlayerPrefs.GetInt("screenshake") == 0);
        if (ignoreFOVChange == true)
        {
            cam.fieldOfView = startFOV;
        }
    }

    void ToCenterPos(float lerpSpeed)
    {
        Vector3 centerPos = Vector3.zero;
        for (int i = 0; i < target.Length; i++)
        {
            centerPos += target[i].position;
        }
        centerPos /= target.Length;
        if (crosshair.mouseControlled == true)
        {
            centerPos = MouseFollow(centerPos);
        }
        transform.position = Vector3.Lerp(transform.position, centerPos + offset, Time.deltaTime * lerpSpeed);
    }

    Vector3 MouseFollow(Vector3 centerPos)
    {
        Vector3 mousePos = new Vector3((Input.mousePosition.x - Screen.width) / Screen.width, 0, (Input.mousePosition.y - Screen.height) / Screen.height);
        mousePos += new Vector3(0.5f, 0, 0.5f);
        mousePos = new Vector3(Mathf.Max(mousePos.x, -0.5f), Mathf.Max(mousePos.y, -0.5f), Mathf.Max(mousePos.z, -0.5f));
        mousePos = new Vector3(Mathf.Min(mousePos.x, 0.5f), Mathf.Min(mousePos.y, 0.5f), Mathf.Min(mousePos.z, 0.5f));
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(startRot + (new Vector3(-mousePos.z * rotAmount, mousePos.x * rotAmount, mousePos.x * rotAmount) * PlayerPrefs.GetFloat("mouseFollowAmount"))), Time.deltaTime * 5);
        mousePos.z *= 1.5f;
        if (Vector3.Magnitude(mousePos) > followMouseIgnore)
        {
            centerPos += mousePos * followMouseAmount * PlayerPrefs.GetFloat("mouseFollowAmount");
        }
        return centerPos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{

    CharacterController cc;
    [HideInInspector] public Vector3 moveV3;
    Transform cam;
    [Header("Walking related")]
    [SerializeField] float walkSpeed = 1;
    float curWalkSpeed = 0;
    [SerializeField] float rotateSpeed = 1;
    public float rotGoal = 0;
    [SerializeField] float accelerationSpeed = 3;
    [SerializeField] float decelerationSpeed = 5;
    [Header("Input")]
    [SerializeField] string horInput = "Horizontal";
    [SerializeField] string vertInput = "Vertical";
    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        RotateXZ();
        MoveForward();
        Gravity();
        FinalMove();
    }

    void RotateXZ()
    {
        if (Vector2.SqrMagnitude(new Vector2(Input.GetAxis(horInput), Input.GetAxis(vertInput))) != 0)
        {
            rotGoal = Mathf.Atan2(Input.GetAxis(vertInput), Input.GetAxis(horInput)) * Mathf.Rad2Deg + 90 + cam.eulerAngles.y;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, rotGoal, 0), Time.deltaTime * rotateSpeed);
    }

    void MoveForward()
    {
        Vector3 helpVector = Vector3.zero;
        float lastZRot = transform.eulerAngles.z;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        helpVector = transform.TransformDirection(0, 0, -curWalkSpeed * walkSpeed);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, lastZRot);
        moveV3.x = helpVector.x;
        moveV3.z = helpVector.z;
        float magnitude = Mathf.Min(1, Vector2.SqrMagnitude(new Vector2(Input.GetAxis(horInput), Input.GetAxis(vertInput))));
        if (magnitude != 0)
        {
            curWalkSpeed = Mathf.MoveTowards(curWalkSpeed, magnitude, Time.deltaTime * accelerationSpeed);
        }
        else
        {
            curWalkSpeed = Mathf.MoveTowards(curWalkSpeed, magnitude, Time.deltaTime * decelerationSpeed);
        }
    }

    void Gravity(){
        moveV3.y = -9.81f;
    }

    void FinalMove()
    {
        cc.Move(moveV3 * Time.deltaTime);
    }
}

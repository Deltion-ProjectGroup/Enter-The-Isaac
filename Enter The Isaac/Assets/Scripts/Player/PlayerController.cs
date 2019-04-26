using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{

    CharacterController cc;
    [HideInInspector] public Vector3 moveV3;
    Transform cam;
    [Header("Random stuff")]
    [SerializeField] GameObject hitbox;
    [Header("Walking related")]
    [SerializeField] float walkSpeed = 1;
    float curWalkSpeed = 0;
    [SerializeField] float rotateSpeed = 1;
    [SerializeField] Transform crosshair;
    [SerializeField] float rotateCrosshairSpeed;
    public float rotGoal = 0;
    [SerializeField] float accelerationSpeed = 3;
    [SerializeField] float decelerationSpeed = 5;
    [Header("Gun")]
    [SerializeField] Gun gun;
    [Header("Input")]
    [SerializeField] string horInput = "Horizontal";
    [SerializeField] string vertInput = "Vertical";
    [SerializeField] string rollInput = "Fire3";
    public enum State
    {
        Normal,
        Roll
    }
    [Header("States")]
    [SerializeField] State curState = State.Normal;
    [Header("Rolling")]
    [SerializeField] float rollSpeed = 40;
    [SerializeField] float rollDeceleration = 100;
    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        switch (curState)
        {
            case State.Normal:
                if (gun != null)
                {
                    RotateCrosshair();
                    MoveXZ();
                    gun.CheckInput();
                }
                else
                {
                    RotateXZ();
                    MoveForward();
                }
                Gravity();
                FinalMove();
                CheckRollInput();
                break;
            case State.Roll:
                RotateXZ();
                moveV3 = Vector3.MoveTowards(moveV3, Vector3.zero, Time.deltaTime * rollDeceleration);
                FinalMove();
                break;
        }
    }

    void CheckRollInput()
    {
        if (Input.GetButtonDown(rollInput) == true)
        {
            StartCoroutine(RollEvent());
        }
    }

    IEnumerator RollEvent()
    {
        moveV3 = Vector3.zero;
        curState = State.Roll;
        float oldRotSpeed = rotateSpeed;
        rotateSpeed = rotateCrosshairSpeed * 8f;
        hitbox.SetActive(false);
        gun.CancelInvoke("Shoot");
        yield return new WaitForSeconds(0.025f);
        rotateSpeed = 0;
        moveV3 = transform.forward * -rollSpeed;
        yield return new WaitForSeconds(0.3f);
        rotateSpeed = oldRotSpeed;
        curState = State.Normal;
        hitbox.SetActive(true);
    }

    void RotateCrosshair()
    {
        crosshair.position = new Vector3(crosshair.position.x, transform.position.y, crosshair.position.z);
        rotGoal = Mathf.Atan2(crosshair.position.x - transform.position.x, crosshair.position.z - transform.position.z) * 180 / Mathf.PI + 180;

        float angleHelper = (Vector3.Angle(crosshair.position - transform.position,transform.right) - 90) / 270;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, rotGoal, 0), Time.deltaTime * rotateCrosshairSpeed);
        transform.GetChild(0).Rotate(0,0,angleHelper * -30);
        transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation,Quaternion.Euler(0,180,0),Time.deltaTime * rotateCrosshairSpeed);


    }

    void MoveXZ()
    {
        Vector2 inputs = new Vector2(Input.GetAxis(horInput), Input.GetAxis(vertInput));
        if (Vector2.SqrMagnitude(inputs) > 1.1f)
        {
            inputs -= new Vector2(0.5f, 0.5f);
        }
        if (Input.GetAxis(horInput) != 0)
        {
            moveV3.x = Mathf.Lerp(moveV3.x, Input.GetAxis(horInput) * walkSpeed, Time.deltaTime * accelerationSpeed);
        }
        else
        {
            moveV3.x = Mathf.Lerp(moveV3.x, Input.GetAxis(horInput) * walkSpeed, Time.deltaTime * decelerationSpeed);
        }

        if (Input.GetAxis(vertInput) != 0)
        {
            moveV3.z = Mathf.Lerp(moveV3.z, Input.GetAxis(vertInput) * walkSpeed, Time.deltaTime * accelerationSpeed);
        }
        else
        {
            moveV3.z = Mathf.Lerp(moveV3.z, Input.GetAxis(vertInput) * walkSpeed, Time.deltaTime * decelerationSpeed);
        }
    }

    void RotateXZ()
    {
        if (Vector2.SqrMagnitude(new Vector2(Input.GetAxis(horInput), Input.GetAxis(vertInput))) != 0)
        {
            rotGoal = Mathf.Atan2(Input.GetAxis(vertInput), -Input.GetAxis(horInput)) * Mathf.Rad2Deg + 90 + cam.eulerAngles.y;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, rotGoal, 0), Time.deltaTime * rotateSpeed);

        if (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, rotGoal, 0)) > 140)
        {
            curWalkSpeed = 0;
        }
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

    void Gravity()
    {
        moveV3.y = -9.81f;
    }

    void FinalMove()
    {
        cc.Move(moveV3 * Time.deltaTime);
    }
}

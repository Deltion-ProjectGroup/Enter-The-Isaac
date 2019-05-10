using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{

    CharacterController cc;
    [HideInInspector] public Vector3 moveV3;
    Transform cam;
    Animator anim;
    [Header("Random stuff")]
    public GameObject hitbox;
    public int keys = 0;
    public Text keyUICounter;
    [Header("Walking related")]
    public float walkSpeed = 1;
    float curWalkSpeed = 0;
    [SerializeField] float rotateSpeed = 1;
    public Transform crosshair;
    [SerializeField] float rotateCrosshairSpeed;
    public float rotGoal = 0;
    [SerializeField] float accelerationSpeed = 3;
    [SerializeField] float decelerationSpeed = 5;
    [Header("Gun")]
    public Gun gun;
    public GunType[] guns;
    [HideInInspector] public int curGun = 0;
    [HideInInspector] public List<int> ammoStore = new List<int>();
    [HideInInspector] public List<int> magazineStore = new List<int>();
    public Gun.gunDelegate gunDel;
    [Header("Input")]
    [SerializeField] string horInput = "Horizontal";
    [SerializeField] string vertInput = "Vertical";
    [SerializeField] string rollInput = "Fire3";
    public string shootInput = "Fire1";
    [SerializeField] string reloadInput = "Fire2";
    [SerializeField] string scrollInput = "Mouse ScrollWheel";
    public delegate void playerDelegate();
    public playerDelegate onShootDel;//maybe do something when shooting
    public playerDelegate onNoGunShootDel;//used for when hats do stuff not related to shooting, like the constructor hat, or a melee hat
    public enum State
    {
        Normal,
        Roll,
        GetHit
    }
    [Header("States")]
    [SerializeField] State curState = State.Normal;
    [Header("Rolling")]
    public float rollSpeed = 40;
    public float rollDeceleration = 100;
    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main.transform;


        for (int i = 0; i < guns.Length; i++)
        {
            ammoStore.Add(guns[i].magazineSize);
        }
        for (int i = 0; i < guns.Length; i++)
        {
            magazineStore.Add(guns[i].maxAmmo);
        }


        if (transform.GetChild(0).GetComponent<Animator>() != null)
        {
            anim = transform.GetChild(0).GetComponent<Animator>();
        }
        else
        {
            Debug.LogWarning("Player model is missing. The model must be child 0! -Casper");
        }
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
                    GunInput();
                }
                else
                {
                    SwitchGun();
                    RotateXZ();
                    MoveForward();
                    if (onNoGunShootDel != null)
                    {
                        onNoGunShootDel();
                    }
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
        SetAnimValues();
        if (keyUICounter != null)
        {
            if (keys >= 10)
            {
                keyUICounter.text = "X " + keys;
            }
            else
            {
                keyUICounter.text = "X 0" + keys;//lazyness intensfies
            }
        }
    }

    void GunInput()
    {
        //shooting
        gun.shootFirstFrame = Input.GetButtonDown(shootInput);
        gun.shootInput = Input.GetAxis(shootInput);
        if (Input.GetAxis(shootInput) != 0)//was originally getbuttondown
        {
            gun.GetShootInput();
            ammoStore[curGun] = gun.curAmmo;
        }
        //reloading
        if (Input.GetButtonDown(reloadInput))
        {
            gun.GetReloadInput();
        }
        //switch gun
        SwitchGun();
    }

    void SwitchGun()
    {
        if (Input.GetAxis(scrollInput) != 0 && GameObject.FindGameObjectWithTag("LaserHold") == null)
        {
            // gun.GetSwitchGunInput(Input.GetAxis(scrollInput));
            if (Input.GetAxis(scrollInput) > 0)
            {
                curGun = (int)Mathf.Repeat(curGun + 1, guns.Length);
            }
            else
            {
                curGun = (int)Mathf.Repeat(curGun - 1, guns.Length);
            }
        }
    }

    void SetAnimValues()
    {
        anim.SetFloat("posChangedDistance", Vector2.SqrMagnitude(new Vector2(Input.GetAxis(horInput), Input.GetAxis(vertInput))));

        Vector3 inputDir = new Vector3(Input.GetAxis(horInput), 0, Input.GetAxis(vertInput));
        inputDir = transform.InverseTransformDirection(inputDir);

        anim.SetFloat("horInput", -inputDir.x);
        anim.SetFloat("vertInput", -inputDir.z);
    }

    void CheckRollInput()
    {
        if (Input.GetButtonDown(rollInput) == true)
        {
            if (GameObject.FindGameObjectWithTag("LaserHold") != null && Input.GetAxis(shootInput) != 0)
            {
                Destroy(GameObject.FindGameObjectWithTag("LaserHold"));
            }
            StartCoroutine(RollEvent());
        }
    }

    IEnumerator RollEvent()
    {
        transform.GetChild(0).localRotation = Quaternion.Euler(0, 180, 0);
        moveV3 = Vector3.zero;
        curState = State.Roll;
        float oldRotSpeed = rotateSpeed;
        rotateSpeed = rotateCrosshairSpeed * 8f;
        hitbox.SetActive(false);
        if (gun != null)
        {
            gun.CancelInvoke("Shoot");
        }
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

        float angleHelper = (Vector3.Angle(crosshair.position - transform.position, transform.right) - 90) / 270;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, rotGoal, 0), Time.deltaTime * rotateCrosshairSpeed);
        transform.GetChild(0).localEulerAngles = new Vector3(0, 180, angleHelper * -70 * Time.timeScale);
        transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * rotateCrosshairSpeed);


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

    public void GetHit(float timeStopTime)
    {
        Invoke("StopHitControl", timeStopTime);
        StopAllCoroutines();
        moveV3 = Vector3.zero;
        curState = State.GetHit;
        if (gun != null)
        {
            gun.StopAllCoroutines();
        }
    }

    void StopHitControl()
    {
        curState = State.Normal;
    }
}

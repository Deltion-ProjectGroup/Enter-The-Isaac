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
    public int money = 0;
    public int keys = 0;
    public Text keyUICounter;
    [SerializeField] GameObject reloadIcon;
    public GameObject reloadBar;
    SoundSpawn soundSpawn;
    [Header("Walking related")]
    public float walkSpeed = 1;
    float curWalkSpeed = 0;
    [SerializeField] float rotateSpeed = 1;
    public Transform crosshair;
    [SerializeField] float rotateCrosshairSpeed;
    public float rotGoal = 0;
    [SerializeField] float accelerationSpeed = 3;
    [SerializeField] float decelerationSpeed = 5;
    [SerializeField] AudioClip footStepSound;
    [SerializeField] float footStepSpeed = 0.2f;
    [Header("Gun")]
    public Gun gun;
    public List<GunType> guns;
    [HideInInspector] public int curGun = 0;
    [HideInInspector] public List<int> ammoStore = new List<int>();
    [HideInInspector] public List<int> magazineStore = new List<int>();
    public Gun.voidDelegate gunDel;
    [SerializeField] AudioClip reloadSound;
    public AudioClip realReloadSound;
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
    public playerDelegate onSwapGun;
    public enum State
    {
        Normal,
        Roll,
        GetHit
    }
    [Header("States")]
    public State curState = State.Normal;
    [Header("Rolling")]
    public float rollSpeed = 40;
    public float rollDeceleration = 100;
    [SerializeField] AudioClip rollSound;
    [SerializeField] GameObject rollParticle;
    [Header("INTERACTION")]
    public string interactButton;
    public float interactRadius;
    public LayerMask interactablesLayer;
    public GameObject closestInteractableObject;
    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        soundSpawn = FindObjectOfType<SoundSpawn>();


        for (int i = 0; i < guns.Count; i++)
        {
            ammoStore.Add(guns[i].magazineSize);
        }
        for (int i = 0; i < guns.Count; i++)
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
        //setting the reload icon
        if (reloadIcon.activeSelf != (gun.curAmmo <= 0))
        {
            reloadIcon.transform.localScale = new Vector3(0, 2, 0);
            reloadIcon.SetActive(gun.curAmmo <= 0);
        }
        reloadIcon.SetActive(!gun.IsInvoking("Reload"));
        CheckClosestInteractable();
        Interact();
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
        if (Input.GetAxis(scrollInput) != 0 && GameObject.FindGameObjectWithTag("LaserHold") == null && IsInvoking("NoSwitchGun") == false && gun.IsInvoking("Reload") == false)
        {
            Invoke("NoSwitchGun", 0.2f);
            gun.transform.Rotate(-90, 0, 0);
            soundSpawn.SpawnEffect(reloadSound);
            if (Input.GetAxis(scrollInput) > 0)
            {
                curGun = (int)Mathf.Repeat(curGun + 1, guns.Count);
            }
            else
            {
                curGun = (int)Mathf.Repeat(curGun - 1, guns.Count);
            }
        }
    }

    void NoSwitchGun()
    {
        //invoke function lol
    }

    void SetAnimValues()
    {
        anim.SetFloat("posChangedDistance", Vector2.SqrMagnitude(new Vector2(Input.GetAxis(horInput), Input.GetAxis(vertInput))));

        Vector3 inputDir = new Vector3(Input.GetAxis(horInput), 0, Input.GetAxis(vertInput));
        inputDir = transform.InverseTransformDirection(inputDir);

        anim.SetFloat("horInput", -inputDir.x);
        anim.SetFloat("vertInput", -inputDir.z);

        if(anim.GetCurrentAnimatorStateInfo(1).IsTag("Walk") == true && IsInvoking("PlayFootStepSound") == false && curState == State.Normal){
            Invoke("PlayFootStepSound",footStepSpeed);
        }
    }

   void PlayFootStepSound(){
       soundSpawn.SpawnEffect(footStepSound,0.4f,0.8f,1,transform);
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
        soundSpawn.SpawnEffect(rollSound);
        anim.Play("Dash", 0);
        anim.SetLayerWeight(1, 0);
        GetComponentInChildren<IKHoldGun>().enabled = false;
        Vector3 lastGunScale = Vector3.zero;
        if (gun != null)
        {
            lastGunScale = gun.transform.localScale;
            gun.transform.localScale = Vector3.zero;
        }
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
        if (rollParticle != null)
        {
            Instantiate(rollParticle, transform.position, rollParticle.transform.rotation * transform.rotation * Quaternion.Euler(0, 180, 0));
        }
        transform.Find("Line").gameObject.SetActive(true);
        cam.GetComponent<Shake>().SmallShake();
        yield return new WaitForSeconds(0.3f);
        if (gun != null)
        {
            gun.transform.localScale = lastGunScale;
        }
        anim.SetLayerWeight(1, 1);
        GetComponentInChildren<IKHoldGun>().enabled = true;
        rotateSpeed = oldRotSpeed;
        curState = State.Normal;
        hitbox.SetActive(true);
        transform.Find("Line").gameObject.SetActive(false);
        if (new Vector2(Input.GetAxis(horInput), Input.GetAxis(vertInput)).sqrMagnitude == 0)
        {
            anim.Play("Run", 1);
        }
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
        //moveV3.y = -9.81f;
        moveV3.y = -0.5f;// our floor is extremely thin, so if there's a little lag you fall through the floor, so almost no gravity this time
    }

    void FinalMove()
    {
        cc.Move(moveV3 * Time.deltaTime);
    }

    Vector3 lastGunScale = Vector3.zero;
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
        anim.Play("Flinch", 0);
        anim.SetLayerWeight(1, 0);
        anim.GetComponent<IKHoldGun>().enabled = false;
        if (gun != null)
        {
            lastGunScale = gun.transform.localScale;
            gun.transform.localScale = Vector3.zero;
        }
    }

    void StopHitControl()
    {
        curState = State.Normal;
        anim.SetLayerWeight(1, 1);
        anim.GetComponent<IKHoldGun>().enabled = true;
        if (gun != null)
        {
            gun.transform.localScale = lastGunScale;
        }
    }
    public void Interact()
    {
        if (closestInteractableObject)
        {
            if (Input.GetButtonDown(interactButton))
            {
                foreach (Interactable interactableComponent in closestInteractableObject.GetComponents<Interactable>())
                {
                    interactableComponent.Interact(gameObject);
                }
            }
        }
    }
    public void CheckClosestInteractable()
    {
        Collider[] allInteractables = Physics.OverlapSphere(transform.position, interactRadius, interactablesLayer);
        if (closestInteractableObject)
        {
            //RMOVE OUTLINE RENDERER
            closestInteractableObject = null;
        }
        if (allInteractables.Length > 0)
        {
            if (allInteractables.Length > 1)
            {
                closestInteractableObject = allInteractables[0].gameObject;
                float closestDistance = Vector3.Distance(transform.position, closestInteractableObject.transform.position);
                foreach (Collider interactable in allInteractables)
                {
                    if (Vector3.Distance(interactable.transform.position, transform.position) < closestDistance)
                    {
                        closestDistance = Vector3.Distance(interactable.transform.position, transform.position);
                        closestInteractableObject = interactable.gameObject;
                    }
                }
            }
            else
            {
                closestInteractableObject = allInteractables[0].gameObject;
            }
            //ADD OUTLINE RENDERER TO CLOSEST OBJECT
        }
    }
}

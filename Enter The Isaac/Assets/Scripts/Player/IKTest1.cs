using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTest1 : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] Transform head;
    Transform lastRot;
    Animator anim;
    [SerializeField] bool withIK = true;

    void Start()
    {
        head = transform;
        //head = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
        lastRot = new GameObject().transform;
        lastRot.name = "";

        anim = GetComponent<Animator>();

    }

    void LateUpdate()
    {
        if (withIK == false)
        {
            LookAtTarget();
        }
    }


    void LookAtTarget()
    {
        lastRot.position = head.position;
        Quaternion targetRotation = Quaternion.LookRotation(target.position - lastRot.position);
        lastRot.rotation = Quaternion.Lerp(lastRot.rotation, targetRotation, Time.deltaTime * 10);
        head.rotation = lastRot.rotation;
        head.Rotate(offset);

    }

    void IKLookAt()
    {
        if (target != null)
        {
            Debug.DrawLine(transform.position, target.position, Color.red, Time.deltaTime);
            anim.SetLookAtWeight(1);
           // anim.SetBoneLocalRotation(HumanBodyBones.Head,Quaternion.Euler(0,Mathf.PingPong(Time.time, 45),0));
           print(anim.GetBoneTransform(HumanBodyBones.Head).parent.name);
            anim.SetLookAtPosition(target.position);
        }
    }

    void OnAnimatorIK()
    {
        if (withIK == true)
        {
            IKLookAt();
        }
    }
}

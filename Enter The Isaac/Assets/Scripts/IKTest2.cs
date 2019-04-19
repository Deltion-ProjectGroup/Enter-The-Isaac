using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTest2 : MonoBehaviour
{

    Animator anim;
    [SerializeField] Transform ikHelpLeft;
    [SerializeField] Transform ikHelpRight;
    [SerializeField] float add = 0;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnAnimatorIK()
    {
        RaycastHit hit;
        //left leg
        Ray ray = new Ray(anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position, -transform.up);
        ray.direction = anim.GetBoneTransform(HumanBodyBones.LeftFoot).position - ray.origin;
        if (Physics.Raycast(ray, out hit, Vector3.Distance(ray.origin, anim.GetBoneTransform(HumanBodyBones.LeftFoot).position) + add))
        {
            ikHelpLeft.position = hit.point - (ray.direction * add);
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, ikHelpLeft.position);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        }
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
        }

        //right leg
        ray = new Ray(anim.GetBoneTransform(HumanBodyBones.RightUpperLeg).position, -transform.up);
        if (Physics.Raycast(ray, out hit, Vector3.Distance(ray.origin, anim.GetBoneTransform(HumanBodyBones.RightFoot).position) + add))
        {
            ikHelpRight.position = hit.point - (ray.direction * add);
            anim.SetIKPosition(AvatarIKGoal.RightFoot, ikHelpRight.position);
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        }
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
        }

        //ignore ik at animations
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("IgnoreIK") == true)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
        }
    }
}

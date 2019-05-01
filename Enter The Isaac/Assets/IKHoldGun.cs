﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHoldGun : MonoBehaviour
{

    Animator anim;
    Transform leftHandPos;
    Transform rightHandPos;
    [SerializeField] string leftHandName = "LeftHandIK";
    [SerializeField] string rightHandName = "RightHandIK";
    [SerializeField] string bellyName = "RotateBelly";
    [SerializeField] float rotateBelly;


    void Start()
    {
        anim = GetComponent<Animator>();
        InvokeRepeating("FindHands", 0, 0.01f);
    }
    void FindHands()
    {
        leftHandPos = null;
        rightHandPos = null;
        if (GameObject.Find(leftHandName))
        {
            leftHandPos = GameObject.Find(leftHandName).transform;
        }
        if (GameObject.Find(rightHandName))
        {
            rightHandPos = GameObject.Find(rightHandName).transform;
        }
        //also find belly thing
         if (GameObject.Find(bellyName))
        {
            rotateBelly = GameObject.Find(bellyName).transform.localEulerAngles.y;
        } else {
            rotateBelly = 0;
        }
    }
    void OnAnimatorIK()
    {
        if (leftHandPos != null)
        {
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos.position);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        }
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
        }

        if (rightHandPos != null)
        {
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos.position);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        }
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        }

        anim.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(0, rotateBelly, 0));


        //ignore ik at animations
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("IgnoreIK") == true)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        }
    }
}

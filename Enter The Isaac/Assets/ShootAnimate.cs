using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAnimate : MonoBehaviour {
    Animator anim;
    PlayerController player;
    void Start () {
        anim = GetComponent<Animator> ();
        player = FindObjectOfType<PlayerController> ();
    }

    void LateUpdate () {
        if (player != null && player.gun.IsInvoking ("Shoot") == true) {
            anim.SetFloat ("speed", 1);
        } else {
            anim.SetFloat ("speed", 0);
        }
    }
}
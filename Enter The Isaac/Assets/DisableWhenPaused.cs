using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWhenPaused : MonoBehaviour {
    Pause pauseSetter;
    [SerializeField] GameObject toSet;
    void Start () {
        pauseSetter = FindObjectOfType<Pause> ();
        if (pauseSetter == null) {
            this.enabled = false;
        }
    }

    void LateUpdate () {
        if (pauseSetter.isPaused == true) {
            toSet.SetActive (false);
        } else {
            toSet.SetActive (true);
        }
    }
}
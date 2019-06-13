using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectArrow : MonoBehaviour {
    [SerializeField] Transform[] posses;
    [SerializeField] Vector3 offset;

    void Start(){
        UpdatePosition(0);
    }
    public void UpdatePosition (int newPos) {
        transform.position = posses[newPos].position + offset;
    }
}
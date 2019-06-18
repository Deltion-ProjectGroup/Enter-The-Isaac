using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    void Start(){
        InvokeRepeating("CheckController",0,0.5f);
    }
    void CheckController () {
        string[] names = Input.GetJoystickNames ();
        bool isController = false;
        for (int i = 0; i < names.Length; i++)
        {
            if(names[i].Length == 19){
                isController = true;
            }
        }
       transform.GetChild(0).gameObject.SetActive(!isController);
       transform.GetChild(1).gameObject.SetActive(isController);
    }
}
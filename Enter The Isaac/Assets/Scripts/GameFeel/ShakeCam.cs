using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCam : MonoBehaviour
{

    Shake camShake;
    void Awake()
    {
        if (Camera.main != null && Camera.main.GetComponent<Shake>() != null)
        {
            camShake = Camera.main.GetComponent<Shake>();
        }
    }

    public void SmallShake(){
        camShake.SmallShake();
    }

    public void MediumShake(){
        camShake.MediumShake();
    }

    public void HardShake(){
        camShake.HardShake();
    }

    public void CustomShake(float time,float strength){
        camShake.CustomShake(time,strength);
    }
}

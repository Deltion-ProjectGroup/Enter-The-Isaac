using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HATBase : MonoBehaviour
{  
    [HideInInspector] public PlayerController player;
    public void GetPlayer(PlayerController p){
        player = p;
    }
    public virtual void StopHat(){
        print("base stop");
    }
}

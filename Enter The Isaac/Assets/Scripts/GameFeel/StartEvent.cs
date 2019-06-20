using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class StartEvent : MonoBehaviour
{
    [SerializeField] UnityEvent ev;
    [SerializeField] float waitTime = 0;
    void Start()
    {
        Invoke("DoEvent",waitTime);
    }

    void DoEvent(){
        ev.Invoke();
    }
}

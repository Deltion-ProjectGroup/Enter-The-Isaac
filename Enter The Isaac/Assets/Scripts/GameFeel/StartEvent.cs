using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class StartEvent : MonoBehaviour
{
    [SerializeField] UnityEvent ev;
    void Start()
    {
        ev.Invoke();
    }
}

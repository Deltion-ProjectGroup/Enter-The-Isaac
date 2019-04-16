using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{

    [SerializeField] string requiredComponent;
    [SerializeField] EventArray[] timedEvents;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent(requiredComponent) != null)
        {
            float totalTime = 0;
            for (int i = 0; i < timedEvents.Length; i++)
            {
                totalTime += timedEvents[i].nextEvent;
                StartCoroutine(TimedEvents(timedEvents[i].curEvent, totalTime));
            }
        }
    }

     IEnumerator TimedEvents(UnityEvent ev, float time)
    {
        yield return new WaitForSeconds(time);
        ev.Invoke();
    }
}

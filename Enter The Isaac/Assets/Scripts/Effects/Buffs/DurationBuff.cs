using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurationBuff : MonoBehaviour
{
    public int duration;
    public virtual IEnumerator Duration(int duration)
    {
        yield return new WaitForSeconds(duration);
        RemoveEffect();
    }
    public void ResetTimer()
    {
        StopAllCoroutines();
        StartCoroutine(Duration(duration));
    }
    public virtual void RemoveEffect()
    {
        Destroy(this);
    }
}

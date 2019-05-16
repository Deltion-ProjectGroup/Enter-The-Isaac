using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public abstract void ApplyEffect(GameObject target);
    public abstract void RemoveEffect(GameObject target);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    public GameObject target;
    public float speed;

    public void StartMove()
    {
        StartCoroutine(Move());
    }
    public void StopMove()
    {
        StopAllCoroutines();
    }

    IEnumerator Move()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position, speed);
            yield return null;
        }
    }
}

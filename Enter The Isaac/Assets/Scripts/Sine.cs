using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sine : MonoBehaviour
{

    float time;
    [SerializeField] AnimationCurve curve;
    public float speed = 1;
    float startY;
    [SerializeField] float scale = 1;
    void Start()
    {
        startY = transform.position.y;
        time = Random.Range(0,curve[curve.length - 1].time);
    }

    void Update()
    {
        time = Mathf.Repeat(time + Time.deltaTime * speed,curve[curve.length - 1].time);
        transform.position = new Vector3(transform.position.x,startY + curve.Evaluate(time) * scale,transform.position.z);
        startY = transform.position.y - curve.Evaluate(time) * scale;
    }
}

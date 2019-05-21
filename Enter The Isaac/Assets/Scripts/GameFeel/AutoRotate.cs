using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]

public class AutoRotate : MonoBehaviour
{

    public Vector3 tranformV3 = Vector3.zero;
    public Vector3 eulerV3 = Vector3.zero;
    public Vector3 scaleV3 = Vector3.zero;
    public float speed = 1;
    [SerializeField] bool moveInEditMode = true;

    void Start()
    {

    }

    void Update()
    {
        bool ignore = false;
        if (Application.isPlaying == false && moveInEditMode == false)
        {
            ignore = true;
        }
        if (ignore == false)
        {
            transform.position += transform.TransformDirection(tranformV3 * speed * Time.deltaTime);
            transform.Rotate(eulerV3 * speed * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, scaleV3, speed * Time.deltaTime);
        }
    }
}

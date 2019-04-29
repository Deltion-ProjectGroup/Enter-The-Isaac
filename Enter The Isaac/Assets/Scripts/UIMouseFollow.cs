using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMouseFollow : MonoBehaviour
{
    RectTransform rect;
    Vector3 startPos;
    [SerializeField] float scale = 10;
    [SerializeField] float lerpSpeed = 10;
    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.position;
    }

    void Update()
    {
        Vector3 mousePos = new Vector3((Input.mousePosition.x - Screen.width) / Screen.width, 0, (Input.mousePosition.y - Screen.height) / Screen.height);
        mousePos += new Vector3(0.5f, 0, 0.5f);
        mousePos = new Vector3(Mathf.Max(mousePos.x, -0.5f), Mathf.Max(mousePos.y, -0.5f), Mathf.Max(mousePos.z, -0.5f));
        mousePos = new Vector3(Mathf.Min(mousePos.x, 0.5f), Mathf.Min(mousePos.y, 0.5f), Mathf.Min(mousePos.z, 0.5f));
        mousePos = new Vector3(mousePos.x,mousePos.z,mousePos.y);
        rect.position = Vector3.Lerp(rect.position,startPos + mousePos * scale,Time.deltaTime * lerpSpeed);
    }
}

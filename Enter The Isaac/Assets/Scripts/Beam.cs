using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour {

    [ColorUsage(true, true)]
    public Color color;

    public float speed;

    public Material material;

    void Start ()
    {
        material.SetColor("_TintColor", color);
	}
	
	void Update ()
    {
        material.SetColor("_TintColor", color);

        Vector2 offset = material.GetTextureOffset("_MainTex");
        offset.x -= speed * Time.deltaTime;
        material.SetTextureOffset("_MainTex", offset);
	}
}

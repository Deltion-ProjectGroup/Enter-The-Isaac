using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMagName : MonoBehaviour
{

    TextMesh mesh;
    void Start()
    {
        mesh = GetComponent<TextMesh>();
        if(PlayerPrefs.GetInt ("doomMusic") == 1){
            mesh.text = "CUM MAGICIAN";
        }
    }
}

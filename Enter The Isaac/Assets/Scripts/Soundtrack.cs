using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Soundtrack Variant", menuName = "Soundtrack")]
public class Soundtrack : ScriptableObject
{

    public OST[] ost;

}

[System.Serializable]
public class OST
{
    [Space]
    [Space]
    [Space]
    public AudioClip clip;
    public bool rememberTime = false;
    [Range(0,1)] public float volume = 1;
}

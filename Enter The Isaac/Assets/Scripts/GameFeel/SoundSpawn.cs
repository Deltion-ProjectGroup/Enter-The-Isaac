using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSpawn : MonoBehaviour
{
    public void SpawnEffect(AudioClip clip){
        GameObject g = new GameObject();
        Destroy(g,clip.length);
        g.AddComponent<AudioSource>();
        AudioSource source = g.GetComponent<AudioSource>();
        source.clip = clip;
        source.Play();
    }
}

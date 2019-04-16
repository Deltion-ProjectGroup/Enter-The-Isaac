using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSpawn : MonoBehaviour
{
    public void SpawnEffect(AudioClip clip){
        GameObject g = Instantiate(new GameObject(),transform.position,transform.rotation);
        g.AddComponent<AudioSource>();
        AudioSource source = g.GetComponent<AudioSource>();
        source.clip = clip;
        source.Play();
    }
}

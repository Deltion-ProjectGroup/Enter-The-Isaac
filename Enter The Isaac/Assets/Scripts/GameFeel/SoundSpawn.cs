using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSpawn : MonoBehaviour
{
    public void SpawnEffect(AudioClip clip)
    {
        if (clip != null)
        {
            GameObject g = new GameObject();
            Destroy(g, clip.length);
            g.AddComponent<AudioSource>();
            AudioSource source = g.GetComponent<AudioSource>();
            source.clip = clip;
            source.Play();
            if (GetComponent<AudioSource>() != null)
            {
                AudioSource mySource = GetComponent<AudioSource>();
                source.volume = mySource.volume;
                source.pitch = mySource.pitch;
                source.loop = mySource.loop;
                source.outputAudioMixerGroup = mySource.outputAudioMixerGroup;
                source.spatialBlend = mySource.spatialBlend;
            }
        }
    }
}

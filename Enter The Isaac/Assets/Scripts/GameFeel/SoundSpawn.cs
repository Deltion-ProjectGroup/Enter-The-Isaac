﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSpawn : MonoBehaviour
{

    [SerializeField] AudioMixerGroup mixerGroup;
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
            source.outputAudioMixerGroup = mixerGroup;
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

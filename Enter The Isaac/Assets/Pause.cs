using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Audio;

public class Pause : MonoBehaviour
{
    [SerializeField] string pauseButton = "Pause";
    public bool isPaused = false;
    [SerializeField] UnityEvent setPauseEvent;
    [SerializeField] UnityEvent unPauseEvent;
    [SerializeField] PostProcessVolume ppVolume;
    [SerializeField] AudioMixerGroup audioMixer;
    float ignorePause = 0;
    void Update()
    {
        if (Input.GetButtonDown(pauseButton) == true && Mathf.Approximately(ignorePause,0) == true)
        {
            ignorePause = 0.3f;
            isPaused = !isPaused;
            DepthOfField depth;
            ppVolume.profile.TryGetSettings(out depth);
            if (depth != null)
            {
                depth.enabled.value = isPaused;
            }
            if (isPaused == false)
            {
                unPauseEvent.Invoke();
                Time.timeScale = 1;
                audioMixer.audioMixer.SetFloat("pauseEffect",22000);
            }
            else
            {
                setPauseEvent.Invoke();
                Time.timeScale = 0;
                audioMixer.audioMixer.SetFloat("pauseEffect",300);
            }
        }
        ignorePause = Mathf.MoveTowards(ignorePause,0,Time.unscaledDeltaTime); 
    }
}

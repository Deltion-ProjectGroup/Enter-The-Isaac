using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public class Pause : MonoBehaviour
{
    [SerializeField] string pauseButton = "Pause";
    public bool isPaused = false;
    [SerializeField] UnityEvent setPauseEvent;
    [SerializeField] UnityEvent unPauseEvent;
    [SerializeField] PostProcessVolume ppVolume;
    void Update()
    {
        if (Input.GetButtonDown(pauseButton) == true)
        {
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
            }
            else
            {
                setPauseEvent.Invoke();
                Time.timeScale = 0;
            }
        }
    }
}

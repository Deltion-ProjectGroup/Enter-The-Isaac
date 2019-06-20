using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public class Pause : MonoBehaviour {
    [SerializeField] string pauseButton = "Pause";
    public bool isPaused = false;
    [SerializeField] UnityEvent setPauseEvent;
    [SerializeField] UnityEvent unPauseEvent;
    [SerializeField] PostProcessVolume ppVolume;
    [SerializeField] AudioMixerGroup audioMixer;
    Crosshair cross;

    float ignorePause = 0;

    void Start () {
        cross = FindObjectOfType<Crosshair> ();
        if (isPaused == false) {
            Time.timeScale = 1;
            if (audioMixer != null) {
                audioMixer.audioMixer.SetFloat ("pauseEffect", 22000);
            }
        } else {
            Time.timeScale = 0;
            if (audioMixer != null) {
                audioMixer.audioMixer.SetFloat ("pauseEffect", 300);
            }
        }
    }
    void Update () {
        if (pauseButton != "") {
            if (Input.GetButtonDown (pauseButton) == true && Mathf.Approximately (ignorePause, 0) == true) {
                PauseFuntion ();
            }
            ignorePause = Mathf.MoveTowards (ignorePause, 0, Time.unscaledDeltaTime);

            //related to mouse visibility when pausing
            if (cross != null) {
                if (cross.mouseControlled == false && isPaused == false) {
                    Cursor.visible = false;
                } else {
                    Cursor.visible = true;
                }
            } else {
                Cursor.visible = true;
            }
        }
    }

    public void PauseFuntion () {
        Time.timeScale = 0;
        DepthOfField depth;
        ppVolume.profile.TryGetSettings (out depth);
        if (depth != null) {
            depth.enabled.value = !isPaused;
        }
        ignorePause = 0.3f;
        StartCoroutine (InvokedPauseSetter ());

        if (cross != null && cross.mouseControlled == false && FindObjectOfType<MoveMouseWithInput> () != null) {
            FindObjectOfType<MoveMouseWithInput> ().SetMouseToCenter ();
        }
    }

    IEnumerator InvokedPauseSetter () {
        yield return new WaitForSecondsRealtime (0.05f);
        isPaused = !isPaused;
        if (isPaused == false) {
            unPauseEvent.Invoke ();
            Time.timeScale = 1;
            audioMixer.audioMixer.SetFloat ("pauseEffect", 22000);
        } else {
            setPauseEvent.Invoke ();
            Time.timeScale = 0;
            audioMixer.audioMixer.SetFloat ("pauseEffect", 300);
        }
    }

}
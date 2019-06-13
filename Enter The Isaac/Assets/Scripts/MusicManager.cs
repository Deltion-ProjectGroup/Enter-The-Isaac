using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

    AudioSource source;
    [SerializeField] SoundTracks[] soundTrack;
    public int currentSoundtrackPool = 0;
    List<float> savedTimes = new List<float> ();
    int randomSoundtrack = 0;
    public int curTrack = 0;
    int lastTrack = 0;
    int fadeState = 0;
    [SerializeField] string dungeonLevelName = "BossScene";
    [SerializeField] bool useDoomSoundtrack = false;

    void Awake () {
        SetTrackPool ();
        randomSoundtrack = Random.Range (0, soundTrack[currentSoundtrackPool].soundTrack.Length);
        float[] helper = new float[soundTrack[currentSoundtrackPool].soundTrack[randomSoundtrack].ost.Length];
        savedTimes.AddRange (helper);

        source = GetComponent<AudioSource> ();
        lastTrack = curTrack;



        curTrack = 0;
        source.clip = soundTrack[currentSoundtrackPool].soundTrack[randomSoundtrack].ost[curTrack].clip;
        if (soundTrack[currentSoundtrackPool].soundTrack[randomSoundtrack].ost[curTrack].clip != null) {
            //Invoke("PlaySource", 0.6f);
            PlaySource ();
        }

    }

    public void SetTrackPool () {
        useDoomSoundtrack = (PlayerPrefs.GetInt ("doomMusic") == 1);
        if (SceneManager.GetActiveScene ().name == dungeonLevelName) {
            currentSoundtrackPool = 0;
        } else {
            currentSoundtrackPool = 1;
        }

        if (useDoomSoundtrack == true) {
            currentSoundtrackPool += 2;
        }
        //currentSoundtrackPool = 
    }

    void Update () {
        if (curTrack != lastTrack) {
            UpdateMusic (soundTrack[currentSoundtrackPool].soundTrack[randomSoundtrack].ost[curTrack].clip);
            lastTrack = curTrack;
        }

        /*These are examples for the fucntions to activate
       if (Input.GetButtonDown("Fire1"))
       {
           int newTrack = (int)Mathf.Repeat(curTrack + 1, soundTrack[randomSoundtrack].ost.Length);
           FadeToNewMusic(newTrack);
       }
       if (Input.GetButtonDown("Fire2"))
       {
           FadeMusic();
       }
        */
    }

    public void UpdateMusic (AudioClip clip) {
        if (source.clip != clip) {
            if (soundTrack[currentSoundtrackPool].soundTrack[randomSoundtrack].ost[curTrack].rememberTime == true && source.clip != clip) {
                savedTimes[curTrack] = source.time;
            }

            source.clip = clip;
            if (clip != null) {
                //Invoke("PlaySource", 0.6f);
                PlaySource ();
            }
        }
    }
    public void UpdateMusic (int clip) {
        if (source.clip != soundTrack[currentSoundtrackPool].soundTrack[randomSoundtrack].ost[clip].clip) {
            if (soundTrack[currentSoundtrackPool].soundTrack[randomSoundtrack].ost[curTrack].rememberTime == true && curTrack != clip) {
                savedTimes[curTrack] = source.time;
            }

            curTrack = clip;
            source.clip = soundTrack[currentSoundtrackPool].soundTrack[randomSoundtrack].ost[curTrack].clip;
            if (soundTrack[currentSoundtrackPool].soundTrack[randomSoundtrack].ost[curTrack].clip != null) {
                //Invoke("PlaySource", 0.6f);
                PlaySource ();
            }
        }
    }

    public void FadeToNewMusic (int newTrack) {
        if (source.clip != soundTrack[currentSoundtrackPool].soundTrack[randomSoundtrack].ost[newTrack].clip) {
            StopCoroutine ("MusicFadeEvent");
            StartCoroutine (MusicFadeEvent (newTrack));
        }
    }

    IEnumerator MusicFadeEvent (int newSong) {
        FadeMusic ();
        yield return new WaitUntil (() => fadeState != 0);
        UpdateMusic (newSong);
    }

    void PlaySource () {
        source.Stop ();
        source.time = savedTimes[curTrack];
        source.volume = soundTrack[currentSoundtrackPool].soundTrack[randomSoundtrack].ost[curTrack].volume;
        source.Play ();
    }

    void FadeMusic () {
        fadeState = 0;
        CancelInvoke ("FadeMusicEvent");
        InvokeRepeating ("FadeMusicEvent", 0, 0.01f);
    }

    void FadeMusicEvent () {
        switch (fadeState) {
            case 0:
                source.volume = Mathf.MoveTowards (source.volume, 0, Time.unscaledDeltaTime);
                if (source.volume == 0) {
                    fadeState = 1;
                }
                break;
            case 1:
                source.volume = Mathf.MoveTowards (source.volume, 1, Time.unscaledDeltaTime);
                if (source.volume == 1) {
                    fadeState = 0;
                    CancelInvoke ("FadeMusicEvent");
                }
                break;
        }
    }
}

[System.Serializable]
public class SoundTracks {
    public Soundtrack[] soundTrack;
}
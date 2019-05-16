using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    AudioSource source;
    [SerializeField] Soundtrack[] soundTrack;
    List<float> savedTimes = new List<float>();
    int randomSoundtrack = 0;
    [SerializeField] int curTrack = 0;
    int lastTrack = 0;
    int fadeState = 0;

    void Start()
    {
        randomSoundtrack = Random.Range(0, soundTrack.Length);
        float[] helper = new float[soundTrack[randomSoundtrack].ost.Length];
        savedTimes.AddRange(helper);

        source = GetComponent<AudioSource>();
        lastTrack = curTrack;
        UpdateMusic(curTrack);
    }

    void Update()
    {
        if (curTrack != lastTrack)
        {
            UpdateMusic(soundTrack[randomSoundtrack].ost[curTrack].clip);
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

    public void UpdateMusic(AudioClip clip)
    {
        if (soundTrack[randomSoundtrack].ost[curTrack].rememberTime == true && source.clip != clip)
        {
            savedTimes[curTrack] = source.time;
        }

        source.clip = clip;
        if (clip != null)
        {
            Invoke("PlaySource", 0.6f);
        }
    }
    public void UpdateMusic(int clip)
    {
        if (soundTrack[randomSoundtrack].ost[curTrack].rememberTime == true && curTrack != clip)
        {
            savedTimes[curTrack] = source.time;
        }

        curTrack = clip;
        source.clip = soundTrack[randomSoundtrack].ost[curTrack].clip;
        if (soundTrack[randomSoundtrack].ost[curTrack].clip != null)
        {
            Invoke("PlaySource", 0.6f);
        }
    }

    public void FadeToNewMusic(int newTrack)
    {
        StopCoroutine("MusicFadeEvent");
        StartCoroutine(MusicFadeEvent(newTrack));
    }

    IEnumerator MusicFadeEvent(int newSong)
    {
        FadeMusic();
        yield return new WaitUntil(() => fadeState != 0);
        UpdateMusic(newSong);
    }

    void PlaySource()
    {
        source.Stop();
        source.time = savedTimes[curTrack];
        source.volume = soundTrack[randomSoundtrack].ost[curTrack].volume;
        source.Play();
    }

    void FadeMusic()
    {
        fadeState = 0;
        CancelInvoke("FadeMusicEvent");
        InvokeRepeating("FadeMusicEvent", 0, 0.01f);
    }

    void FadeMusicEvent()
    {
        switch (fadeState)
        {
            case 0:
                source.volume = Mathf.MoveTowards(source.volume, 0, Time.unscaledDeltaTime);
                if (source.volume == 0)
                {
                    fadeState = 1;
                }
                break;
            case 1:
                source.volume = Mathf.MoveTowards(source.volume, 1, Time.unscaledDeltaTime);
                if (source.volume == 1)
                {
                    fadeState = 0;
                    CancelInvoke("FadeMusicEvent");
                }
                break;
        }
    }
}

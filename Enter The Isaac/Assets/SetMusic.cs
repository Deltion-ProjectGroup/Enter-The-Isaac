using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMusic : MonoBehaviour {
    public void SetMusics (int value) {
        if (FindObjectOfType<MusicManager> () != null) {
            FindObjectOfType<MusicManager> ().UpdateMusic (value);
        }
    }
    public void SetMusics (AudioClip value) {
        if (FindObjectOfType<MusicManager> () != null) {
            FindObjectOfType<MusicManager> ().UpdateMusic (value);
        }
    }
    public void FadeMusics (int value) {
        if (FindObjectOfType<MusicManager> () != null) {
            FindObjectOfType<MusicManager> ().FadeToNewMusic (value);
        }
    }

}
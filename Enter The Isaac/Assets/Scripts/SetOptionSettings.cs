using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetOptionSettings : MonoBehaviour {
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider mainVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Slider mouseFollowSlider;
    [SerializeField] Slider nightcoreSlider;
    [SerializeField] Toggle screenShakeToggle;
    [SerializeField] Toggle doomMusicToggle;
    [SerializeField] GameObject continueButton;

    public void Awake () {
        if (File.Exists (Application.dataPath + "/LevelData")) {
            continueButton.SetActive (true);
        }
    }
    public void SetMainVolume (float value) {
        if (value != 0) {
            mixer.SetFloat ("masterVolume", Mathf.Log10 (value) * 20);
        } else {
            mixer.SetFloat ("masterVolume", -80);
        }
        PlayerPrefs.SetFloat ("masterVolume", value);
    }

    public void SetDOOMMusic (bool value) {
        if (value == false) {
            PlayerPrefs.SetInt ("doomMusic", 0);
        } else {
            PlayerPrefs.SetInt ("doomMusic", 1);
        }
        if (FindObjectOfType<MusicManager> () != null) {
            FindObjectOfType<MusicManager> ().SetTrackPool ();
            FindObjectOfType<MusicManager> ().UpdateMusic (FindObjectOfType<MusicManager> ().curTrack);
        }
    }

    public void QuitGame () {
        print ("quiting");
        Application.Quit ();
    }

    public void SetSFXVolume (float value) {
        if (value != 0) {
            mixer.SetFloat ("sfxVolume", Mathf.Log10 (value) * 20);
        } else {
            mixer.SetFloat ("sfxVolume", -80);
        }
        PlayerPrefs.SetFloat ("sfxVolume", value);
    }

    public void SetMusicVolume (float value) {
        if (value != 0) {
            mixer.SetFloat ("musicVolume", Mathf.Log10 (value) * 20);
        } else {
            mixer.SetFloat ("musicVolume", -80);
        }
        PlayerPrefs.SetFloat ("musicVolume", value);
    }

    public void SetMusicPitch (float value) {
        PlayerPrefs.SetFloat ("musicPitch", value);
        value *= 2;
        mixer.SetFloat ("musicPitch", value);
    }

    public void SetScreenshake (bool value) {
        if (value == false) {
            PlayerPrefs.SetInt ("screenshake", 0);
        } else {
            PlayerPrefs.SetInt ("screenshake", 1);
        }
    }

    public void SetMouseFollowAmount (float value) {
        PlayerPrefs.SetFloat ("mouseFollowAmount", value);
        value -= 0.5f;
        value *= 2;
    }

    public void SetToDefault () {
        mainVolumeSlider.value = 1;
        musicVolumeSlider.value = 1;
        sfxVolumeSlider.value = 1;
        mouseFollowSlider.value = 0.5f;
        nightcoreSlider.value = 0.5f;
        screenShakeToggle.isOn = true;
        doomMusicToggle.isOn = false;
    }

    void Start () {
        SetSavedValues ();
        Resources.UnloadUnusedAssets ();
    }

    void SetSavedValues () {
        mainVolumeSlider.value = PlayerPrefs.GetFloat ("masterVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat ("musicVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat ("sfxVolume");
        mouseFollowSlider.value = PlayerPrefs.GetFloat ("mouseFollowAmount");
        nightcoreSlider.value = PlayerPrefs.GetFloat ("musicPitch");
        screenShakeToggle.isOn = (PlayerPrefs.GetInt ("screenshake") == 1);
        doomMusicToggle.isOn = (PlayerPrefs.GetInt ("doomMusic") == 1);
    }

    public void LoadScene (int newScene) {
        SceneManager.UnloadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene (newScene);
    }

    public void LoadScene (string newScene) {
        SceneManager.UnloadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene (newScene);
    }

    public void LoadSavedScene () {
        GameObject.FindGameObjectWithTag("SceneManager").GetComponent<InGameSceneManager>().LoadSceneAsyncStart(GameObject.FindGameObjectWithTag("Database").GetComponent<SaveDatabase>().LoadSavedLevel());
    }
    public void StartNewGame (float newScene) {
        if (File.Exists (Application.dataPath + "/LevelData")) {
            File.Delete (Application.dataPath + "/LevelData");
        }
        GameObject.FindGameObjectWithTag("SceneManager").GetComponent<InGameSceneManager>().LoadSceneAsyncStart((int)newScene);
    }
    public void LoadSceneASync (float newScene) {
        SceneManager.UnloadScene(SceneManager.GetActiveScene().buildIndex);
        StartCoroutine (LoadSceneASyncIE (newScene));
    }

    IEnumerator LoadSceneASyncIE (float newScene) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync ((int) newScene);

        while (!asyncLoad.isDone) {
            yield return null;
        }
    }

}
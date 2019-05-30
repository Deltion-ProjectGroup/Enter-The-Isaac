using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetOptionSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider mainVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Slider mouseFollowSlider;
    [SerializeField] Slider nightcoreSlider;
    [SerializeField] Toggle screenShakeToggle;
    public void SetMainVolume(float value)
    {
        if (value != 0)
        {
            mixer.SetFloat("masterVolume", Mathf.Log10(value) * 20);
        }
        else
        {
            mixer.SetFloat("masterVolume", -80);
        }
        PlayerPrefs.SetFloat("masterVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        if (value != 0)
        {
            mixer.SetFloat("sfxVolume", Mathf.Log10(value) * 20);
        }
        else
        {
            mixer.SetFloat("sfxVolume", -80);
        }
        PlayerPrefs.SetFloat("sfxVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        if (value != 0)
        {
            mixer.SetFloat("musicVolume", Mathf.Log10(value) * 20);
        }
        else
        {
            mixer.SetFloat("musicVolume", -80);
        }
        PlayerPrefs.SetFloat("musicVolume", value);
    }

    public void SetMusicPitch(float value)
    {
        PlayerPrefs.SetFloat("musicPitch", value);
        value *= 2;
        mixer.SetFloat("musicPitch", value);
    }

    public void SetScreenshake(bool value)
    {
        if (value == false)
        {
            PlayerPrefs.SetInt("screenshake", 0);
        }
        else
        {
            PlayerPrefs.SetInt("screenshake", 1);
        }
    }

    public void SetMouseFollowAmount(float value)
    {
        PlayerPrefs.SetFloat("mouseFollowAmount", value);
        value -= 0.5f;
        value *= 2;
    }

    public void SetToDefault()
    {
        mainVolumeSlider.value = 1;
        musicVolumeSlider.value = 1;
        sfxVolumeSlider.value = 1;
        mouseFollowSlider.value = 0.5f;
        nightcoreSlider.value = 0.5f;
        screenShakeToggle.isOn = true;
    }

    void Start()
    {
        SetSavedValues();
    }

    void SetSavedValues()
    {
        mainVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        mouseFollowSlider.value = PlayerPrefs.GetFloat("mouseFollowAmount");
        nightcoreSlider.value = PlayerPrefs.GetFloat("musicPitch");
        screenShakeToggle.isOn = (PlayerPrefs.GetInt("screenshake") == 1);
    }

    public void LoadScene(int newScene)
    {
        SceneManager.LoadScene(newScene);
    }

    public void LoadScene(string newScene)
    {
        SceneManager.LoadScene(newScene);
    }

    public void LoadSceneASync(float newScene)
    {
        StartCoroutine(LoadSceneASyncIE(newScene));
    }

    IEnumerator LoadSceneASyncIE(float newScene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)newScene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


}

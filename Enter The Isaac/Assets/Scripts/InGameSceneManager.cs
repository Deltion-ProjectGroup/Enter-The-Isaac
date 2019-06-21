using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameSceneManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public Animator loadingScreenAnimator;

    public void Start()
    {
        StartCoroutine(DisableLoading());
    }
    IEnumerator DisableLoading()
    {
        yield return new WaitForSeconds(loadingScreenAnimator.GetCurrentAnimatorStateInfo(0).length);
        loadingScreen.SetActive(false);
    }
    public void LoadSceneAsyncStart(int sceneID_)
    {
        StartCoroutine(LoadSceneAsync(sceneID_));
    }
    public void LoadSceneAsyncStart(string sceneName_)
    {
        StartCoroutine(LoadSceneAsync(sceneName_));
    }
    public IEnumerator LoadSceneAsync(int sceneID)
    {
        loadingScreenAnimator.SetBool("FadeIn", true);
        loadingScreen.SetActive(true);
        loadingScreenAnimator.Play("LoadingFadeIn");
        yield return new WaitForSeconds(loadingScreenAnimator.GetCurrentAnimatorStateInfo(0).length);
        AsyncOperation loadData = SceneManager.LoadSceneAsync(sceneID);
        loadData.allowSceneActivation = true;
        while (!loadData.isDone)
        {
            yield return null;
        }
    }
    public IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreenAnimator.SetBool("FadeIn", true);
        loadingScreen.SetActive(true);
        loadingScreenAnimator.Play("LoadingFadeIn");
        yield return new WaitForSeconds(loadingScreenAnimator.GetCurrentAnimatorStateInfo(0).length);
        AsyncOperation loadData = SceneManager.LoadSceneAsync(sceneName);
        loadData.allowSceneActivation = true;
        while (!loadData.isDone)
        {
            yield return null;
        }
    }
    public void LoadScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

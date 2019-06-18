using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DebugReload : MonoBehaviour {
    public void Reload () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}
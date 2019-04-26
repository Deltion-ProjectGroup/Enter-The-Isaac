using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DebugReload : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

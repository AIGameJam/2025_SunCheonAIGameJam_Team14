using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChange : MonoBehaviour
{
    public string scene; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Change()
    {
        SceneManager.LoadScene(scene);
    }

}

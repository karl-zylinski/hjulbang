using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Starter : MonoBehaviour
{
    void Start()
    {
        GameData.Reset();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameData.Reset();
            SceneManager.LoadScene("World");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}

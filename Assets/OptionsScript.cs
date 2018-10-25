using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour {
    public GameObject pauseCanvas;
    public bool isPaused;
    public bool enableInternalPID = false;

    void Start()
    {
        pauseCanvas.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            } else if (isPaused)
            {
                ContinueGame();
            }
        }
    }

    private void PauseGame()
    {
        Debug.Log("Pause");
        pauseCanvas.SetActive(true);
        isPaused = true;
    }

    public void ContinueGame()
    {
        Debug.Log("Continue");
        pauseCanvas.SetActive(false);
        isPaused = false;
    }

    public void controlTypeChanged()
    {
        int option = GetComponentInChildren<Dropdown>().value;
        if (option == 1)
        {
            enableInternalPID = true;
        } else
        {
            enableInternalPID = false;
        }
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}

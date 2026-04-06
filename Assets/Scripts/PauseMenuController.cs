using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private Button submitButton;
    [SerializeField] private Light directionalLight;
    private bool paused = true;
    private List<Transform> pauseMenuElements = new();
    void Start()
    {
        DontDestroyOnLoad(this);
        foreach(Transform child in transform)
        {
            pauseMenuElements.Add(child);
        }

        // Begin unpaused
        TogglePause();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Menu"))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        paused = !paused;

        // Toggle pause menu elements
        foreach(Transform element in pauseMenuElements)
        {
            element.gameObject.SetActive(paused);
        }

        // Toggle game elements
        submitButton.interactable = !paused;        
        directionalLight.gameObject.SetActive(!paused);
        Time.timeScale = paused ? 0 : 1;
    }
    public void ResumeGame()
    {
        TogglePause();
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
        TogglePause();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool isPaused()
    {
        return paused;
    }
}

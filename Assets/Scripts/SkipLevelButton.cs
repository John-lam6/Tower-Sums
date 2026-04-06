using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipLevelButton : MonoBehaviour
{
    private Button button;
    private GameManager gameManager;
    private int timesFailed = 0;
    void Start()
    {
        button = GetComponent<Button>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Deactivate();
    }
    public void SkipLevel()
    {
        gameManager.LoadNextLevel();
    }
    public void AddFail()
    {
        timesFailed++;
        if(timesFailed == 3)
        {
            Activate();
        }
    }
    public void ResetFailCount()
    {
        timesFailed = 0;
        Deactivate();
    }

    private void Activate()
    {
        button.image.color = Color.white;
        button.interactable = true;
    }

    private void Deactivate()
    {
        button.image.color = Color.gray;
        button.interactable = false;
    }
}

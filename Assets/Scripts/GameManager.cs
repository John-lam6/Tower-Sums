using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject addTower;

    public GameObject subtractionTower;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel(LevelData level) {
        switch (level.mode) {
            case DifficultyMode.AdditionOnly:
                addTower.gameObject.SetActive(true);
                subtractionTower.gameObject.SetActive(false);
                break;
            case DifficultyMode.SubtractionOnly:
                addTower.gameObject.SetActive(false);
                subtractionTower.gameObject.SetActive(true);
                break;
            case DifficultyMode.Both:
                addTower.gameObject.SetActive(true);
                subtractionTower.gameObject.SetActive(true);
                break;
        }
    }
}

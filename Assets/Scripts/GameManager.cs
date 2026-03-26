using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject addTower_GO;
    public GameObject startTower_GO;
    public GameObject subTower_GO;
    public GameObject targetTower_GO;
    public TowerController startTower, addTower, subTower, targetTower;
    public Light dirLight;
    public Light startSpotLight;
    public Light addSpotlight;
    public Light subSpotlight;
    public Light goalSpotlight;

    public TextMeshPro textbox;
    
    public AudioSource audiosource;
    public AudioClip correct_clip;
    public AudioClip wrong_clip;
    public AudioClip value1_clip;
    public AudioClip value2_clip;
    public AudioClip lights_out_clip;

    public Button submitButton;

    private bool isClicked = false;
    
    public void OnSubmit() {
        if (!isClicked) StartCoroutine(OnSubmitCoroutine());
        isClicked = true;
    }

    IEnumerator OnSubmitCoroutine() {
        submitButton.image.color = Color.gray;
        submitButton.interactable = false;
        textbox.color = Color.white;
        int towervalue, result;
        textbox.transform.position = new Vector3(-5.23f, 5.42f, 0.1f);
        textbox.enabled = true;
        goalSpotlight.enabled = false;
        dirLight.enabled = false;
        //yield return new WaitForSeconds(0.3f);
        audiosource.volume = 1;
        audiosource.PlayOneShot(lights_out_clip);
        audiosource.volume = 0.41f;
        yield return new WaitForSeconds(0.8f);
        
        // show start tower
        startSpotLight.enabled = true;
        towervalue = startTower.GetTotalValue();
        result = startTower.GetTotalValue();
            // display result
                // shows the total value of the tower next to the tower, then displays current result
        yield return new WaitForSeconds(0.8f);
        audiosource.PlayOneShot(value1_clip);
        textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString();
                
        // show add tower
            // DoTween the textbox of the current result with an Ease in and an Ease out to the next tower
        yield return new WaitForSeconds(0.8f);
        textbox.text = "Tower Value: ---" + "\nResult: " + result.ToString();
        startSpotLight.enabled = false;
        yield return textbox.transform.DOMoveX(0.16f, 0.5f).SetEase(Ease.InOutSine).WaitForCompletion();
        addSpotlight.enabled = true;
        towervalue = addTower.GetTotalValue();
            // display result
                // shows the total value of the tower next to the tower, then displays current result
        yield return new WaitForSeconds(0.65f);
        audiosource.PlayOneShot(value1_clip);
        //textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString();
        textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString() + " + " + towervalue.ToString();
        result += addTower.GetTotalValue();
        yield return new WaitForSeconds(0.65f);
        audiosource.PlayOneShot(value2_clip);
        textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString();
        
        // show sub tower
            // DoTween the textbox of the current result with an Ease in and an Ease out to the next tower
        yield return new WaitForSeconds(0.8f);
        textbox.text = "Tower Value: ---" + "\nResult: " + result.ToString();
        addSpotlight.enabled = false;
        yield return textbox.transform.DOMoveX(3.9f, 0.5f).SetEase(Ease.InOutSine).WaitForCompletion();
        subSpotlight.enabled = true;
        towervalue = subTower.GetTotalValue();
            // display result
                // shows the total value of the tower next to the tower, then displays current result
        yield return new WaitForSeconds(0.65f);
        audiosource.PlayOneShot(value1_clip);
        //textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString();
        textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString() + " - " + towervalue.ToString();
        result -= subTower.GetTotalValue();
        yield return new WaitForSeconds(0.65f);
        audiosource.PlayOneShot(value2_clip);
        textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString();
                
        // show goal tower
        yield return new WaitForSeconds(0.8f);
        textbox.text = "Tower Value: ---" + "\nResult: " + result.ToString();
        subSpotlight.enabled = false;
        yield return textbox.transform.DOMoveX(8.37f, 0.5f).SetEase(Ease.InOutSine).WaitForCompletion();
        goalSpotlight.enabled = true;
        towervalue = targetTower.GetTotalValue();
        textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString();

        yield return new WaitForSeconds(0.8f);
        // correct
        if (result == targetTower.GetTotalValue()) {
            audiosource.PlayOneShot(correct_clip);
            textbox.color = Color.green;
        }
        // incorrect
        else {
            audiosource.PlayOneShot(wrong_clip);
            textbox.color = Color.red;
        }
    }

    public void resetClicked() {
        isClicked = false;
    }
    
    // Start is called before the first frame update
    void Start() {
        isClicked = false;
        submitButton.image.color = Color.white;
        submitButton.interactable = true;
        textbox.enabled = false;
        startTower = startTower_GO.GetComponent<TowerController>();
        // addTower = addTower_GO.GetComponent<TowerController>();
        // subTower = subTower_GO.GetComponent<TowerController>();
        targetTower = targetTower_GO.GetComponent<TowerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel(LevelData level) {
        switch (level.mode) {
            case DifficultyMode.AdditionOnly:
                addTower.gameObject.SetActive(true);
                subTower.gameObject.SetActive(false);
                break;
            case DifficultyMode.SubtractionOnly:
                addTower.gameObject.SetActive(false);
                subTower.gameObject.SetActive(true);
                break;
            case DifficultyMode.Both:
                addTower.gameObject.SetActive(true);
                subTower.gameObject.SetActive(true);
                break;
        }
    }
}

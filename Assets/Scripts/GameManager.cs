using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public List<LevelData> levels;
    private int currentLevel; 
    public Object baseBlock;
    public GameObject startingTowerBlock;
    public GameObject goalTowerBlock;
    private int startAmount;
    private int goalAmount;
    private float defaultTowerHeight = -1.5f;
    public Transform hotbar;   // your Cube
    public float spacing = 1.5f;
 
    public GameObject addTower_GO;
    public GameObject subTower_GO;
    public GameObject visualAdditionTower, visualSubtractionTower;
    public TowerController addTower, subTower;
    public Light dirLight;
    public Light startSpotLight;
    public Light addSpotlight;
    public Light subSpotlight;
    public Light goalSpotlight;
    public DropHandler additionDropHandler;
    public DropHandler subtractionDropHandler;

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
        towervalue = startAmount;
        result = startAmount;
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
        towervalue = goalAmount;
        textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString();

        yield return new WaitForSeconds(0.8f);
        // correct
        if (result == goalAmount) {
            audiosource.PlayOneShot(correct_clip);
            textbox.color = Color.green;
            yield return new WaitForSeconds(0.8f);
            
            currentLevel++;
            if(currentLevel < levels.Count)
            {
                LoadLevel(levels[currentLevel]);
            } else
            {
                // victory screen or something
            }
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
           
        currentLevel = 0;
        LoadLevel(levels[currentLevel]);
    }

    public void LoadLevel(LevelData level) {
        visualAdditionTower.SetActive(true);
        visualSubtractionTower.SetActive(true);
        switch (level.mode)
        {
            case DifficultyMode.AdditionOnly:
                addTower.gameObject.SetActive(true);
                subTower.gameObject.SetActive(true); // keep active
                visualSubtractionTower.gameObject.SetActive(false);

                additionDropHandler.SetDropEnabled(true);
                subtractionDropHandler.SetDropEnabled(false);
                break;

            case DifficultyMode.SubtractionOnly:
                addTower.gameObject.SetActive(true);
                subTower.gameObject.SetActive(true);
                visualAdditionTower.gameObject.SetActive(false);


                additionDropHandler.SetDropEnabled(false);
                subtractionDropHandler.SetDropEnabled(true);
                break;

            case DifficultyMode.Both:
                addTower.gameObject.SetActive(true);
                subTower.gameObject.SetActive(true);

                additionDropHandler.SetDropEnabled(true);
                subtractionDropHandler.SetDropEnabled(true);
                break;
        }

        BlockData startingBlock = startingTowerBlock.GetComponent<BlockData>();
        startAmount = level.startValue;
        startingBlock.SetValue(startAmount);
        startingTowerBlock.transform.position = new(startingTowerBlock.transform.position.x, defaultTowerHeight + (startAmount / 2.0f * startingBlock.unitHeight));

        BlockData goalBlock = goalTowerBlock.GetComponent<BlockData>();
        goalAmount = level.targetValue;
        goalBlock.SetValue(goalAmount);
        goalTowerBlock.transform.position = new(goalTowerBlock.transform.position.x, defaultTowerHeight + (goalAmount / 2.0f * goalBlock.unitHeight));

        // foreach(int blockValue in level.availableBlocks)
        // {
        //     Instantiate(createBlock(blockValue));
        //     // these should be put in the hotbar
        // }

        int blockCount = 0;

        foreach(int blockValue in level.availableBlocks)
        {
            GameObject newBlock = Instantiate(baseBlock as GameObject);

            // position using index
            Vector3 offset = new Vector3(blockCount * 1.5f, 0f, -0.5f);
            newBlock.transform.position = hotbar.position + offset;

            BlockData blockData = newBlock.GetComponent<BlockData>();
            blockData.isHotbarBlock = true;
            blockData.hotbarPosition = hotbar.position + offset;
            newBlock.transform.position = blockData.hotbarPosition;

            blockData.hasTower = false;
            blockData.SetState(BlockState.InHotbar);
            blockData.SetValue(level.availableBlocks[blockCount]);
            blockData.ApplyHotbarScale();

            DragHandler dragHandler = newBlock.GetComponent<DragHandler>();
            if (dragHandler != null)
            {
                dragHandler.additionDropZone = additionDropHandler;
                dragHandler.subtractionDropZone = subtractionDropHandler;
            }

            blockCount++;
        }
    }

    private Object createBlock(int value)
    {
        Object newBlock = baseBlock;
        BlockData newBlockData = newBlock.GetComponent<BlockData>();
        newBlockData.SetValue(value);
        return newBlock;  
    }
}

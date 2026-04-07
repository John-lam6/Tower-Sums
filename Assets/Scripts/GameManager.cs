using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<LevelData> levels;
    private int currentLevel;
    private LevelData currLevelData;
    public Object baseBlock;
    private List<Object> activeBlocks = new();
    public BlockData startBlock;
    public BlockData goalBlock;
    private int startAmount;
    private int goalAmount;
    public Transform hotbar;   // your Cube
    public float spacing = 1.5f;

    public TextMeshProUGUI add_text;
    public TextMeshProUGUI sub_text;
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
    [SerializeField] private SkipLevelButton skipLevelButton;
    
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
        textbox.text = "Tower Value: ---" + "\nResult: ---";
        textbox.enabled = true;
        goalSpotlight.enabled = false;
        dirLight.enabled = false;
        audiosource.PlayOneShot(lights_out_clip);
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
               
        if (currLevelData != null && (currLevelData.mode == DifficultyMode.AdditionOnly || currLevelData.mode == DifficultyMode.Both)) {
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
            textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString() + " + " +
                           towervalue.ToString();
            result += addTower.GetTotalValue();
            
            StartCoroutine(addTower.MergeAddTower());
            yield return new WaitForSeconds(0.65f);
            
            audiosource.PlayOneShot(value2_clip);
            textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString();
        }

        if (currLevelData != null && (currLevelData.mode == DifficultyMode.SubtractionOnly ||
                                      currLevelData.mode == DifficultyMode.Both)) {
            // show sub tower
            // DoTween the textbox of the current result with an Ease in and an Ease out to the next tower
            yield return new WaitForSeconds(0.8f);
            textbox.text = "Tower Value: ---" + "\nResult: " + result.ToString();
            startSpotLight.enabled = false;
            addSpotlight.enabled = false;
            yield return textbox.transform.DOMoveX(3.9f, 0.5f).SetEase(Ease.InOutSine).WaitForCompletion();
            subSpotlight.enabled = true;
            towervalue = subTower.GetTotalValue();
            // display result
            // shows the total value of the tower next to the tower, then displays current result
            yield return new WaitForSeconds(0.65f);
            audiosource.PlayOneShot(value1_clip);
            textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString() + " - " +
                           towervalue.ToString();
            result -= subTower.GetTotalValue();
            if (result < 0) result = 0;
            
            StartCoroutine(subTower.MergeSubtractTower());
            yield return new WaitForSeconds(0.65f);

            audiosource.PlayOneShot(value2_clip);
            textbox.text = "Tower Value: " + towervalue.ToString() + "\nResult: " + result.ToString();
        }

        // show goal tower
        yield return new WaitForSeconds(0.8f);
        textbox.text = "Tower Value: ---" + "\nResult: " + result.ToString();
        addSpotlight.enabled = false;
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
            yield return new WaitForSeconds(1.6f);
            
            LoadNextLevel();
        }
        // incorrect
        else {
            audiosource.PlayOneShot(wrong_clip);
            textbox.color = Color.red;
            yield return new WaitForSeconds(1.6f);
            startBlock.SetValue(currLevelData.startValue);
            addTower.ResetBlockPositions();
            subTower.ResetBlockPositions();
            skipLevelButton.AddFail();
        }

        goalSpotlight.enabled = false;
        dirLight.enabled = true;
        textbox.enabled = false;
        submitButton.image.color = Color.white;
        submitButton.interactable = true;
        isClicked = false;
    }

    public void resetClicked() {
        isClicked = false;
    }
    
    void Start() {
        isClicked = false;
        submitButton.image.color = Color.white;
        submitButton.interactable = true;
        textbox.enabled = false;
           
        currentLevel = 0;
        LoadLevel(levels[currentLevel]);
    }
    
    public void LoadNextLevel()
    {
        currentLevel++;
        skipLevelButton.ResetFailCount();
            if(currentLevel < levels.Count)
            {
                LoadLevel(levels[currentLevel]);
            } else
            {
                SceneManager.LoadScene("Win");
            }
    }

    public void LoadLevel(LevelData level) {
        currLevelData = level;

        // Clear blocks from previous level
        if(activeBlocks.Count > 0)
        {
            foreach(Object block in activeBlocks)
            {
                Destroy(block);
            }
            activeBlocks.Clear();
        }
        addTower.ClearBlocks();
        subTower.ClearBlocks();

        switch (level.mode) {
            case DifficultyMode.AdditionOnly:
                addTower.transform.parent.gameObject.SetActive(true);
                add_text.gameObject.SetActive(true);
                subTower.transform.parent.gameObject.SetActive(false);
                sub_text.gameObject.SetActive(false);
                break;

            case DifficultyMode.SubtractionOnly:
                addTower.transform.parent.gameObject.SetActive(false);
                add_text.gameObject.SetActive(false);
                subTower.transform.parent.gameObject.SetActive(true);
                sub_text.gameObject.SetActive(true);
                break;

            case DifficultyMode.Both:
                addTower.transform.parent.gameObject.SetActive(true);
                add_text.gameObject.SetActive(true);
                subTower.transform.parent.gameObject.SetActive(true);
                sub_text.gameObject.SetActive(true);
                break;
        }

        startAmount = level.startValue;
        startBlock.SetValue(startAmount);
    
        goalAmount = level.targetValue;
        goalBlock.SetValue(goalAmount);
    
        int blockCount = 0;

        foreach(int blockValue in level.availableBlocks)
        {
            GameObject newBlock = Instantiate(baseBlock as GameObject);

            // position using index
            Vector3 offset = new Vector3(blockCount * 1.5f - 3.0f, 0.5f, -3f);
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
            activeBlocks.Add(newBlock);
        }
    }

    public bool IsSubmitting()
    {
        return isClicked;
    }
}

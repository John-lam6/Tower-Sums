using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [HideInInspector] public List<BlockData> stackedBlocks = new List<BlockData>();
    [SerializeField] private BlockData startBlock;
    private float mergeOffset;

    public int GetTotalValue() {
        int total = 0;
        foreach (var block in stackedBlocks) total += block.value;
        return total;
    }

    public void AddBlock(BlockData block) {
    if (!stackedBlocks.Contains(block)) {
        stackedBlocks.Add(block);
    }
    block.hasTower = true;
}

    public void RemoveBlock(BlockData block) {
        stackedBlocks.Remove(block);
        block.hasTower = false;
    }

    public void ClearBlocks()
    {
        stackedBlocks.Clear();
    }
    public IEnumerator MergeAddTower()
    {
        mergeOffset = startBlock.value * startBlock.unitHeight; 
        foreach(BlockData block in stackedBlocks)
        {
            //block.transform.DOMove(new(startBlock.transform.position.x, block.transform.position.y + mergeOffset), 0.75f)
            block.transform.DOMove (new (startBlock.transform.position.x, block.transform.position.y + mergeOffset),0.75f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => block.gameObject.SetActive(false));
        }
        
        yield return new WaitForSeconds(0.65f);
        startBlock.SetValue(startBlock.value + GetTotalValue());
        // startBlock.ResetTowerHeight();
    }
    public IEnumerator MergeSubtractTower()
    {
        mergeOffset = Math.Max(0, startBlock.value - GetTotalValue()) * startBlock.unitHeight;
        foreach(BlockData block in stackedBlocks)
        {
            //block.transform.DOMove(new(startBlock.transform.position.x, block.transform.position.y + mergeOffset), 0.75f)
            block.transform.DOMove (new (startBlock.transform.position.x, block.transform.position.y),0.75f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => block.gameObject.SetActive(false));
        }
        
        yield return new WaitForSeconds(0.65f);
        startBlock.SetValue(startBlock.value - GetTotalValue());
        // startBlock.ResetTowerHeight();
    }

    public void ResetBlockPositions()
    {
        if(stackedBlocks.Count == 0) return;

        foreach(BlockData block in stackedBlocks)
        {
            block.transform.position = new(transform.position.x, block.transform.position.y - mergeOffset);
            block.gameObject.SetActive(true);
        }
    }
}

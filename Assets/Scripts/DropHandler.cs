using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TowerController))]
[RequireComponent(typeof(BoxCollider))]
public class DropHandler : MonoBehaviour, IDropHandler
{
    private TowerController tower;
    private BoxCollider boxCollider;
    void Start()
    {
        tower = GetComponent<TowerController>();    
        boxCollider = GetComponent<BoxCollider>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        BlockData block = eventData.pointerDrag.GetComponent<BlockData>();
        if(block)
        {
            int towerHeight = tower.GetTotalValue();
            Vector3 targetPos = new(transform.position.x, boxCollider.bounds.min.y + ((towerHeight + block.value / 2.0f) * block.unitHeight), 0);
            eventData.pointerDrag.transform.DOMove(targetPos, 0.5f);
            tower.AddBlock(block);   
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        BlockData block = eventData.pointerDrag.GetComponent<BlockData>();
        if(block && tower.GetTotalValue() > 0)
        {
            List<BlockData> blocks = tower.stackedBlocks;
            int index = tower.stackedBlocks.IndexOf(block);
            if(index < 0)
            {
                return;  
            } 
            tower.RemoveBlock(block);
    
            // Make the blocks above fall down
            for(int i = index; i < blocks.Count; i++)
            {
                blocks[i].transform.DOMoveY(blocks[i].transform.position.y - (block.value * block.unitHeight), 0.5f);
            }    
            
        }
    }
}

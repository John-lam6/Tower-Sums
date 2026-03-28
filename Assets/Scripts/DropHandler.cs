using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TowerController))]
[RequireComponent(typeof(BoxCollider))]
public class DropHandler : MonoBehaviour, IDropHandler
{
    [SerializeField] private float blockMoveTime = 0.5f;
    public Light hoverLight;
    public bool isAdditionTower = true;
    public float idleDragIntensity = 0.8f;
    public float fullHoverIntensity = 2.0f;
    private TowerController tower;
    private BoxCollider boxCollider;
    void Start()
    {
        tower = GetComponent<TowerController>();    
        boxCollider = GetComponent<BoxCollider>();

        if (hoverLight != null) {
            hoverLight.enabled = false;
        }
    }
    public void SetSoftGlow(bool glowing)
    {
        if (hoverLight == null) return;

        hoverLight.enabled = glowing;

        if (glowing) {
            hoverLight.intensity = idleDragIntensity;
        }
    }

    public void SetHover(BlockData block, bool hovering)
    {
        if (hoverLight != null) {
            hoverLight.enabled = hovering;

            if (hovering) {
                hoverLight.intensity = fullHoverIntensity;
            }
        }

        if (block == null) return;

        if (hovering) {
            block.ApplyTowerScale();

            if (isAdditionTower) block.SetState(BlockState.OnAddition);
            else block.SetState(BlockState.OnSubtraction);
        }
        else {
            if (!block.hasTower) {
                block.SetState(BlockState.InHotbar);
                block.ApplyHotbarScale();
            }
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        BlockData block = eventData.pointerDrag.GetComponent<BlockData>();
        if (block)
        {
            // remove from previous tower if needed
            if (block.hasTower)
            {
                TowerController oldTower = block.transform.parent.GetComponent<TowerController>();
                if (oldTower != null)
                {
                    oldTower.RemoveBlock(block);
                }
            }

            block.hasTower = true;
            block.ApplyTowerScale();

            int towerHeight = tower.GetTotalValue();
            Vector3 targetPos = new Vector3(
                transform.position.x,
                boxCollider.bounds.min.y + ((towerHeight + block.value / 2.0f) * block.unitHeight),
                0
            );

            tower.AddBlock(block);

            if (isAdditionTower) block.SetState(BlockState.OnAddition);
            else block.SetState(BlockState.OnSubtraction);

            block.targetHeight = targetPos.y;
            block.transform.position = targetPos;

            SetHover(block, false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        BlockData block = eventData.pointerDrag.GetComponent<BlockData>();
        if(block && block.hasTower)
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
                blocks[i].targetHeight -= block.value * block.unitHeight;
                blocks[i].transform.DOMoveY(blocks[i].targetHeight, blockMoveTime);
            }
        }
    }
}

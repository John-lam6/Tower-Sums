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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip placeSound;
    public Light hoverLight;
    public bool isAdditionTower = true;
    public float idleDragIntensity = 0.8f;
    public float fullHoverIntensity = 2.0f;

    private TowerController tower;
    private BoxCollider boxCollider;
    private PauseMenuController pauseMenu;
    private GameManager gameManager;

    void Start()
    {
        tower = GetComponent<TowerController>();
        boxCollider = GetComponent<BoxCollider>();
        pauseMenu = GameObject.Find("Pause Menu").GetComponent<PauseMenuController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (hoverLight != null)
        {
            hoverLight.enabled = false;
        }
    }

    public bool CanAcceptDrop()
    {
        return enabled &&
               gameObject.activeInHierarchy &&
               gameObject.layer == LayerMask.NameToLayer("Drop Zone") &&
               boxCollider != null &&
               boxCollider.enabled
               && !pauseMenu.isPaused() && !gameManager.IsSubmitting();
    }

    public void SetDropEnabled(bool value)
    {
        enabled = value;

        Collider[] allColliders = GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < allColliders.Length; i++)
        {
            allColliders[i].enabled = value;
        }

        if (!value && hoverLight != null)
        {
            hoverLight.enabled = false;
        }
    }

    public void SetSoftGlow(bool glowing)
    {
        if (hoverLight == null) return;
        if (!CanAcceptDrop()) return;

        hoverLight.enabled = glowing;

        if (glowing)
        {
            hoverLight.intensity = idleDragIntensity;
        }
    }

    public void SetHover(BlockData block, bool hovering)
    {
        if (!CanAcceptDrop())
        {
            if (hoverLight != null)
            {
                hoverLight.enabled = false;
            }
            return;
        }

        if (hoverLight != null)
        {
            hoverLight.enabled = hovering;

            if (hovering)
            {
                hoverLight.intensity = fullHoverIntensity;
            }
        }

        if (block == null) return;

        if (hovering)
        {
            if (isAdditionTower)
            {
                if (block.state != BlockState.OnAddition)
                {
                    block.SetState(BlockState.OnAddition);
                }
            }
            else
            {
                if (block.state != BlockState.OnSubtraction)
                {
                    block.SetState(BlockState.OnSubtraction);
                }
            }
        }
        else
        {
            if (!block.hasTower)
            {
                block.SetNeutralDragState();
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!CanAcceptDrop())
        {
            return;
        }

        BlockData block = eventData.pointerDrag.GetComponent<BlockData>();
        if (block == null) return;

        block.transform.DOKill();

        if (block.hasTower)
        {
            TowerController oldTower = block.transform.parent != null
                ? block.transform.parent.GetComponent<TowerController>()
                : null;

            if (oldTower != null)
            {
                oldTower.RemoveBlock(block);
            }
        }

        block.hasTower = true;
        block.ApplyTowerScale();

        tower.AddBlock(block);

        int towerHeight = tower.GetTotalValue() - block.value;

        Vector3 targetPos = new Vector3(
            transform.position.x,
            boxCollider.bounds.min.y + ((towerHeight + block.value / 2.0f) * block.unitHeight),
            0
        );

        if (isAdditionTower) block.SetState(BlockState.OnAddition);
        else block.SetState(BlockState.OnSubtraction);

        block.targetHeight = targetPos.y;
        //block.transform.DOMove(targetPos, blockMoveTime).OnComplete(() => audioSource.PlayOneShot(placeSound));
        
        // play at 55% of the tween
        float audioDelay = blockMoveTime * 0.55f; 
        block.transform.DOMove(targetPos, blockMoveTime);
        DOVirtual.DelayedCall(audioDelay, () => audioSource.PlayOneShot(placeSound));

        SetHover(block, false);
    }

   public void OnDrag(PointerEventData eventData)
    {
        if (!CanAcceptDrop())
        {
            return;
        }

        BlockData block = eventData.pointerDrag.GetComponent<BlockData>();
        if (block && block.hasTower)
        {
            int index = tower.stackedBlocks.IndexOf(block);

            if (index < 0)
            {
                return;
            }

            tower.RemoveBlock(block);
            block.hasTower = false;

            for (int i = index; i < tower.stackedBlocks.Count; i++)
            {
                BlockData stackedBlock = tower.stackedBlocks[i];
                stackedBlock.transform.DOKill();
                stackedBlock.targetHeight -= block.value * block.unitHeight;
                stackedBlock.transform.DOMoveY(stackedBlock.targetHeight, blockMoveTime);
            }
        }
    }

    public float GetBlockMoveTime()
    {
        return blockMoveTime;
    }
}
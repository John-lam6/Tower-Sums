using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    private Transform startParent;

    private DropHandler currentDropZone;
    private BlockData blockData;

    public DropHandler additionDropZone;
    public DropHandler subtractionDropZone;
    private PauseMenuController pauseMenu;
    private GameManager gameManager;
    
    void Awake()
    {
        blockData = GetComponent<BlockData>();
        pauseMenu = GameObject.Find("Pause Menu").GetComponent<PauseMenuController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!CanDrag()) return; 
        
        transform.DOKill();

        Vector3 raycastOrigin = Camera.main.WorldToScreenPoint(transform.position);

        if (Physics.Raycast(Camera.main.ScreenPointToRay(raycastOrigin), out RaycastHit hit, 15.0f, LayerMask.GetMask("Drop Zone")))
        {
            DropHandler dropZone = hit.transform.GetComponentInParent<DropHandler>();

            if (dropZone != null && dropZone.CanAcceptDrop())
            {
                dropZone.OnDrag(eventData);
            }
        }

        startPosition = transform.position;
        startParent = transform.parent;

        if (additionDropZone != null && additionDropZone.CanAcceptDrop())
            additionDropZone.SetSoftGlow(true);

        if (subtractionDropZone != null && subtractionDropZone.CanAcceptDrop())
            subtractionDropZone.SetSoftGlow(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!CanDrag()) return; 

        transform.DOKill();

        Vector3 screenPos = new Vector3(
            eventData.position.x,
            eventData.position.y,
            Camera.main.WorldToScreenPoint(transform.position).z
        );

        transform.position = Camera.main.ScreenToWorldPoint(screenPos);

        DropHandler newDropZone = null;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(eventData.position), out RaycastHit hit, 15.0f, LayerMask.GetMask("Drop Zone")))
        {
            newDropZone = hit.transform.GetComponentInParent<DropHandler>();

            if (newDropZone != null && !newDropZone.CanAcceptDrop())
            {
                newDropZone = null;
            }
        }

        if (newDropZone != currentDropZone)
        {
            if (currentDropZone != null)
            {
                if (currentDropZone.CanAcceptDrop())
                {
                    currentDropZone.SetSoftGlow(true);
                    currentDropZone.SetHover(blockData, false);
                }
            }

            currentDropZone = newDropZone;

            if (currentDropZone != null)
            {
                if (additionDropZone != null && additionDropZone != currentDropZone && additionDropZone.CanAcceptDrop())
                    additionDropZone.SetSoftGlow(true);

                if (subtractionDropZone != null && subtractionDropZone != currentDropZone && subtractionDropZone.CanAcceptDrop())
                    subtractionDropZone.SetSoftGlow(true);

                currentDropZone.SetHover(blockData, true);
            }
            else
            {
                if (additionDropZone != null && additionDropZone.CanAcceptDrop())
                    additionDropZone.SetSoftGlow(true);

                if (subtractionDropZone != null && subtractionDropZone.CanAcceptDrop())
                    subtractionDropZone.SetSoftGlow(true);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!CanDrag()) return; 

        if (Physics.Raycast(Camera.main.ScreenPointToRay(eventData.position), out RaycastHit hit, 15.0f, LayerMask.GetMask("Drop Zone")))
        {
            DropHandler dropZone = hit.transform.GetComponentInParent<DropHandler>();

            if (dropZone != null && dropZone.CanAcceptDrop())
            {
                if (additionDropZone != null) additionDropZone.SetSoftGlow(false);
                if (subtractionDropZone != null) subtractionDropZone.SetSoftGlow(false);

                currentDropZone = null;
                dropZone.OnDrop(eventData);
                return;
            }
        }

        if (currentDropZone != null)
        {
            if (currentDropZone.CanAcceptDrop())
            {
                currentDropZone.SetSoftGlow(true);
                currentDropZone.SetHover(blockData, false);
            }

            currentDropZone = null;
        }

        if (additionDropZone != null) additionDropZone.SetSoftGlow(false);
        if (subtractionDropZone != null) subtractionDropZone.SetSoftGlow(false);

        if (blockData != null && !blockData.hasTower)
        {
            transform.DOKill();

            float moveTime = 0.5f;

            if (additionDropZone != null)
            {
                moveTime = additionDropZone.GetBlockMoveTime();
            }
            else if (subtractionDropZone != null)
            {
                moveTime = subtractionDropZone.GetBlockMoveTime();
            }

            transform.DOMove(blockData.hotbarPosition, moveTime)
                .OnComplete(() =>
                {
                    blockData.SetState(BlockState.InHotbar);
                    blockData.ApplyHotbarScale();
                    currentDropZone = null;
                });
        }
    }
    private bool CanDrag()
    {
        return !pauseMenu.isPaused() && !gameManager.IsSubmitting(); 
    }
}
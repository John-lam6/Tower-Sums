using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private Vector3 startPosition;
    private Transform startParent;
    //private RectTransform text_RectTransform;

    private DropHandler currentDropZone;
    private BlockData blockData;

    public DropHandler additionDropZone;
    public DropHandler subtractionDropZone;

    void Awake() {
        blockData = GetComponent<BlockData>();
    }

    public void OnBeginDrag(PointerEventData eventData) {

        Vector3 raycastOrigin = Camera.main.WorldToScreenPoint(transform.position);
        if (Physics.Raycast(Camera.main.ScreenPointToRay(raycastOrigin), out RaycastHit hit, 15.0f, LayerMask.GetMask("Drop Zone")))
        {
            DropHandler dropZone = hit.transform.gameObject.GetComponent<DropHandler>();
            if (dropZone)
            {
                dropZone.OnDrag(eventData);
            }
        }
        
        startPosition = transform.position;
        startParent = transform.parent;

        if (additionDropZone != null) additionDropZone.SetSoftGlow(true);
        if (subtractionDropZone != null) subtractionDropZone.SetSoftGlow(true);
        //text_RectTransform = transform.GetComponent<RectTransform>();
    }

    // public void OnDrag(PointerEventData eventData) {
    //     Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, Camera.main.WorldToScreenPoint(transform.position).z);
    //     transform.position = Camera.main.ScreenToWorldPoint(screenPos);
    //     //text_RectTransform.anchoredPosition = Camera.main.ScreenToWorldPoint(screenPos);
    // }

    public void OnDrag(PointerEventData eventData) {
        Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, Camera.main.WorldToScreenPoint(transform.position).z);
        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
        //text_RectTransform.anchoredPosition = Camera.main.ScreenToWorldPoint(screenPos);

        DropHandler newDropZone = null;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(eventData.position), out RaycastHit hit, 15.0f, LayerMask.GetMask("Drop Zone")))
        {
            newDropZone = hit.transform.gameObject.GetComponent<DropHandler>();
        }

        if (newDropZone != currentDropZone)
        {
            if (currentDropZone != null)
            {
                currentDropZone.SetSoftGlow(true);
                currentDropZone.SetHover(blockData, false);
            }

            currentDropZone = newDropZone;

            if (currentDropZone != null)
            {
                if (additionDropZone != null && additionDropZone != currentDropZone)
                    additionDropZone.SetSoftGlow(true);

                if (subtractionDropZone != null && subtractionDropZone != currentDropZone)
                    subtractionDropZone.SetSoftGlow(true);

                currentDropZone.SetHover(blockData, true);
            }
            else
            {
                if (additionDropZone != null) additionDropZone.SetSoftGlow(true);
                if (subtractionDropZone != null) subtractionDropZone.SetSoftGlow(true);

                if (blockData != null && !blockData.hasTower)
                {
                    blockData.SetState(BlockState.InHotbar);
                    blockData.ApplyHotbarScale();
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // check what we dropped on
        if (Physics.Raycast(Camera.main.ScreenPointToRay(eventData.position), out RaycastHit hit, 15.0f, LayerMask.GetMask("Drop Zone")))
        {
            DropHandler dropZone = hit.transform.GetComponentInParent<DropHandler>();

            if (dropZone != null && dropZone.enabled)
            {
                if (additionDropZone != null) additionDropZone.SetSoftGlow(false);
                if (subtractionDropZone != null) subtractionDropZone.SetSoftGlow(false);

                currentDropZone = null;
                dropZone.OnDrop(eventData);
                return;
            }
        }

        // only now clear hover visuals
        if (currentDropZone != null)
        {
            currentDropZone.SetSoftGlow(true);
            currentDropZone.SetHover(blockData, false);
            currentDropZone = null;
        }

        if (additionDropZone != null) additionDropZone.SetSoftGlow(false);
        if (subtractionDropZone != null) subtractionDropZone.SetSoftGlow(false);

        // failed drop, return to hotbar
        if (blockData != null && !blockData.hasTower)
        {
            transform.parent = startParent;

            blockData.SetState(BlockState.InHotbar);
            blockData.ApplyHotbarScale();

            startPosition = transform.position;
            startParent = transform.parent;
            currentDropZone = null;
        }
    }
}

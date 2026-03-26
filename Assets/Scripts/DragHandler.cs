using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private Vector3 startPosition;
    private Transform startParent;
    //private RectTransform text_RectTransform;

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
        //text_RectTransform = transform.GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData) {
        Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, Camera.main.WorldToScreenPoint(transform.position).z);
        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
        //text_RectTransform.anchoredPosition = Camera.main.ScreenToWorldPoint(screenPos);
    }

    public void OnEndDrag(PointerEventData eventData) {
        Vector3 raycastOrigin = Camera.main.WorldToScreenPoint(transform.position);
        if (Physics.Raycast(Camera.main.ScreenPointToRay(raycastOrigin), out RaycastHit hit, 15.0f, LayerMask.GetMask("Drop Zone")))
        {
            DropHandler dropZone = hit.transform.gameObject.GetComponent<DropHandler>();
            if (dropZone)
            {
                dropZone.OnDrop(eventData);
            }
        }
    }
}

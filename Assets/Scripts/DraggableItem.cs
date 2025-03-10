using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 originalPosition;

    public string itemID;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;
        transform.SetParent(originalParent.root); 
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Check if dropped in Combo Area
        if (IsPointerOverUIElement("ComboArea"))
        {
            transform.SetParent(GameObject.Find("ComboArea").transform, true);

            // Check for combination logic
            InventoryManager.instance.CheckCombination(this);
        }
        // If dropped outside Inventory & ComboArea -> Return to original position
        else if (!IsPointerOverUIElement("InventoryGrid"))
        {
            transform.position = originalPosition;
            transform.SetParent(originalParent);
        }
    }

    private bool IsPointerOverUIElement(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
            {
                return true;
            }
        }
        return false;
    }
}

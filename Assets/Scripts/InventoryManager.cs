using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

[System.Serializable] 
public class EvidenceSprite
{
    public string evidenceName; // should match GameObject of evidence name
    public Sprite evidenceSprite; // corresponding UI sprite
}


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public Transform itemGridContent;
    public GameObject inventoryItemPrefab;
    public GameObject comboResultPrefab;
    public List<EvidenceSprite> evidenceSpriteList = new List<EvidenceSprite>(); 
    public Dictionary<string, Sprite> evidenceSprites = new Dictionary<string, Sprite>(); 

    private List<string> pickedEvidences = new List<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (var entry in evidenceSpriteList)
        {
            evidenceSprites[entry.evidenceName] = entry.evidenceSprite;
        }
    }

    public void AddEvidenceToInventory(string evidenceName, string itemID = "")
    {
        if (!pickedEvidences.Contains(evidenceName))
        {
            pickedEvidences.Add(evidenceName);

            if (evidenceSprites.ContainsKey(evidenceName))
            {
                GameObject newItem = Instantiate(inventoryItemPrefab, itemGridContent);
                newItem.GetComponent<Image>().sprite = evidenceSprites[evidenceName];

                DraggableItem draggable = newItem.AddComponent<DraggableItem>();
                draggable.itemID = string.IsNullOrEmpty(itemID) ? evidenceName : itemID;
            }
        }
    }

    public void CheckCombination(DraggableItem droppedItem)
    {
        List<DraggableItem> itemsInComboArea = new List<DraggableItem>();

        // Find all items currently in Combo Area
        foreach (Transform child in GameObject.Find("ComboArea").transform)
        {
            DraggableItem item = child.GetComponent<DraggableItem>();
            if (item != null)
            {
                itemsInComboArea.Add(item);
            }
        }

        // Check for a matching pair
        DraggableItem matchedItem = null;
        foreach (DraggableItem item in itemsInComboArea)
        {
            if (item != droppedItem && item.itemID == droppedItem.itemID)
            {
                matchedItem = item;
                break;
            }
        }

        if (matchedItem != null)
        {
            // Destroy both original items
            Destroy(droppedItem.gameObject);
            Destroy(matchedItem.gameObject);

            // Instantiate the result item in Combo Area
            GameObject newItem = Instantiate(inventoryItemPrefab, GameObject.Find("ComboArea").transform);
            newItem.GetComponent<Image>().sprite = GetComboResultSprite(droppedItem.itemID);
            newItem.AddComponent<DraggableItem>(); // Make it draggable
        }
    }

    private Sprite GetComboResultSprite(string itemID)
    {
        Dictionary<string, Sprite> comboResults = new Dictionary<string, Sprite>
    {
        { "itemA", Resources.Load<Sprite>("ComboResult1") },
        { "itemB", Resources.Load<Sprite>("ComboResult2") }
    };

        return comboResults.ContainsKey(itemID) ? comboResults[itemID] : null;
    }


    public void ClearInventory()
    {
        foreach (Transform child in itemGridContent)
        {
            Destroy(child.gameObject);
        }
        pickedEvidences.Clear();
    }
}

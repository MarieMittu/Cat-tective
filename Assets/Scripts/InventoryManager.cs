using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    public void AddEvidenceToInventory(string evidenceName)
    {
        if (!pickedEvidences.Contains(evidenceName))
        {
            pickedEvidences.Add(evidenceName);

            if (evidenceSprites.ContainsKey(evidenceName))
            {
                GameObject newItem = Instantiate(inventoryItemPrefab, itemGridContent);
                newItem.GetComponent<Image>().sprite = evidenceSprites[evidenceName];

                newItem.AddComponent<DraggableItem>();
            }
        }
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

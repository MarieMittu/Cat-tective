using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AreaActions : MonoBehaviour
{
    public Camera subCamera;
    public Camera catCamera;
    public GameObject catsBody;
    public GameObject closeBtn;
    [HideInInspector] public bool isDisplayed;
    [HideInInspector] public bool pickedEvidence;

    public GameObject[] evidences;
    private GameObject currentEvidence;

    // Start is called before the first frame update
    void Start()
    {
        DeactivateView();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisplayed) 
        {
            DetectMouseOverEvidence();
        }
    }

    private void DetectMouseOverEvidence()
    {
        Ray ray = subCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            foreach (GameObject evidence in evidences)
            {
                if (hit.collider.gameObject == evidence)
                {
                    HandleEvidenceHover(evidence);
                    return; 
                }
            }
        }

        if (currentEvidence != null)
        {
            ResetEvidence(currentEvidence);
            currentEvidence = null;
        }
    }

    private void HandleEvidenceHover(GameObject evidence)
    {
        if (currentEvidence != evidence)
        {
            if (currentEvidence != null)
            {
                ResetEvidence(currentEvidence);
            }

            if (evidence.transform.childCount > 0)
            {
                evidence.transform.GetChild(0).gameObject.SetActive(true);
            }

            currentEvidence = evidence;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (evidence.transform.childCount > 1)
            {
                evidence.transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            evidence.SetActive(false);
            pickedEvidence = true;

            DraggableItem draggable = evidence.GetComponent<DraggableItem>();
            string itemID = draggable != null ? draggable.itemID : evidence.name;

            InventoryManager.instance?.AddEvidenceToInventory(evidence.name, itemID);
        }
    }
    

    private void ResetEvidence(GameObject evidence)
    {
        foreach (Transform child in evidence.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ActivateView()
    {
        isDisplayed = true;
        subCamera.enabled = true;
        catCamera.enabled = false;
        catsBody.SetActive(false);
        closeBtn.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        ActivateCloseButton(DeactivateView);
    }

    public void DeactivateView()
    {
        isDisplayed = false;
        subCamera.enabled = false;
        catCamera.enabled = true;
        catsBody.SetActive(true);
        closeBtn.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }


    void ActivateCloseButton(Action method)
    {
        if (closeBtn != null)
        {
            var button = closeBtn.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {

                button.onClick.AddListener(new UnityEngine.Events.UnityAction(method));


            }
        }
    }
}

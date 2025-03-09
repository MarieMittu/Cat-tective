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

    public GameObject[] evidences;

    // Start is called before the first frame update
    void Start()
    {
        DeactivateView();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        foreach (GameObject evidence in evidences)
        {
            if (evidence.transform.childCount > 0)
            {
                evidence.transform.GetChild(0).gameObject.SetActive(true); 
            }
        }
    }

    private void OnMouseExit()
    {
        foreach (GameObject evidence in evidences)
        {
            if (evidence.transform.childCount > 0)
            {
                evidence.transform.GetChild(0).gameObject.SetActive(false);
            }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager sharedInstance;

    public float standardDelay = 2f;
    public float chilldDelay = 5f;
    public float shortDelay = 0.5f;

    public GameObject[] tutorials;
    public GameObject climbMarker;
    public GameObject sunMarker;
    public GameObject sunTrigger;
    public GameObject playMarker;
    public GameObject ball;
    public GameObject snakeMarker;
    public GameObject cucumber;
    public GameObject investigationMarker;
    public GameObject investigationArea;
    public GameObject photoPart;
    public GameObject closetMarker;
    public GameObject closetTrigger;

    [HideInInspector] public int currentIndex = 0;
    [HideInInspector] public bool startClimbing = false;
    [HideInInspector] public bool startChilling = false;
    [HideInInspector] public bool startPlaying = false;
    [HideInInspector] public bool isScared = false;
    [HideInInspector] public bool isSearching = false;
    [HideInInspector] public bool foundWatch = false;

    private bool isSwitching = false;

    private void Awake()
    {

        sharedInstance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        ActivateTutorial(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            ShowNextTutorial(1, standardDelay);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ShowNextTutorial(2, standardDelay);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftCommand))
        {
            ShowNextTutorial(3, standardDelay);
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentIndex == 3)
        {
            ShowNextTutorial(4, standardDelay);
            climbMarker.SetActive(true);
        }

        if (startClimbing && currentIndex == 4)
        {
            ShowNextTutorial(5, standardDelay);
            climbMarker.SetActive(false);
            sunMarker.SetActive(true);
            sunTrigger.SetActive(true);
        }

        if (startChilling)
        {
            if (currentIndex == 5)
            {
                ShowNextTutorial(6, standardDelay);
                sunMarker.SetActive(false);
            } else if (currentIndex == 6)
            {
                ShowNextTutorial(7, chilldDelay);
            }
        } else
        {
            if (currentIndex == 7)
            {
                ShowNextTutorial(8, standardDelay);
                playMarker.SetActive(true);
                ball.SetActive(true);
            }
        }

        if (startPlaying)
        {
            if (currentIndex == 8)
            {
                ShowNextTutorial(9, standardDelay);
                playMarker.SetActive(false);
            }
            else if (currentIndex == 9)
            {
                ShowNextTutorial(10, chilldDelay);
            }
        } else
        {
            if (currentIndex == 10)
            {
                ShowNextTutorial(11, standardDelay);
                snakeMarker.SetActive(true);
                cucumber.SetActive(true);
            }
        }

        if (isScared)
        {
            if (currentIndex == 11)
            {
                ShowNextTutorial(12, shortDelay);
                snakeMarker.SetActive(false);
            }
        } else
        {
            if (currentIndex == 12)
            {
                ShowNextTutorial(13, standardDelay);
            }
        }

        AreaActions areaScript = investigationArea.GetComponent<AreaActions>();

        if (currentIndex == 13)
        {
            ShowNextTutorial(14, standardDelay);
            investigationMarker.SetActive(true); //add to the marker img text press E
            investigationArea.SetActive(true);
            photoPart.SetActive(true);
            areaScript.closeBtn.SetActive(false);
        }

        if (currentIndex == 14 && isSearching) // change condition to solving puzzle
        {
            //areaScript.closeBtn.SetActive(true); //for later tutorial
            ShowNextTutorial(15, standardDelay);
        }

        if (currentIndex == 15 && !areaScript.isDisplayed)
        {
            ShowNextTutorial(16, standardDelay);
            closetMarker.SetActive(true);
            closetTrigger.SetActive(true);
        }

        if (foundWatch)
        {
            FindObjectOfType<ScenesController>().ShowTimeTravelScene();
        }
    }

    private void ActivateTutorial(int index)
    {
        if (index < 0 || index >= tutorials.Length) return;

        foreach (var tutorial in tutorials)
        {
            tutorial.SetActive(false);
        }

        tutorials[index].SetActive(true);
        currentIndex = index;
        isSwitching = false;
    }

    private void ShowNextTutorial(int index, float delay)
    {
        if (currentIndex == index - 1 && !isSwitching)
        {
            StartCoroutine(PrepareSwitch(index, delay));
        }
    }

    private IEnumerator PrepareSwitch(int index, float delay)
    {
        isSwitching = true;
        yield return new WaitForSeconds(delay);
        ActivateTutorial(index);
    }
}

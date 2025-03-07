using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager sharedInstance;

    public GameObject[] tutorials;

    [HideInInspector]
    public int currentIndex = 0;

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
            ShowNextTutorial(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ShowNextTutorial(2);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftCommand))
        {
            ShowNextTutorial(3);
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentIndex == 3)
        {
            ShowNextTutorial(3);
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

    private void ShowNextTutorial(int index)
    {
        if (currentIndex == index - 1 && !isSwitching)
        {
            StartCoroutine(PrepareSwitch(index, 2f));
        }
    }

    private IEnumerator PrepareSwitch(int index, float delay)
    {
        isSwitching = true;
        yield return new WaitForSeconds(delay);
        ActivateTutorial(index);
    }
}

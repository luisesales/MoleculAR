using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialCanvasSteps : MonoBehaviour
{
    [SerializeField] private List<GameObject> tutorialParts;
    [SerializeField] private GameObject foward;
    [SerializeField] private GameObject backwards;
    [SerializeField] private GameObject conclude;
    [SerializeField] private GameObject background;
    private GameObject currentStep;

    private int currentStepIndex;
    private int currentPart;

    private void Start()
    {
        currentStepIndex = 0;
        currentPart = 0;           
        currentStep = tutorialParts[currentPart].transform.GetChild(currentStepIndex).gameObject;             
    }
    public void NextStep()
    {
        currentStep.SetActive(false);
        currentStepIndex = (currentStepIndex + 1) % tutorialParts[currentPart].transform.childCount;
        currentStep = tutorialParts[currentPart].transform.GetChild(currentStepIndex).gameObject;
        currentStep.SetActive(true);
        Debug.Log("" + currentStep);
        if(currentStepIndex == tutorialParts[currentPart].transform.childCount - 1)
            conclude.SetActive(true);
        backwards.SetActive(true);
    }

    public void PreviousStep()
    {
        currentStep.SetActive(false);
        currentStepIndex = (currentStepIndex - 1 + tutorialParts[currentPart].transform.childCount) % tutorialParts[currentPart].transform.childCount;
        currentStep = tutorialParts[currentPart].transform.GetChild(currentStepIndex).gameObject;
        currentStep.SetActive(true);
        if(currentStepIndex == 0)
            backwards.SetActive(false);
        conclude.SetActive(false);        
    }

    public void Conclude()
    {
        foward.SetActive(false);
        backwards.SetActive(false);
        conclude.SetActive(false);
        background.SetActive(false);
        tutorialParts[currentPart].SetActive(false);
        currentPart++;
    }

    public void NextPart()
    {
        foward.SetActive(true);
        backwards.SetActive(true);        
        background.SetActive(true);
        tutorialParts[currentPart].SetActive(true);
    }
}


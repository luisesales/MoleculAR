using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

[System.Serializable] 
public struct ModelInfoReferences
{
    public TMP_Text name;
    public TMP_Text atomicNumber;
    public TMP_Text molecularFormula;
    public TMP_Text applications;

    public Toggle primario;
    public Toggle secundario;
    public Toggle terciario;
    public Toggle quaternario;
}


public class DetailedViewController : MonoBehaviour     
{
    [SerializeField] private float zoomSensitivity = 0.001f; // Zoom Sensitivity for Touch Input
    [SerializeField] private float maxZoomIn = 10f;         // Max Zoom Distance in Meters
    [SerializeField] private float maxZoomOut = 0f;         // Min Zoom Distance in Meters

    [SerializeField] private RectTransform lateralPannel; // Reference to the lateral panel RectTransform
    [SerializeField] private GameObject closeButton; // Reference to the close the scene
    [SerializeField] private GameObject resetButton; // Reference to the reset the scene

    [SerializeField] private GameObject infoCloseButton; // Reference to the close the info panel
    [SerializeField] private GameObject infoOpenButton; // Reference to the open the info panel

    [SerializeField] private ModelInfoReferences modelInfoReferences; // Reference to the model info UI elements


    private ModelController modelController; // Reference to the ModelController script    
    private GameObject mainCamera;
    private Vector3 startingPosition;
    private float currentDistance = 0f;
    void Start()
    {
        modelController = GameController.Instance.selectedModel.GetComponent<ModelController>();
        mainCamera = Camera.main.gameObject;
        currentDistance = mainCamera.transform.position.z;
        startingPosition = mainCamera.transform.position;
        maxZoomIn = modelController.modelData.maxZoomIn;
        maxZoomOut = modelController.modelData.maxZoomOut;        
        modelInfoReferences.name.text = modelController.modelData.name;
        modelInfoReferences.atomicNumber.text = modelController.modelData.atomicNumber;
        modelInfoReferences.molecularFormula.text = modelController.modelData.molecularFormula;
        modelInfoReferences.applications.text = modelController.modelData.applications;
        modelInfoReferences.primario.SetIsOnWithoutNotify(modelController.modelData.structuralLevel.primario);
        modelInfoReferences.secundario.SetIsOnWithoutNotify(modelController.modelData.structuralLevel.secundario);
        modelInfoReferences.terciario.SetIsOnWithoutNotify(modelController.modelData.structuralLevel.terciario);
        modelInfoReferences.quaternario.SetIsOnWithoutNotify(modelController.modelData.structuralLevel.quaternario);
    }

    void Update()
    {
        //Mobile
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            
            float difference = currentMagnitude - prevMagnitude;
                        
            currentDistance = Mathf.Clamp(currentDistance + (difference * zoomSensitivity), currentDistance -maxZoomOut, currentDistance + maxZoomIn);
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, currentDistance);
        }



        //PC
        // float scroll = Input.GetAxis("Mouse ScrollWheel");
        // Debug.Log(scroll);        
        // currentDistance = currentDistance - scroll * zoomSensitivity; 
        // Debug.Log("Current Distance : "+ currentDistance);        
        
        // mainCamera.transform.position = startingPosition + (mainCamera.transform.forward * currentDistance); 
    }

    public void Return()
    {
        GameController.Instance.UndetailModel();
    }

    public void Reset()
    {
        GameController.Instance.ResetModel();
        mainCamera.transform.position = startingPosition;
    }

    public void OpenCloseTooltip()
    {
        GameController.Instance.OpenCloseTooltip();        
    }

    
    public void OpenInfo()
    {        
        lateralPannel.DOAnchorPosX(446f, 0.5f).SetEase(Ease.OutCubic);
        closeButton.SetActive(false);
        resetButton.SetActive(false);
        infoCloseButton.SetActive(true);
        infoOpenButton.SetActive(false);
    }
    
    public void CloseInfo()
    {        
        lateralPannel.DOAnchorPosX(1500f, 0.5f).SetEase(Ease.InCubic);
        closeButton.SetActive(true);
        resetButton.SetActive(true);
        infoCloseButton.SetActive(false);
        infoOpenButton.SetActive(true);
    }
}

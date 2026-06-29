using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailedViewController : MonoBehaviour     
{
    public float zoomSensitivity = 0.01f; // Zoom Sensitivity for Touch Input
    public float maxZoomIn = 10f;         // Max Zoom Distance in Meters
    public float maxZoomOut = 0f;         // Min Zoom Distance in Meters
    
    private GameObject mainCamera;
    private Vector3 startingPosition;
    private float currentDistance = 0f;
    public void Start()
    {
        mainCamera = Camera.main.gameObject;
        startingPosition = mainCamera.transform.position;
        maxZoomIn = GameController.Instance.selectedModel.GetComponent<ModelController>().modelData.maxZoomIn;
        maxZoomOut = GameController.Instance.selectedModel.GetComponent<ModelController>().modelData.maxZoomOut;
    }

    public void Update()
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
                        
            currentDistance = Mathf.Clamp(currentDistance + (difference * zoomSensitivity), -maxZoomOut, maxZoomIn);
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, Mathf.Clamp(currentDistance + (mainCamera.transform.position.z * currentDistance), -maxZoomOut, maxZoomIn) * zoomSensitivity);
        }



        //PC
        // float scroll = Input.GetAxis("Mouse ScrollWheel");
        // Debug.Log(scroll);        
        // float currentDistance = currentDistance - scroll * zoomSensitivity; 
        // Debug.Log("Current Distance : "+ currentDistance);
        // Debug.Log("New Distance : "+ newDistance);
        
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
}

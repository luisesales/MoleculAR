using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModelController : MonoBehaviour
{

    public Model modelData;
    public bool isActive;
    public float rotationSpeed;

    public bool check = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // PC
        if (Input.GetMouseButton(0))
        {
            // Verifica se o toque está sobre a UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Clique na UI detectado. Ignorando interação 3D.");
                return;
            }
            else
            {
                Quaternion rotationX = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * rotationSpeed, Vector3.right);
                Quaternion rotationZ = Quaternion.AngleAxis(-Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);
                transform.rotation = rotationZ * rotationX * transform.rotation;        
                isActive = true;
                if (check)
                {
                    check = false;
                    GameController.Instance.SelectModel(gameObject);                    
                }
            }
            
        }
        if (Input.GetMouseButtonUp(0) && isActive)
        {        
            isActive = false;
            check = true;
            GameController.Instance.OutlineObject();
        }

        // Mobile
        // if (isActive)
        // {            
        //     if (Input.touchCount == 1)
        //     {
        //         Touch screenTouch = Input.GetTouch(0);
        //         if (screenTouch.phase == TouchPhase.Moved)
        //         {        
        //             Quaternion rotationX = Quaternion.AngleAxis(screenTouch.deltaPosition.y * rotationSpeed, Vector3.right);
        //             Quaternion rotationZ = Quaternion.AngleAxis(-screenTouch.deltaPosition.x * rotationSpeed, Vector3.up);
        //             transform.rotation = rotationZ * rotationX * transform.rotation;
        //         }

        //         if (screenTouch.phase == TouchPhase.Ended)
        //         {
        //             isActive = false;
        //             check = true;                    
        //             GameController.Instance.OutlineObject();
        //         }
        //     }            
        // }       
    }
}

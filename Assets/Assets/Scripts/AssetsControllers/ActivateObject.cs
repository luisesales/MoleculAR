using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActivateObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);

            RaycastHit hit;

            // Verifica se o toque está sobre a UI
            if (!IsPointerOverUIObject(Input.touches[0]))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "TargetModel")
                    {
                        ModelController script = hit.collider.GetComponent<ModelController>();
                        script.isActive = !script.isActive;
                        if (script.check)
                        {
                            GameController.Instance.SelectModel(hit.transform.gameObject);
                            script.check = false;
                        }
                    }
                }
            }
        }
    }

    private bool IsPointerOverUIObject(Touch touch)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(touch.position.x, touch.position.y);
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        
        // Se a lista for maior que 0, significa que o dedo bateu em algo da UI.
        return results.Count > 0; 
    }
}

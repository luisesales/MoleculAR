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
            if (!EventSystem.current.IsPointerOverGameObject())
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
}

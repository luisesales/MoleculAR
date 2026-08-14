using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogController : MonoBehaviour
{
    public GameObject UI;
    private bool IsFiltersOpen = false;
    private bool IsFiltersActive = false;

    private GameObject FiltersPanel;



    // Start is called before the first frame update
    void Start()
    {
        FiltersPanel = UI.transform.Find("FiltersPanel").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OpenCloseFilters(){
        FiltersPanel.SetActive(!FiltersPanel.activeSelf);
    }
}

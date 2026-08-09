using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogController : MonoBehaviour
{
    private bool IsFiltersOpen = false;
    private bool IsFiltersActive = false;

    public GameObject FiltersPanel;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OpenCloseFilters(){
        FiltersPanel.SetActive(!FiltersPanel.activeSelf);
    }
}

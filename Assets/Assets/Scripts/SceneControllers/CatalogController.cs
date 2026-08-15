using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum ZoomType : short
{
    ZoomIn = 1,
    ZoomOut = -1,
}

[System.Serializable]
public enum FilterType{
    MolecularType,
    MolecularStructure
}

[System.Serializable]
public struct Filter
{
    public string Name;
    public bool IsActive;
    public FilterType FilterType;
}
[System.Serializable]
public struct UICardPositions
{
    public Transform Position;
    public bool IsOccupied { get; private set; }
}
[System.Serializable]
public struct UIModel
{
    public GameObject SearchText;
    public GameObject FiltersPannel;
    public List<Filter> Filters;
    public List<UICardPositions> CardPositions;


}

public class CatalogController : MonoBehaviour
{
    private bool IsFiltersOpen = false;
    private bool IsFiltersActive = false;
    
    public UIModel UIModel;

    public float ZoomDistanceJump = 5f;



    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OpenCloseFilters(){
        UIModel.FiltersPannel.SetActive(!UIModel.FiltersPannel.activeSelf);
    }

    public void Search(string searchText){
        Debug.Log("Searching for: " + searchText);
    }

    public void ApplyFilters(){
        Debug.Log("Applying Filters");
    }

    public void ClearFilters(){
        Debug.Log("Clearing Filters");
        for(int i = 0; i < UIModel.Filters.Count; i++)
        {            
            Filter tempFilter = UIModel.Filters[i];         
            tempFilter.IsActive = false;            
            UIModel.Filters[i] = tempFilter;
        }
    }

    public void SelectModel()
    {
        
    }

    public void DetailModel()
    {
        
    }

    public void Zoom(ZoomType type)
    {
        // Distance += type*ZoomDistanceJump;
    }
}

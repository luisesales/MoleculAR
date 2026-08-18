using System.Numerics;
using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
public struct UIModel
{
    public GameObject SearchText;
    public GameObject FiltersPannel;

    public GameObject SimulationControls;

    public GameObject CardsSpawner;

    public GameObject CardPrefab;
    public List<Filter> Filters;
    
    public Transform ModelSpawnerPosition;


}

public class CatalogController : MonoBehaviour
{
    private bool IsFiltersActive = false;
    
    public UIModel UIModel;

    public float ZoomDistanceJump = 5f;



    // Start is called before the first frame update
    void Start()
    {
        InstantiateCards();
        UIModel.FiltersPannel.SetActive(false);
        UIModel.SimulationControls.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InstantiateCards()
    {
        int i = 0;
        foreach (GameObject model in GameController.Instance.prefabs.Values)
        {            
            GameObject card = Instantiate(UIModel.CardPrefab,UIModel.CardsSpawner.transform,false);
            card.GetComponent<Button>().onClick.AddListener(() => SelectModel(model));
            card.GetComponent<CardController>().SetupCard(model);            
            i++;
        }
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

    public void SelectModel(GameObject model)
    {
        GameController.Instance.VisualizeModel(model,UIModel.ModelSpawnerPosition);
        if(UIModel.SimulationControls.activeSelf) return;        
        UIModel.SimulationControls.SetActive(true);        
    }

    public void DetailModel()
    {
        GameController.Instance.DetailModel();
    }

    public void Zoom(ZoomType type)
    {
        // Distance += type*ZoomDistanceJump;
    }
}

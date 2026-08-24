using System.ComponentModel;
using System;
using System.Numerics;
using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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
    public Toggle toggle;
    public FilterType FilterType;

}
[System.Serializable]
public struct FilterGroup
{
    public List<Filter> Filters;
    public TMP_Text SelectedFiltersCountText;
    public int SelectedFiltersCount;

}
[System.Serializable]
public struct UIModel
{
    public ScrollRect CatalogScrollRect;
    public TMP_InputField SearchText;
    public GameObject FiltersPannel;

    public GameObject SimulationControls;

    public Transform CardsSpawner;

    public GameObject CardPrefab;
    
    public FilterGroup FilterGroup;
    
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


    private void InstantiateCards()
    {
        GameController.Instance.InitiatePrefabsDictionary();
        foreach (GameObject model in GameController.Instance.prefabs.Values)
        {            
            GameObject card = Instantiate(UIModel.CardPrefab,UIModel.CardsSpawner,false);
            card.GetComponent<Button>().onClick.AddListener(() => SelectModel(model));
            card.GetComponent<CardController>().SetupCard(model);            
        }
        Canvas.ForceUpdateCanvases();    
        UIModel.CatalogScrollRect.verticalNormalizedPosition = 1f;        
    }

    private void SetFiltersCount(int count)
    {
        UIModel.FilterGroup.SelectedFiltersCount = count;
        UIModel.FilterGroup.SelectedFiltersCountText.text = count.ToString();
    }


    public void OpenCloseFilters(){
        UIModel.FiltersPannel.SetActive(!UIModel.FiltersPannel.activeSelf);
    }

    public void Search(){
        if (string.IsNullOrEmpty(UIModel.SearchText.text))
        {
            for (int i = 0; i < UIModel.CardsSpawner.childCount; i++)
            {
                UIModel.CardsSpawner.GetChild(i).gameObject.SetActive(true);
            }
        }        
        Debug.Log("Searching for: " + UIModel.SearchText.text);
        string lowerText = UIModel.SearchText.text.ToLower();
        for (int i = 0; i < UIModel.CardsSpawner.childCount; i++)
        {
            Transform child = UIModel.CardsSpawner.GetChild(i);
            CardController card = child.GetComponent<CardController>();
            if (card != null)
            {
                if (card.approvedInFilters == false)
                {
                    child.gameObject.SetActive(false);
                    continue; 
                }
                if (card.model.scientificalName.Contains(lowerText)){
                    child.gameObject.SetActive(true);
                    continue;
                }
                child.gameObject.SetActive(false);
            }
        }
    }

    public void ApplyFilters(){
        Debug.Log("Applying Filters");
        
        bool filterPrimary = UIModel.FilterGroup.Filters[0].toggle.isOn;
        bool filterSecondary = UIModel.FilterGroup.Filters[1].toggle.isOn;
        bool filterTerciary = UIModel.FilterGroup.Filters[2].toggle.isOn;
        bool filterQuaternary = UIModel.FilterGroup.Filters[3].toggle.isOn;
        bool filterAminoacid = UIModel.FilterGroup.Filters[4].toggle.isOn;
        bool filterProtein = UIModel.FilterGroup.Filters[5].toggle.isOn;        
        
        // Se nenhum filtro estiver marcado, mostramos tudo (regra comum de UX)
        bool filtersOff = !(filterPrimary || filterSecondary || filterTerciary || filterQuaternary || filterAminoacid || filterProtein);
        
        for (int i = 0; i < UIModel.CardsSpawner.childCount; i++)
        {
            Transform child = UIModel.CardsSpawner.GetChild(i);
            CardController card = child.GetComponent<CardController>();
            
            if (card != null && card.model != null)
            {
                // Se nenhum filtro estiver ativo, liga o card e vai para o próximo
                if (filtersOff)
                {
                    child.gameObject.SetActive(true);
                    continue;
                }

                // Acessamos os dados da proteína diretamente
                var level = card.model.structuralLevel;

                // Fazemos o match (compara as variáveis em memória, o que é instantâneo)
                bool showCard = false;

                if (filterPrimary && level.primario) showCard = true;
                if (filterSecondary && level.secundario) showCard = true;
                if (filterTerciary && level.terciario) showCard = true;
                if (filterQuaternary && level.quaternario) showCard = true;

                // Liga ou desliga baseado no resultado
                card.approvedInFilters = showCard;
                child.gameObject.SetActive(showCard);
            }
        }
    }
    public void ClearFilters(){
        Debug.Log("Clearing Filters");
        foreach(Filter filter in UIModel.FilterGroup.Filters)        
        {                  
            filter.toggle.SetIsOnWithoutNotify(false);            
        }
        ApplyFilters();
        SetFiltersCount(0);
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

    public void ToggleFilter(bool value)
    {
        Debug.Log("Toggling a Filter");
        if(value) 
        {
            Debug.Log("Increasing FilterCount Value");
            SetFiltersCount(++UIModel.FilterGroup.SelectedFiltersCount);
            return;
        }
        Debug.Log("Decreasing FilterCount Value");
        SetFiltersCount(--UIModel.FilterGroup.SelectedFiltersCount);
    }

    public void ReturnMenu()
    {
        GameController.Instance.ReturnMenu();
    }
}

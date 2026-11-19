using System.Net.Mime;
using System;
using UnityEngine;


public enum elementType
{
    proteina,
    aminoacido,
}
[System.Serializable]
    public struct structuralLevel
    {
        public structuralLevel(bool primario, bool secundario, bool terciario, bool quaternario, elementType type)
        {
            this.primario = primario;
            this.secundario = secundario;
            this.terciario = terciario;
            this.quaternario = quaternario;
            this.type = type;
        }

        public elementType type;
        public bool primario;
        public bool secundario;
        public bool terciario;
        public bool quaternario;

        
        public string GetElementType() => type.GetElementTypeText();

        public void ClearLevels()
        {
            primario = secundario = terciario = quaternario = false;
        }
    }

public static class staticMethods
{
    public static string GetElementTypeText(this elementType type)
    {
        return type switch
        {
            elementType.proteina => "Proteína",
            elementType.aminoacido => "Aminoácido",
            _ => "Desconhecido"
        };
    }
}
[CreateAssetMenu(fileName = "model", menuName = "Model/Create New Model")]
public class Model : ScriptableObject
{
    private string _id;

    public string id => _id;
    public string name = "Não Inserido";

    public string extendedName = "Não Inserido";
    public string scientificalName = "Não Inserido";
    public string atomicNumber = "Não Inserido";
    public string molecularFormula = "Não Inserida";
    public string applications = "Nenhuma";   
    public structuralLevel structuralLevel = new structuralLevel(true, false, false, false, elementType.proteina);
    public Sprite associatedImage;

    public float maxZoomIn = 10f;         // Max Zoom Distance in Meters
    public float maxZoomOut = 0f;         // Min Zoom Distance in Meters   
    
     private void OnEnable()
    {
        if (string.IsNullOrEmpty(_id))
            _id = Guid.NewGuid().ToString();

        if(associatedImage == null)
            associatedImage = Resources.Load<Sprite>("placeholder_image");
    }
    
    private void OnValidate()
    {
        if (structuralLevel.type != elementType.proteina)
            structuralLevel.ClearLevels();
    }
}

using System;
using UnityEngine;

[System.Serializable]
    public struct structuralLevel
    {
        public structuralLevel(bool primario, bool secundario, bool terciario, bool quaternario)
        {
            this.primario = primario;
            this.secundario = secundario;
            this.terciario = terciario;
            this.quaternario = quaternario;
        }
        public bool primario;
        public bool secundario;
        public bool terciario;
        public bool quaternario;
    }
[CreateAssetMenu(fileName = "model", menuName = "Model/Create New Model")]
public class Model : ScriptableObject
{
    private string _id;

    public string id => _id;
    public string name = "Não Inserido";
    public string scientificalName = "Não Inserido";
    public string atomicNumber = "Não Inserido";
    public string molecularFormula = "Não Inserida";
    public string applications = "Nenhuma";   
    public structuralLevel structuralLevel = new structuralLevel(true, false, false, false);

    public float maxZoomIn = 10f;         // Max Zoom Distance in Meters
    public float maxZoomOut = 0f;         // Min Zoom Distance in Meters   
    
     private void OnEnable()
    {
        if (string.IsNullOrEmpty(_id))
            _id = Guid.NewGuid().ToString();
    }
}   

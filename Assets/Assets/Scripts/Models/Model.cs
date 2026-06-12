using System;
using UnityEngine;


[CreateAssetMenu(fileName = "model", menuName = "Model/Create New Model")]
public class Model : ScriptableObject
{
    private string _id;

    public string id => _id;
    public string name;
    public string scientificalName;            
    
     private void OnEnable()
    {
        if (string.IsNullOrEmpty(_id))
            _id = Guid.NewGuid().ToString();
    }
}   

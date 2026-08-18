using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardController : MonoBehaviour
{
    public Model model { get; private set;}


    [SerializeField]
    private TMP_Text name;

    [SerializeField]
    private Image sourceImage;

    public void SetupCard(GameObject model)
    {
        this.model = model.GetComponent<ModelController>().modelData;
        name.text = this.model.name;        
        sourceImage.sprite = this.model.associatedImage;
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

     void Start()
    {
        GameController.Instance.GetCanvas();

        StartCoroutine(AutoIniciarTeste());
    }

    private IEnumerator AutoIniciarTeste()
    {
        // Dá tempo para o Firebase registrar o uso de memória no Menu vazio
        yield return new WaitForSeconds(3f); 
        
        // Simula o clique no botão
        StartApplication();
    }
    public void StartApplication()
    {
        GameController.Instance.LoadSimulation();
    }

    public void DownloadPDF()
    {
        GameController.Instance.DownloadCartilha();
    }

    public void ToggleTutorials()
    {
        GameController.Instance.ToggleTutorials();
    }

    public void ToggleHighQualityModels()
    {
        GameController.Instance.ToggleHighQualityModels();
    }

    // public void ExitApplication()
    // {
    //     GameController.Instance.ExitApplication();
    // }
}

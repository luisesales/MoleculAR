using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

     void Start()
    {
        GameController.Instance.GetCanvas();
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

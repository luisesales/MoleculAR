using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewController : MonoBehaviour
{   
    public void Return()
    {
        GameController.Instance.OpenCloseCanvas();
    }

    public void Reset()
    {
        GameController.Instance.ResetModel();
    }

    public void Detail()
    {
        GameController.Instance.DetailModel();
    }

    public void OpenCloseTooltip()
    {
        GameController.Instance.OpenCloseTooltip();
        GameController.Instance.ResetTooltip();
    }

    public void ReturnMenu()
    {
        GameController.Instance.ReturnMenu();
    }
}

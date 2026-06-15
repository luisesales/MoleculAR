using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Vuforia;


public class GameController : MonoBehaviour
{
    // Final Variables
    public static GameController Instance { get; private set; }

    public Dictionary<string, GameObject> prefabs { get; private set; }


    public List<GameObject> prefabsListHighQuality = new List<GameObject>();
    public List<GameObject> prefabsListLowQuality = new List<GameObject>();


    // Changeable Variables
    public GameObject selectedModel { get; private set; }
    private GameObject selectedSceneModel;

    private bool isUsingHighQualityModels = false;    


    // Private Variables

    //CANVAS
    private GameObject canvas;
    private GameObject loadingCanvas;

    private GameObject tutorialCanvas;
    private TMP_Text canvasName;
    private GameObject toolTip;
    private TMP_Text toolTipText;

    private bool activeTutorials = true;

    private List<GameObject> selectedPrefabsList;

    // Start is called before the first frame update

    void Awake()
    {
        if (Instance != null && Instance != this)
        {

            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Initialize selectedModel to null
        selectedModel = null;

        // Get the canvas and its components
        GetCanvas();
        
    }

    // Method to get the canvas and its components
    public void GetCanvas()
    {
        canvas = GameObject.FindWithTag("Canvas");
        if (canvas != null)
        {
            Debug.Log("Canvas found: " + canvas.name);
            canvasName = GameObject.FindWithTag("CanvasName").GetComponent<TMP_Text>();
            toolTip = GameObject.FindWithTag("Tooltip");
            if(toolTip != null)
            {
                toolTipText = toolTip.GetComponentInChildren<TMP_Text>();
            }
        }
        loadingCanvas = GameObject.FindWithTag("Loading");
        if(loadingCanvas != null)
        {
            Debug.Log("Loading canvas found: " + loadingCanvas.name);
            loadingCanvas.SetActive(false);
        }
        tutorialCanvas = GameObject.FindWithTag("CanvasTutorial");
        if(tutorialCanvas != null)
        {
            Debug.Log("Tutorial canvas found: " + tutorialCanvas.name);
            if(!activeTutorials)
                tutorialCanvas.SetActive(false);
        }
    }

    //Called when a model is selected in the simulation scene
    public void SelectModel(GameObject model)
    {
        Debug.Log("Selecting model: " + model.name);
        string modelId = model.GetComponent<ModelController>().modelData.id;
        if (modelId != selectedModel?.GetComponent<ModelController>().modelData.id)
        {
            selectedSceneModel = model;
            selectedModel = prefabs[modelId];
            UpdateCanvas();
            if(toolTip != null)
            {
                toolTipText.text = "Rotacione o modelo para vê-lo de outros ângulos.";
            }
        }
        if (canvas != null && !canvas.activeSelf)
        {
            OpenCloseCanvas();
        }
        OutlineObject();
    }

    //Called when the simulation scene is loaded for preparing the scene
    private void StartSimulation(Scene scene, LoadSceneMode mode)
    {
        InitiatePrefabsDictionary();
        Debug.Log("Starting simulation scene");
        GetCanvas();
        OpenCloseCanvas();
        if(!activeTutorials)
        {
            OpenCloseTooltip();
        }
        selectedModel = null;
        selectedSceneModel = null;
        VuforiaApplication.Instance.OnVuforiaStarted +=
        () =>
        {
            var targetFps = VuforiaBehaviour.Instance.CameraDevice.GetRecommendedFPS();
            if (UnityEngine.Application.targetFrameRate != targetFps)
            {
                Debug.Log("Setting frame rate to " + targetFps + "fps");
                UnityEngine.Application.targetFrameRate = targetFps;
            }
        };
        SceneManager.sceneLoaded -= StartSimulation;
    }

    //Called for loading the simulation scene
    public void LoadSimulation()
    {
        Debug.Log("Loading Simulation Scene");
        StartCoroutine(LoadSceneAsync("CameraView"));                        
    }

    private IEnumerator AnimateLoading(float previousFramePercentage, float percentage, AsyncOperation asyncLoad, LoadingCanvasController loadingCanvasController)
    {
        float time = 0f;        
        {
            time = 0f;
            percentage = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            while(time <= 0.4f)
            {                
                loadingCanvasController.progress = Mathf.Lerp(previousFramePercentage, percentage, time/0.4f);                
                time += Time.deltaTime;                        
                yield return new WaitForEndOfFrame();
            }
            previousFramePercentage = loadingCanvasController.progress;
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {            
        if (loadingCanvas != null)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);  
            asyncLoad.allowSceneActivation = false;
            SceneManager.sceneLoaded += StartSimulation;    
            Debug.Log("Showing loading canvas for scene: " + sceneName);
            loadingCanvas.SetActive(true);
            OpenCloseCanvas();
            LoadingCanvasController loadingCanvasController = loadingCanvas.GetComponent<LoadingCanvasController>();
            float previousFramePercentage = 0f;
            float percentage = 0f;
            while (!asyncLoad.isDone)
            {       
                if (asyncLoad.progress >= 0.9f)
                {
                    Debug.Log("Finalizing loading canvas animation for scene: " + sceneName);            
                    yield return AnimateLoading(previousFramePercentage, percentage, asyncLoad, loadingCanvasController);
                    previousFramePercentage = loadingCanvasController.progress;            
                    asyncLoad.allowSceneActivation = true;         
                }
                if(loadingCanvasController.progress != previousFramePercentage)
                {
                    Debug.Log("Animating loading canvas for scene: " + sceneName + " with progress: " + percentage);
                    yield return AnimateLoading(previousFramePercentage, percentage, asyncLoad, loadingCanvasController);
                    previousFramePercentage = loadingCanvasController.progress;
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                }         
                loadingCanvasController.progress = percentage;
                yield return null;                
            }                                               
        }                
    }

    //Called when the detailed model is closed for returning to the simulation scene
    public void UndetailModel()
    {
        Destroy(GameObject.FindWithTag("TargetModel"));
        LoadSimulation();
    }

    //Called when the detailed model scene is loaded for preparing the scene
    private void InstantiateModel(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Instantiating model: " + selectedModel?.name);        
        GameObject selector = GameObject.FindWithTag("Selector");
        GameObject model = Instantiate(selectedModel);
        model.transform.position = selector.transform.position;
        selectedSceneModel = model;
        GetCanvas();
        UpdateCanvas();
        if(!activeTutorials)
        {
            OpenCloseTooltip();
            activeTutorials = false;
        }
        SceneManager.sceneLoaded -= InstantiateModel;
    }

    //Called for loading the detailed model scene
    public void DetailModel()
    {
        Debug.Log("Detailing model: " + selectedModel?.name);
        if (selectedModel != null)
        {
            SceneManager.LoadScene("DetailedView");
            SceneManager.sceneLoaded += InstantiateModel;
        }
    }

    //Called for reset the selected model rotation
    public void ResetModel()
    {
        Debug.Log("Reseting model rotation: " + selectedSceneModel?.name);
        if (selectedSceneModel != null)
        {
            //Debug.Log("Model rotation: " + selectedSceneModel?.name + " to " + Quaternion.identity);
            selectedSceneModel.transform.localRotation = Quaternion.identity;
        }
    }

    //Called to open or close the canvas
    public void OpenCloseCanvas()
    {
        Debug.Log("Toggling Canvas Active State");
        if (canvas != null)
        {
            canvas.SetActive(!canvas.activeSelf);
            Debug.Log("Changing Canvas Active State to: " + canvas.activeSelf);
        }
    }

    public void OpenCloseLoadingCanvas()
    {
        Debug.Log("Toggling Loading Canvas Active State");
        if (loadingCanvas != null)
        {
            loadingCanvas.SetActive(!loadingCanvas.activeSelf);
            Debug.Log("Changing Loading Canvas Active State to: " + loadingCanvas.activeSelf);
        }
    }

    //Called to update the canvas name based on the selected model
    public void UpdateCanvas()
    {
        if (canvas != null)
        {
            if (selectedModel != null)
                canvasName.text = selectedModel.GetComponent<ModelController>().modelData.name;
            else
            {
                canvasName.text = "No Model Selected";
            }
        }

    }

    //Called to activate and deactivate outline on SelectedSceneModel
    public void OutlineObject()
    {
        if (selectedSceneModel != null)
        {            
            Debug.Log("Outlining model: " + selectedSceneModel.name);
            Outline outline = selectedSceneModel.GetComponent<Outline>();            
            if (outline != null)
            {
                Debug.Log("Toggling outline for model: " + selectedSceneModel.name + " to " + !outline.enabled);
                outline.enabled = !outline.enabled;
            }
        }
    }

    public void OpenCloseTooltip()
    {        
        Debug.Log("Toggling Tooltip Active State");
        if (toolTip != null)
        {
            toolTip.SetActive(!toolTip.activeSelf);
            Debug.Log("Changing Tooltip Active State to: " + toolTip.activeSelf);
        }
    }
    
    public void ResetTooltip()
    {
        if (toolTipText != null)
        {
            toolTipText.text = "Posicione a Câmera em um Marcador e toque no modelo para ver informações.";
        }
    }

    public void NextStep()
    {
        if (tutorialCanvas != null)
        {
            // TutorialCanvasController tutorialCanvasController = tutorialCanvas.GetComponent<TutorialCanvasController>();
            // if (tutorialCanvasController != null)
            // {
            //     tutorialCanvasController.NextStep();
            // }
        }
    }

    public void ToggleHighQualityModels()
    {
        isUsingHighQualityModels = !isUsingHighQualityModels;        
    }

    private void InitiatePrefabsDictionary()
    {
        selectedPrefabsList = isUsingHighQualityModels ? prefabsListHighQuality : prefabsListLowQuality;
        prefabs = new Dictionary<string, GameObject>();        
        foreach (GameObject prefab in selectedPrefabsList)
        {
            if (prefab == null || prefabs.ContainsKey(prefab.GetComponent<ModelController>().modelData.id)) continue;
            prefabs.Add(prefab.GetComponent<ModelController>().modelData.id, prefab);
        }
    }

}

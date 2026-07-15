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

    public static string linkCartilha { get; private set; }  = "https://www.exemplo.com.br/artigo_proteina.pdf";

    public Dictionary<string, GameObject> prefabs { get; private set; }


    public List<GameObject> prefabsListHighQuality = new List<GameObject>();
    public List<GameObject> prefabsListLowQuality = new List<GameObject>();


    // Changeable Variables
    public GameObject selectedModel { get; private set; }

    public bool activeTutorials { get; private set; } = false;
    private GameObject selectedSceneModel;

    private bool isUsingHighQualityModels = false;    


    // Private Variables

    //CANVAS
    private GameObject canvas;
    private GameObject loadingCanvas;

    private GameObject returnMenuCanvas;

    private GameObject tutorialCanvas;
    private TMP_Text canvasName;
    private GameObject toolTip;
    private TMP_Text toolTipText;
    
    private GameObject toggleTutorialsButton;
    private bool firstTimeTutorialSimulation = true;
    private bool firstTimeTutorialDetailed = true;

    private bool tutorialCompleted = false;

    private List<GameObject> selectedPrefabsList;

    // METHODS

    // MONOBEHAVIOUR METHODS

    // Start is called before the first frame update
    void Awake()
    {        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);            
            return;
        }                
        Instance = this;        
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Initialize selectedModel to null
        selectedModel = null;

        // Locks Screen Rotation
        Screen.orientation = ScreenOrientation.Portrait;

        // Starts Menu Scene
        SetupMenu();      
    }

    // PRIVATE METHODS
    private void ToggleGameObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(!obj.activeSelf);
            Debug.Log("Changing " + obj.name + " Active State to: " + obj.activeSelf);
        }
    }

    // Initialize the prefabs dictionary based on the selected quality
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

    // Check if it's the first time the tutorials are being shown and activate the tutorial canvas if necessary
    private void CheckFirstTimeTutorials(ref bool firstTimeTutorial)
    {
        tutorialCanvas = GameObject.FindWithTag("CanvasTutorial");
        if (activeTutorials && firstTimeTutorial)
        {            
            if (tutorialCanvas != null)
            {
                tutorialCanvas.SetActive(true);
                Debug.Log("Tutorial canvas found: " + tutorialCanvas.name);
            }
            else
            {
                Debug.LogWarning("Tutorial canvas not found.");
            }          
            firstTimeTutorial = false;      
            tutorialCompleted = false; 
            Debug.Log("First time tutorial engaged. Setting firstTimeTutorial to false and tutorialCompleted to false on " + firstTimeTutorial.GetType().Name);
            return;       
        }
        Debug.Log("Tutorials are not active or have already been shown.");
        ToggleGameObject(tutorialCanvas);

    }
    
    // Called to Setup the Menu Scene 
    private void SetupMenu()
    {
        Debug.Log("Setting up Menu Scene");
        GetCanvas();       
        toggleTutorialsButton = GameObject.FindWithTag("CanvasToggleTutorial");
        if (toggleTutorialsButton != null)
        {
            toggleTutorialsButton.GetComponent<UnityEngine.UI.Toggle>().SetIsOnWithoutNotify(activeTutorials);
        }
        else
        {
            Debug.LogWarning("Toggle Tutorials Button not found in the Menu scene.");
        }
        
    }

    // Called when the menu scene is loaded and called by SceneManager.sceneLoaded 
    private void StartMenu(Scene scene, LoadSceneMode mode)
    {
       SetupMenu();
       SceneManager.sceneLoaded -= StartMenu;
    }

    //Called when the detailed model scene is loaded for preparing the scene
    private void InstantiateModel(Scene scene, LoadSceneMode mode)
    {        
        CheckFirstTimeTutorials(ref firstTimeTutorialDetailed);        
        Debug.Log("Instantiating model: " + selectedModel?.name);        
        GameObject selector = GameObject.FindWithTag("Selector");
        GameObject model = Instantiate(selectedModel);
        model.transform.position = selector.transform.position;
        selectedSceneModel = model;
        GetCanvas();
        UpdateCanvas();
        OpenCloseTooltip();
        SceneManager.sceneLoaded -= InstantiateModel;
    }

    // Coroutine for animating the loading canvas progress bar
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

    // Coroutine for loading the simulation scene asynchronously with a loading canvas
    private IEnumerator LoadSceneAsync(string sceneName)
    {            
        if (loadingCanvas != null)
        {            
            SceneManager.sceneLoaded += StartSimulation;    
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);  
            asyncLoad.allowSceneActivation = false;
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
                    loadingCanvas.SetActive(false);                                                  
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

    //Called when the simulation scene is loaded for preparing the scene
    private void StartSimulation(Scene scene, LoadSceneMode mode)
    {        
        InitiatePrefabsDictionary();
        Debug.Log("Starting simulation scene");
        GetCanvas();
        OpenCloseCanvas();        
        OpenCloseTooltip();
        Debug.Log("I Passed all Canvas and Tooltip checks");
        CheckFirstTimeTutorials(ref firstTimeTutorialSimulation);
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

    // PUBLIC METHODS

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

    //Called when the detailed model is closed for returning to the simulation scene
    public void UndetailModel()
    {
        Destroy(GameObject.FindWithTag("TargetModel"));
        if (toolTip != null && toolTip.activeSelf)
        {
            toolTip.SetActive(false);
        }
        LoadSimulation();
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
        ToggleGameObject(canvas);
        ToggleGameObject(returnMenuCanvas);
    }

    // Called to open or close the loading canvas
    public void OpenCloseLoadingCanvas()
    {
        Debug.Log("Toggling Loading Canvas Active State");
        ToggleGameObject(loadingCanvas);
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

    //Called for loading the simulation scene
    public void LoadSimulation()
    {
        Debug.Log("Loading Simulation Scene");
        StartCoroutine(LoadSceneAsync("CameraView"));                        
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

    // Called to open or close the tooltip
    public void OpenCloseTooltip()
    {        
        Debug.Log("Toggling Tooltip Active State");
        ToggleGameObject(toolTip);
    }
    
    // Called to reset the tooltip text to its default value
    public void ResetTooltip()
    {
        if (toolTipText != null)
        {
            toolTipText.text = "Posicione a Câmera em um Marcador e toque no modelo para ver informações.";
        }
    }

    // Called to toggle between high quality and low quality models
    public void ToggleHighQualityModels()
    {
        isUsingHighQualityModels = !isUsingHighQualityModels;        
    }

    public void ToggleTutorials()
    {
        activeTutorials = !activeTutorials;       
        Debug.Log("" + activeTutorials);
    }

    // Called to open the PDF link in the native browser and download it
    public void DownloadCartilha()
    {
        Debug.Log("Abrindo o PDF '" + linkCartilha + "' no navegador nativo...");
        Application.OpenURL(linkCartilha);
    }

    // Called to return to the main menu scene
    public void ReturnMenu()
    {        
        SceneManager.LoadScene("Menu");
        SceneManager.sceneLoaded += StartMenu;
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
        ToggleGameObject(loadingCanvas);
        returnMenuCanvas = GameObject.FindWithTag("CanvasReturnMenu");
        ToggleGameObject(returnMenuCanvas);         
    }

    //Called when a model is selected in the simulation scene
    public void SelectModel(GameObject model)
    {        
        if(!tutorialCompleted)
        {
            Debug.Log("First time tutorial simulation engaged.");
            if(tutorialCanvas != null)
            {
                Debug.Log("Tutorial canvas found for simulation tutorial: " + tutorialCanvas.name);
                tutorialCompleted = tutorialCanvas.GetComponent<TutorialCanvasSteps>().NextPart();                
            }
        }
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
}
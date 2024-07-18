using System;
using System.Collections;
using Udar.SceneManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : SingletonMonobehaviour<SceneManager>
{
    public event Action OnReturnToMenu;
    public event Action OnConfirmLoadScene;

    [Header("Loading Screen")]
    [SerializeField] bool loadingScreenEnabled;
    public bool LoadingScreenEnabled { get { return loadingScreenEnabled; } }

    [SerializeField] float loadingScreenDelay;

    [Header("Scene References")]
    [SerializeField] SceneField MenuScene;
    [SerializeField] SceneField DrivingScene;
    [SerializeField] SceneField ResultsScene;
    public SceneField GetDrivingScene() { return DrivingScene;}
    public void SetDrivingScene(SceneField newDrivingScene) { DrivingScene = newDrivingScene;}
    [SerializeField] Canvas loadingScreenCanvas;
    AsyncOperation sceneLoad;
    Coroutine sceneLoadCoroutune;

    private void Start()
    {
        // Subscribe to when we want to load the scenes
        GameManagerNew.Instance.OnSwapLoadMenu += () => { LoadScene(MenuScene); };

        OnReturnToMenu += () => { LoadScene(MenuScene); };
        OnConfirmLoadScene += SwapToSceneAfterConfirm;

        GameManagerNew.Instance.OnSwapLoadMatch += HideLoadingScreen;
        GameManagerNew.Instance.OnSwapEnterMenu += HideLoadingScreen;
    }

    private void OnDisable()
    {
        // Unsubscribe to when we want to load the scenes
        GameManagerNew.Instance.OnSwapLoadMenu -= () => { LoadScene(MenuScene); };

        OnReturnToMenu -= () => { LoadScene(MenuScene); };
        OnConfirmLoadScene -= SwapToSceneAfterConfirm;

        GameManagerNew.Instance.OnSwapLoadMatch -= HideLoadingScreen;
        GameManagerNew.Instance.OnSwapEnterMenu -= HideLoadingScreen;
    }

    /// <summary>
    /// Accessible method for loading the driving scene
    /// </summary>
    public void LoadDrivingScene()
    {
        Debug.Log("LOAD DRIVING SCENE");
        LoadScene(DrivingScene);
    }

    ///<summary>
    /// Main method that loads the game
    ///</summary>
    private void LoadScene(SceneField sceneToLoad)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != sceneToLoad.BuildIndex)
        {
            // Stops Corutine
            if (sceneLoadCoroutune != null)
            {
                StopCoroutine(sceneLoadCoroutune);
                sceneLoadCoroutune = null;
            }

            //tutorialImage.sprite = mainTut;
            ShowLoadingScreen();
            sceneLoadCoroutune = StartCoroutine(LoadSceneAsync(sceneToLoad.BuildIndex, loadingScreenDelay));
        }
    }

    ///<summary>
    /// Loads the scene async
    ///</summary>
    private IEnumerator LoadSceneAsync(int sceneToLoad, float delayTime)
    {
        // Sets gamestate to loading
        GameManagerNew.Instance.SetGameState(GameStates.Loading);

        // Loads the first scene asynchronously
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene is allowed to be activated
        while (!asyncLoad.allowSceneActivation)
        {
            if (asyncLoad.progress >= 0.90f)
            {
                sceneLoad = asyncLoad;
                yield return new WaitForSeconds(delayTime);
                ConfirmLoad();
                break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Method to trigger the scene load
    /// </summary>
    public void ConfirmLoad()
    {
        OnConfirmLoadScene?.Invoke();
    }

    ///<summary>
    /// After all players are confirmed, swap to scene
    ///</summary>
    public void SwapToSceneAfterConfirm()
    {
        if (sceneLoad == null)
            return;

        sceneLoad.allowSceneActivation = true;
    }

    /// <summary>
    /// Shows the loading screen while the new scene is being loaded
    /// </summary>
    private void ShowLoadingScreen()
    {
        // Checks if loading screen is set, if is, return
        if (loadingScreenEnabled == true)
            return;

        loadingScreenCanvas.enabled = true;

        // Sets loading screen to true
        loadingScreenEnabled = true;

    }

    /// <summary>
    /// Hides the loading screen when scene is loaded
    /// </summary>
    private void HideLoadingScreen()
    {
        // Checks if loading screen is not set, if is, return
        if(loadingScreenEnabled == false) 
            return;

        loadingScreenCanvas.enabled = false;

        loadingScreenEnabled = false;
    }
}

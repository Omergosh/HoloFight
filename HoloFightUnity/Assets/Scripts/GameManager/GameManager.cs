using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//public enum GameManagerState
//{
//    IN_MENUS,
//    MANAGING_INPUT_DEVICES,
//    REBINDING_CONTROLS,
//    BATTLE,
//    NETWORKING
//}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public Animator animator;
    public CanvasGroup screenCoverFade; // use to change colors for different screen flash effects

    //public MenuOption currentlyActiveMenu;
    //public MenuCursor menuCursor;
    // moved this stuff into 'MainMenuManager' etc.
    // makes sense especially when you consider there'll be menus where multiple plagyers have their own cursors
    
    // References to Managers for each Scene
    //TitleScreenManager titleScreenManager;
    MainMenuManager mainMenuManager;
    CharSelectManager charSelectManager;
    BattleManagerScript battleManager;
    //TrainingManager trainingManager;

    // References to manage persistent aspects
    // such as Input Devices, Player Profiles, Save Data, etc.
    PlayerConfigurationManager playerConfigurationManager;
    AudioManager audioManager;

    // State
    public bool inTransitionAnimation = false;
    public bool acceptingMenuInputs = true;

    string CurrentSceneName => SceneManager.GetActiveScene().name;
    string nextScene = "";
    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Error, SINGLETON - Trying to create another instance of a singleton!");
            Destroy(this.gameObject); // Someday, this will cause a bug, I'm certain.
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Title screen
    public void TitleScreenAdvance(/* input device to assign to player one? */)
    {
        SceneTransitionStart("MainMenuScene");
    }
    public void TitleScreenQuit()
    {
        Application.Quit();
    }
    #endregion

    #region Main menu
    public void MainMenuExit()
    {
        SceneTransitionStart("TitleScene");
    }

    public void MainMenuToVersusCharSelect()
    {
        SceneTransitionStart("CharacterSelectScene");
    }
    #endregion

    #region Char select
    public void CharSelectExit()
    {
        // Back out back to Main Menu
        SceneTransitionStart("MainMenuScene");
    }

    public void CharSelectToVersus()
    {
        // To the scene of battle! Fight time.
        SceneTransitionStart("FightScene");
    }
    #endregion

    #region Battle
    #endregion

    #region Scene transitions
    void SceneTransitionStart(string newSceneName)
    {
        // Play fade out animation
        nextScene = newSceneName;
        animator.Play("ScreenFadeOut");
        animator.speed = 1;
        acceptingMenuInputs = false;
        //Debug.Log("fade start");
    }
    void SceneTransitionMiddle()
    {
        StartCoroutine(LoadNextAsyncScene());
        //Debug.Log("fade middle");
        animator.speed = 0;
    }

    IEnumerator LoadNextAsyncScene()
    {
        AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(nextScene);
        while (!asyncSceneLoad.isDone)
        {
            //Debug.Log(asyncSceneLoad.progress);
            yield return null;
        }
        animator.Play("ScreenFadeIn");
        animator.speed = 1;
    }

    void SceneTransitionEnd()
    {
        // Activate whatever animations/events/components need activating
        // after new scene has fully faded into view.
        // (called at end of "FadeIn" animation)
        nextScene = "";
        animator.Play("ScreenFadeNone");
        animator.speed = 0;
        acceptingMenuInputs = true;
        //Debug.Log("fade end");
    }
    #endregion
}

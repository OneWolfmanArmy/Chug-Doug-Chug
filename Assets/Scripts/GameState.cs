using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public static GameState Instance = null;

    #region Editor
    
    #endregion


    #region Properties

    private const string MainMenuSceneName = "Scn_MainMenu";
    private const string GameSceneName = "Scn_Game";

    private enum State {Default, MainMenu, Tutorial, Playing, Paused, InGameMenu, GameOver};
    private State mState;
    private bool bCanUpdate = false;
    
    private Doug mDoug;
    private UIManager mUIManager;

    #endregion


    #region MonoBehaviour

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (bCanUpdate || mState == State.Default)
        {
            ProcessCurrentState();
        }
    }

    #endregion

    #region Public Methods
    
    public void Pause()
    {
        if (mState == State.Playing)
        {
            EnterState(State.Paused);
        }
        else if(mState == State.Paused)
        {
            EnterState(State.Playing);
        }
    }

    public void GameOver()
    {
        EnterState(State.GameOver);
    }
    
    public void EnterMainGame()
    {
        EnterState(State.Tutorial);
    }

    public void BeginPlay()
    {
        EnterState(State.Playing);
    }

    public void ExitPlay()
    {
        EnterState(State.MainMenu);
    }

    #endregion


    #region Private Methods

    private void EnterState(State EntryState)
    {
        //Disable OnStateUpdate while transitioning states
        bCanUpdate = false;
        //Exit state before entering new one
        ExitCurrentState();

        Debug.Log("Entering state " + EntryState + "...");
        switch (EntryState)
        {
            case State.MainMenu:
                SceneManager.LoadScene(MainMenuSceneName);
                break;
            case State.Tutorial:
                SceneManager.LoadScene(GameSceneName);
                break;
            case State.Playing:
                mDoug.SetActive(true);
                break;
            case State.Paused:
                Time.timeScale = 0;
                break;
            case State.InGameMenu:
                break;
            case State.GameOver:
                mDoug.SetActive(false);
                break;
            default:

                break;
        }

        mState = EntryState;
        bCanUpdate = true;
    }

    private void ExitCurrentState()
    {
        Debug.Log("Exiting state " + mState + "...");
        switch (mState)
        {
            case State.MainMenu:
                break;
            case State.Tutorial:

                mDoug = GameObject.Find("Doug").GetComponent<Doug>();
                mUIManager = GameObject.Find("Canvas_UI").GetComponent<UIManager>();
                mDoug.Init();
                mUIManager.Init();
                break;
            case State.Playing:
                mDoug.SetActive(false);
                break;
            case State.Paused:
                Time.timeScale = 1;
                break;
            case State.InGameMenu:
                break;
            case State.GameOver:
                mDoug.Init();
                mUIManager.Init();
                break;
            default:

                break;
        }
    }

    private void ProcessCurrentState()
    {
        Debug.Log("Updating state " + mState + "...");
        switch (mState)
        {
            case State.MainMenu:
                break;
            case State.Tutorial:
                break;
            case State.Playing:
                mDoug.OnFrame();
                break;
            case State.Paused:
                break;
            case State.InGameMenu:
                break;
            case State.GameOver:
                break;
            default:
                string SceneName = SceneManager.GetActiveScene().name;
                if (SceneName == MainMenuSceneName)
                {
                    EnterState(State.MainMenu);
                }
                else if (SceneName == GameSceneName)
                {
                    EnterState(State.Tutorial);
                }
                break;
        }
    } 

    #endregion

}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour, IGameLoop
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

    private Tutorial mTutorial;
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


    #region IGameLoop

    public void OnCreate()
    {
        mDoug.OnCreate();
        mUIManager.OnCreate();
    }

    public void OnGameBegin()
    {
        mDoug.OnGameBegin();
        mUIManager.OnGameBegin();
    }

    public void OnFrame()
    {
        mDoug.OnFrame();
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

    IEnumerator LoadMainMenuScene()
    {
        SceneManager.LoadScene(MainMenuSceneName);

        yield return null;
    }

    IEnumerator LoadGameScene()
    {
        SceneManager.LoadScene(GameSceneName);  
        
        yield return null;

        mDoug = GameObject.Find("Game/Doug").GetComponent<Doug>();
        mUIManager = GameObject.Find("Canvases").GetComponent<UIManager>();
        OnCreate();

        mTutorial = GameObject.Find("Tutorial").GetComponent<Tutorial>();
        mTutorial.BeginTutorial();
    }

    #endregion


    #region Private Methods

    private void EnterState(State EntryState)
    {
        //Disable OnStateUpdate while transitioning states
        bCanUpdate = false;
        //Exit state before entering new one
        ExitCurrentState();

        //Debug.Log("Entering state " + EntryState + "...");
        switch (EntryState)
        {
            case State.MainMenu:
                LoadMainMenuScene();
                break;
            case State.Tutorial:
                StartCoroutine(LoadGameScene());         
                break;
            case State.Playing:
                break;
            case State.Paused:
                Time.timeScale = 0;
                break;
            case State.InGameMenu:
                break;
            case State.GameOver:
                break;
            default:

                break;
        }

        mState = EntryState;
        bCanUpdate = true;
    }

    private void ExitCurrentState()
    {
        //Debug.Log("Exiting state " + mState + "...");
        switch (mState)
        {
            case State.MainMenu:
                break;
            case State.Tutorial:
                OnGameBegin();
                break;
            case State.Playing:
                break;
            case State.Paused:
                Time.timeScale = 1;
                break;
            case State.InGameMenu:
                break;
            case State.GameOver:
                OnGameBegin();
                break;
            default:

                break;
        }
    }

    private void ProcessCurrentState()
    {
        //Debug.Log("Updating state " + mState + "...");
        switch (mState)
        {
            case State.MainMenu:
                break;
            case State.Tutorial:
                break;
            case State.Playing:
                OnFrame();
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

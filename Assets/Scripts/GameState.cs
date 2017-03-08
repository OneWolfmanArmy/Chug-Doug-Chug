using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public static GameState Instance = null;

    #region Editor

    public ControlPointManager CPManager;

    #endregion


    #region Properties

    private const string MainMenuSceneName = "Scn_MainMenu";
    private const string GameSceneName = "Scn_Game";

    private enum State {Default, MainMenu, Tutorial, Playing, Paused, InGameMenu, GameOver};
    private State mState;
    private bool bCanUpdate = false;

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

        //EnterState(State.MainMenu);
    }

    void Update()
    {
        if (bCanUpdate)
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
                CPManager = GameObject.Find("ControlPoints").GetComponent<ControlPointManager>();
                CPManager.Enable();
                break;
            case State.Paused:
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
        Debug.Log("Exiting state " + mState + "...");
        switch (mState)
        {
            case State.MainMenu:
                break;
            case State.Tutorial:
                break;
            case State.Playing:
                break;
            case State.Paused:
                break;
            case State.InGameMenu:
                break;
            case State.GameOver:
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
            case State.Default:
                string SceneName = SceneManager.GetActiveScene().name;
                if (SceneName == MainMenuSceneName)
                {
                    EnterState(State.MainMenu);
                }
                else if(SceneName == GameSceneName)
                {
                    EnterState(State.Tutorial);
                }
                break;
            case State.MainMenu:
                break;
            case State.Tutorial:
                break;
            case State.Playing:
                break;
            case State.Paused:
                break;
            case State.InGameMenu:
                break;
            case State.GameOver:
                break;
            default:
                
                break;
        }
    } 

    #endregion

}

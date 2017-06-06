using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : Singleton<GameState>, IGameLoop
{
    #region Editor
        
    public GameObject VisualDebugPrefab;

    #endregion


    #region Properties

    private const string MainMenuSceneName = "Scn_MainMenu";
    private const string GameSceneName = "Scn_Game";

    private enum State { Default, MainMenu, Tutorial, Playing, Paused, InGameMenu, GameOver };
    private State mState;
    private bool bCreated = false;
    private bool bCanUpdate = false;

    private CameraManager CMan;
    private GameLoopManager GMan;
    private UIManager UIMan;

    #endregion


    #region MonoBehaviour

    void Awake()
    {
        base.Awake();
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
        if (bCreated)
        {
            EnterState(State.Playing);
            return;
        }

        CMan = CameraManager.Instance;
        GMan = GameLoopManager.Instance;
        UIMan = UIManager.Instance;

        GMan.OnCreate();
        UIMan.OnCreate();

        bCreated = true;
    }

    public void OnGameBegin()
    {
        GMan.OnGameBegin();
        UIMan.OnGameBegin();
    }

    public void OnFrame()
    {
        GMan.OnFrame();
    }

    #endregion


    #region Public Methods

    public void DisplayVisualDebug(TextMesh DebugTextMesh, string DebugText, Color TextColor, float Duration)
    {
        DebugTextMesh.text = DebugText;
        DebugTextMesh.color = TextColor;
        StartCoroutine(HideVisualDebug(DebugTextMesh, Duration));
    }

    IEnumerator HideVisualDebug(TextMesh DebugTextMesh, float DisplayDuration)
    {
        yield return new WaitForSeconds(DisplayDuration);

        DebugTextMesh.text = "";
    }

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
        CMan.gameObject.SetActive(false);
        GMan.gameObject.SetActive(false);
        UIMan.gameObject.SetActive(false);
        EnterState(State.MainMenu);
    }

    IEnumerator LoadMainMenuScene()
    {
        if (SceneManager.GetActiveScene().name != MainMenuSceneName)
        {
            SceneManager.LoadScene(MainMenuSceneName);
        }

        yield return null;
    }

    IEnumerator LoadGameScene()
    {
        if (SceneManager.GetActiveScene().name != GameSceneName)
        {
            SceneManager.LoadScene(GameSceneName);
        }
        
        yield return null;

        OnCreate();
        CMan.gameObject.SetActive(true);
        GMan.gameObject.SetActive(true);
        UIMan.gameObject.SetActive(true);
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
                StartCoroutine(LoadMainMenuScene());
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
                UIMan.TutorialDisplay.gameObject.SetActive(false);
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

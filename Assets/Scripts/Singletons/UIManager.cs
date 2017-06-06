using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>, IGameLoop
{
    #region Static Accessors

    public static float WIDTH;
    public static float HEIGHT;

    #endregion


    #region Editor

    public Canvas BGCanvas;
    public Canvas UICanvas;
    public Canvas FGCanvas;

    public Tutorial TutorialDisplay;
    public RectTransform PauseDisplay;
    public RectTransform FinalScoreDisplay;
    public RectTransform GameOverOverlay;
    
    public Text ScoreText;

    public ProgressBar IntoxicationBar;
    public ProgressBar CredBar;
    public Text CredMultiplier;

    public Button PauseButton;
    public Button ExitButton;

    #endregion


    #region IGameLoop

    public void OnCreate()
    {
        WIDTH = UICanvas.pixelRect.width;
        HEIGHT = UICanvas.pixelRect.height;

        ScoreText.text = "";

        if (TutorialDisplay.gameObject.activeSelf)
        {
            TutorialDisplay.BeginTutorial();
        }
        else
        {
            GameState.Instance.BeginPlay();
        }
    }

    public void OnGameBegin()
    {
        PauseButton.gameObject.SetActive(true);
        ExitButton.gameObject.SetActive(true);
    }

    public void OnFrame() {}

    #endregion


    #region Button Callbacks

    public void OnExitButtonPressed()
    {
        GameState.Instance.ExitPlay();
    }

    public void OnPauseButtonPressed()
    {
        FGCanvas.gameObject.SetActive(true);
        UICanvas.gameObject.SetActive(false);
        PauseDisplay.gameObject.SetActive(true);
        GameState.Instance.Pause();
    }

    public void OnPauseOverlayPressed()
    {
        FGCanvas.gameObject.SetActive(false);
        UICanvas.gameObject.SetActive(true);
        PauseDisplay.gameObject.SetActive(false);
        GameState.Instance.Pause();
    }

    public void OnExitTutorialButtonPressed()
    {
        PauseButton.gameObject.SetActive(true);
        ExitButton.gameObject.SetActive(true);
        TutorialDisplay.ExitTutorial();
    }

    public void OnPlayAgainButtonPressed()
    {
        FinalScoreDisplay.gameObject.SetActive(false);
        GameOverOverlay.gameObject.SetActive(false);
        FGCanvas.gameObject.SetActive(false);
        IntoxicationBar.gameObject.SetActive(true);
        CredBar.gameObject.SetActive(true);
        GameState.Instance.BeginPlay();
    }

    #endregion


    #region Metric Methods

    public void SetScoreMetrics(float Score, float Intoxication, float Cred)
    {
        ScoreText.text = ((int)Score).ToString();
        IntoxicationBar.ResizeFilling(Intoxication);
        CredBar.ResizeFilling(Cred);
    }

    public void UpdateScoreText(float Score)
    {
        ScoreText.text = ((int)Score).ToString();
    }

    public void UpdateIntoxicationBar(float Intoxication)
    {
        IntoxicationBar.ResizeFilling(Intoxication);
    }

    public void UpdateCredBar(float StreetCred)
    {
        CredBar.ResizeFilling(StreetCred);
    }

    public void UpdateCredMultiplier(float Multiplier)
    {
        CredMultiplier.text = Multiplier.ToString();
    }

    public int GetCredMultiplier()
    {
        return System.Int32.Parse(CredMultiplier.text);
    }

    #endregion


    #region Events

    public void DisplayFinalScore()
    {
        ScoreText.text = "GAME OVER";

        IntoxicationBar.gameObject.SetActive(false);
        CredBar.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);
        ExitButton.gameObject.SetActive(false);

        FinalScoreDisplay.gameObject.SetActive(true);
        FGCanvas.gameObject.SetActive(true);
        GameOverOverlay.gameObject.SetActive(true);
    }

    #endregion
}

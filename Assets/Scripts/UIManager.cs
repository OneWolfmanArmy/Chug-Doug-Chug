using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text ScoreText;
    public Tutorial TutorialDisplay;
    public Transform FinalScoreDisplay;
    public ProgressBar IntoxicationBar;
    public ProgressBar CredBar;

    public Button PauseButton;
    public Button ExitButton;  

    public void OnExitButtonPressed()
    {
        GameState.Instance.ExitPlay();
    }

    public void OnPauseButtonPressed()
    {
        GameState.Instance.Pause();
    }

    public void OnExitTutorialButtonPressed()
    {
        TutorialDisplay.ExitTutorial();
    }

    public void OnPlayAgainButtonPressed()
    {
        FinalScoreDisplay.gameObject.SetActive(false);
        GameState.Instance.BeginPlay();
    }

    public void Init()
    {
        PauseButton.gameObject.SetActive(true);
        ExitButton.gameObject.SetActive(true);
    }

    public void SetScoreUI(int Score, float Intoxication, float Cred)
    {
        ScoreText.text = Score.ToString();
        IntoxicationBar.ResizeFilling(Intoxication);
        CredBar.ResizeFilling(Cred);
    }

    public void UpdateScoreText(int Points)
    {
        ScoreText.text = (int.Parse(ScoreText.text) + Points).ToString();
    }

    public void UpdateIntoxicationBar(float Intoxication)
    {
        IntoxicationBar.ResizeFilling(Intoxication);
    }

    public void UpdateStreetCredBar(float StreetCred)
    {
        CredBar.ResizeFilling(StreetCred);
    }

    public void DisplayFinalScore()
    {
        ScoreText.text = "GAME OVER";
        FinalScoreDisplay.gameObject.SetActive(true);
    }
}

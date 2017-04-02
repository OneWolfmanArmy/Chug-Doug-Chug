using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text Score;
    public ProgressBar IntoxicationBar;
    public ProgressBar StreetCredBar;

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

    public void Init()
    {
        PauseButton.gameObject.SetActive(true);
        ExitButton.gameObject.SetActive(true);
    }

    public void InitScoreUI(int StartScore, float StartIntoxication, float StartStreetCred)
    {
        Score.text = StartScore.ToString();
        IntoxicationBar.ResizeFilling(StartIntoxication);
        StreetCredBar.ResizeFilling(StartStreetCred);
    }

    public void UpdateScoreText(int Points)
    {
        Score.text = (int.Parse(Score.text) + Points).ToString();
    }

    public void UpdateIntoxicationBar(float Intoxication)
    {
        IntoxicationBar.ResizeFilling(Intoxication);
    }

    public void UpdateStreetCredBar(float StreetCred)
    {
        StreetCredBar.ResizeFilling(StreetCred);
    }
}

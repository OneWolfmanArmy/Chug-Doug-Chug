using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text Score;
    public ProgressBar IntoxicationBar;
    public ProgressBar StreetCredBar;

    public void ExitButton()
    {
        GameState.Instance.ExitPlay();
    }

    public void PauseButton()
    {
        GameState.Instance.Pause();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doug : MonoBehaviour, IGameLoop
{
    public UIManager UI;
    public ScoreManager ScoreBoard;
    public ControlPointManager CPManager;
    
    private bool bDrinking;

    public void OnGameBegin()
    {
        ScoreBoard.OnGameBegin();
        CPManager.OnGameBegin();

        StartDrinking();
    }    

    public void OnFrame()
    {
        CPManager.OnFrame();

        if(bDrinking)
        {
            ScoreBoard.IncrementIntoxication();
        }
    }

    public void StartDrinking()
    {
        bDrinking = true;
    }

    public void StopDrinking()
    {
        bDrinking = false;
    }


}

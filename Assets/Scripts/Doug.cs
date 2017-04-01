using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doug : MonoBehaviour
{
    public ScoreManager ScoreBoard;
    public ControlPointManager CPManager;

    private bool bActive;
    private bool bDrinking;

    public void Init()
    {
        ScoreBoard.Init();
        CPManager.Init();

        StartDrinking();
    }

    public void SetActive(bool Active)
    {
        CPManager.SetActive(Active);
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

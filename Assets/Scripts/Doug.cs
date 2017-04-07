using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doug : MonoBehaviour, IGameLoop
{
    public ScoreManager ScoreBoard;
    public ControlPointManager CPManager;

    public Transform BeerSrc;
    public Transform BeerDst;

    public float MinDrinkingDistance;
    

    private bool bDrinking;

    public void OnGameBegin()
    {
        ScoreBoard.OnGameBegin();
        CPManager.OnGameBegin();

        MinDrinkingDistance *= MinDrinkingDistance;

        StartDrinking();
    }    

    public void OnFrame()
    {
        CPManager.OnFrame();

        if(CanDrink())
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

    private bool CanDrink()
    {
        return Vector2.SqrMagnitude(BeerDst.position - BeerSrc.position) <= MinDrinkingDistance;
    }


}

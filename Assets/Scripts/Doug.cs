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

    public void OnCreate()
    {
        MinDrinkingDistance *= MinDrinkingDistance;
        ScoreBoard.OnCreate();
        CPManager.OnCreate();
    }

    public void OnGameBegin()
    {
        ScoreBoard.OnGameBegin();
        CPManager.OnGameBegin();       
    }    

    public void OnFrame()
    {
        CPManager.OnFrame();

        if(DrinkingDistance())
        {
            ScoreBoard.IncrementIntoxication();
            ScoreBoard.IncrementCred();
            ScoreBoard.IncrementScore(ScoreBoard.DrinkScoreMultiplier);
        }
        else
        {
            ScoreBoard.DecrementIntoxication();
            ScoreBoard.DecrementStreetCred();
        }

        ScoreBoard.OnFrame();
    }

    public bool DrinkingDistance()
    {
        return Vector2.SqrMagnitude(BeerDst.position - BeerSrc.position) <= MinDrinkingDistance;
    }


}

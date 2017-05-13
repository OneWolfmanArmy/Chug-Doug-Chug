using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doug : MonoBehaviour, IGameLoop
{
    #region Editor
    
    [System.Serializable]
    public struct Difficulty
    {
        public int ScoreRequisite;

        public int MaxDriftCount;
        public float MinDriftDelay;
        public float MaxDriftDelay;
        public float MaxDriftTime;
    }
    public Difficulty[] Difficulties;

    public float InitialDelay;

    public ScoreManager ScoreBoard;
    public ControlPointManager CPManager;

    public Transform BeerSrc;
    public Transform BeerDst;

    public float MinDrinkingDistance;

    #endregion


    #region Properties

    private bool bDrinking;
    private int mDifficultyLevel;

    #endregion


    #region IGameLoop

    public void OnCreate()
    {
        MinDrinkingDistance *= MinDrinkingDistance;

        ScoreBoard.OnCreate();
        CPManager.OnCreate();
    }

    public void OnGameBegin()
    {
        mDifficultyLevel = 0;
        ScoreBoard.SetDifficulty(Difficulties[mDifficultyLevel]);

        ScoreBoard.OnGameBegin();
        CPManager.OnGameBegin();
    }    

    public void OnFrame()
    {
        if(mDifficultyLevel < (Difficulties.Length - 1) && CanIncreaseDifficulty())
        {
            IncreaseDifficulty();
        }

        if(DrinkingDistance())
        {
            ScoreBoard.IncrementIntoxication();
            ScoreBoard.IncrementCred();
            ScoreBoard.IncrementScore(ScoreBoard.DrinkScoreMultiplier);
        }
        else
        {
            ScoreBoard.DecrementIntoxication();
            ScoreBoard.DecrementCred();
        }

        ScoreBoard.OnFrame();
        CPManager.OnFrame();
    }

    #endregion


    #region Public Methods

    public bool DrinkingDistance()
    {
        return Vector2.SqrMagnitude(BeerDst.position - BeerSrc.position) <= MinDrinkingDistance;
    }

    #endregion


    #region Private Methods

    private bool CanIncreaseDifficulty()
    {
        return ScoreBoard.CanIncreaseDifficulty();
    }

    private void IncreaseDifficulty()
    {
        mDifficultyLevel++;
        Difficulty CurrentDifficulty = Difficulties[mDifficultyLevel];
        ScoreBoard.SetDifficulty(CurrentDifficulty);
        CPManager.SetDifficulty(CurrentDifficulty);
    }

    #endregion

}

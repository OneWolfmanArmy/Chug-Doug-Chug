using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour, IGameLoop
{
    #region Editor

    public UIManager ScoreUI;

    public float DrinkMultiplier;
    public float IntoxicationRate;
    public float StreetCredRate;
       
    private const float FATAL_INTOXICATION = 1.0f;
    private const float FATAL_STREETCRED = 0.0f;

    private int mScore;
    private float mIntoxication;
    private float mStreetCred;

    #endregion


    #region IGameLoop

    public void OnGameBegin()
    {
        mScore = 0;
        mIntoxication = 0.0f;        
        mStreetCred = .5f;

        SynchronizeUI();
    }

    public void OnFrame(){}

    #endregion
        

    #region Public Methods

    public void IncrementScore(int Points)
    {
        mScore += Points;
    }

    public void IncrementIntoxication()
    {
        mIntoxication += IntoxicationRate * Time.deltaTime;
        if (mIntoxication >= FATAL_INTOXICATION)
        {
            ScoreUI.UpdateIntoxicationBar(FATAL_INTOXICATION);
            FinalizeScore();
        }
        else
        {
            ScoreUI.UpdateIntoxicationBar(mIntoxication);
        }
    }

    public void IncrementStreetCred()
    {
        mStreetCred += StreetCredRate * Time.deltaTime;
    }

    public void DecrementIntoxication()
    {
        mIntoxication -= IntoxicationRate * Time.deltaTime;
    }

    public void DecrementStreetCred()
    {
        mStreetCred -= StreetCredRate * Time.deltaTime;
        if(mStreetCred <= FATAL_STREETCRED)
        {
            ScoreUI.UpdateIntoxicationBar(FATAL_STREETCRED);
            FinalizeScore();
        }
        else
        {
            ScoreUI.UpdateIntoxicationBar(mStreetCred);
        }   
    }

    #endregion


    #region Private Methods

    private void SynchronizeUI()
    {
        ScoreUI.SetScoreMetrics(mScore, mIntoxication, mStreetCred);
    }

    private void FinalizeScore()
    {
        ScoreUI.DisplayFinalScore();
        GameState.Instance.GameOver();
    }

    #endregion
}

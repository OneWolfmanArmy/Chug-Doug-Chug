using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour, IGameLoop
{
    #region Editor

    public UIManager ScoreUI;

    public bool Sandbox;

    public float DrinkScoreMultiplier;
    public float IntoxicationRate;
    public float SoberRate;
    public float CredGainRate;
    public float CredLoseRate;
       
    private const float FATAL_INTOXICATION = 1.0f;
    private const float FATAL_STREETCRED = 0.0f;

    private float mScore;
    private float mIntoxication;
    private float mCred;

    #endregion


    #region IGameLoop

    public void OnCreate()
    {
    }

    public void OnGameBegin()
    {
        mScore = 0;
        mIntoxication = 0.0f;        
        mCred = .5f;

        ScoreUI.SetScoreMetrics(mScore, mIntoxication, mCred);
    }

    public void OnFrame()
    {
        if (!Sandbox)
        {
            ScoreUI.SetScoreMetrics(mScore, mIntoxication, mCred);
        }
        else
        {
            ScoreUI.SetScoreMetrics(mScore, 0, .5f);
        }

    }

    #endregion
        

    #region Public Methods

    public void IncrementScore(float Points)
    {
        mScore += Points * Time.deltaTime;
    }

    public void IncrementIntoxication()
    {
        mIntoxication += IntoxicationRate * Time.deltaTime;
        if (mIntoxication >= FATAL_INTOXICATION)
        {
            FinalizeScore();
        }
    }

    public void IncrementCred()
    {
        mCred = Mathf.Min(1.0f, mCred + CredGainRate * Time.deltaTime);
    }

    public void DecrementIntoxication()
    {
        mIntoxication = Mathf.Max(0, mIntoxication - SoberRate * Time.deltaTime);
    }

    public void DecrementStreetCred()
    {
        mCred -= CredLoseRate * Time.deltaTime;
        if(mCred <= FATAL_STREETCRED)
        {
            ScoreUI.UpdateCredBar(FATAL_STREETCRED);
            FinalizeScore();
        }
    }

    #endregion


    #region Private Methods

    private void FinalizeScore()
    {
        ScoreUI.DisplayFinalScore();
        GameState.Instance.GameOver();
    }

    #endregion
}

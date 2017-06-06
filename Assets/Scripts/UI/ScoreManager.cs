using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour, IGameLoop
{
    #region Editor

    public bool Sandbox;

    public float InitialIntoxication;
    public float InitialCred;

    public float DrinkScoreMultiplier;
    public float IntoxicationRate;
    public float SoberRate;
    public float CredGainRate;
    public float CredLossRate;

    #endregion


    #region Properties

    private const float FATAL_INTOXICATION = 1.0f;
    private const float FATAL_CRED = 0.0f;
    
    private DifficultyLevel.Score mDifficulty;

    private float mScore;
    private float mIntoxication;
    private float mCred;

    #endregion


    #region IGameLoop

    public void OnCreate()
    {}

    public void OnGameBegin()
    {
        mScore = 0;
        mIntoxication = InitialIntoxication;        
        mCred = InitialCred;

        UpdateScoreUI();
    }

    public void OnFrame()
    {
        if (!Sandbox)
        {
            UpdateScoreUI();
        }
        else
        {
            UIManager.Instance.SetScoreMetrics(mScore, InitialIntoxication, InitialCred);
        }
    }

    #endregion
        

    #region Public Methods

    public void SetDifficulty(DifficultyLevel.Score Difficulty)
    {
        mDifficulty.Target = Difficulty.Target;
    }

    public bool CanIncreaseDifficulty()
    {
        return mScore >= mDifficulty.Target;
    }

    public void IncrementScore(float Points)
    {
        mScore += Points * Time.deltaTime;
    }

    public void IncrementIntoxication()
    {
        mIntoxication += IntoxicationRate * Time.deltaTime;
        if (mIntoxication >= FATAL_INTOXICATION)
        {
            mIntoxication = FATAL_INTOXICATION;
            FinalizeScore();
        }
    }

    public void IncrementCred()
    {
        mCred = Mathf.Min(1.0f, mCred + CredGainRate * UIManager.Instance.GetCredMultiplier() * Time.deltaTime);
    }

    public void DecrementIntoxication()
    {
        mIntoxication = Mathf.Max(0, mIntoxication - SoberRate * Time.deltaTime);
    }

    public void DecrementCred()
    {
        mCred -= CredLossRate * UIManager.Instance.GetCredMultiplier() * Time.deltaTime;
        if(mCred <= FATAL_CRED)
        {
            mCred = FATAL_CRED;
            FinalizeScore();
        }
    }

    #endregion


    #region Private Methods

    private void UpdateScoreUI()
    {
        UIManager.Instance.SetScoreMetrics(mScore, mIntoxication, mCred);
    }

    private void FinalizeScore()
    {
        UIManager.Instance.DisplayFinalScore();
        GameState.Instance.GameOver();
    }

    #endregion
}

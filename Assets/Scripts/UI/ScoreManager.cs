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

    [Range(0, 1)]
    public float AlarmIntoxication;
    [Range(0, 1)]
    public float AlarmCred;

    #endregion


    #region Properties

    private const float FATAL_INTOXICATION = 1.0f;
    private const float FATAL_CRED = 0.0f;

    public DifficultyLevel.Score Difficulty { set { mDifficulty = value; } }
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
            TestWarningCondition();
            TestLoseCondition();
            UpdateScoreUI();
        }
        else
        {
            UIManager.Instance.SetScoreMetrics(mScore, InitialIntoxication, InitialCred);
        }
    }

    #endregion
        

    #region Public Methods

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
    }

    public void IncrementCred(float Multiplier)
    {
        mCred = Mathf.Min(1.0f, mCred + CredGainRate * Multiplier * Time.deltaTime);
    }

    public void DecrementIntoxication()
    {
        mIntoxication = Mathf.Max(0, mIntoxication - SoberRate * Time.deltaTime);
    }

    public void DecrementCred(float Multiplier)
    {
        mCred -= CredLossRate * Multiplier * Time.deltaTime;
    }

    #endregion


    #region Private Methods

    private void UpdateScoreUI()
    {
        UIManager.Instance.SetScoreMetrics(mScore, mIntoxication, mCred);
    }

    private void TestWarningCondition()
    {
        if (mIntoxication >= FATAL_INTOXICATION || mCred <= FATAL_CRED)
        {
            FinalizeScore();
        }
    }

    private void TestLoseCondition()
    {
        if (mIntoxication >= AlarmIntoxication || mCred <= AlarmCred)
        {
            AudioManager.Instance.PlaySoundEffect(gameObject, "Siren");
        }
        else
        {
            AudioManager.Instance.StopSFX(gameObject);
        }
    }

    private void FinalizeScore()
    {
        UIManager.Instance.DisplayFinalScore();
        GameState.Instance.GameOver();
    }

    #endregion
}

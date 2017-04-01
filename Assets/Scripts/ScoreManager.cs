using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public UIManager ScoreUI;

    public float DrinkMultiplier;
    public float IntoxicationRate;
    public float StreetCredRate;
       
    private const float FATAL_INTOXICATION = 1.0f;
    private const float FATAL_STREETCRED = 0.0f;

    private int mScore;
    private float mIntoxication;
    private float mStreetCred;

    public void Init()
    {
        mScore = 0;
        mIntoxication = 0.0f;        
        mStreetCred = .5f;
        ScoreUI.InitScoreUI(mScore, mIntoxication, mStreetCred);
    }

    private void UpdateAllUI()
    {
        ScoreUI.UpdateIntoxicationBar(mIntoxication);
        ScoreUI.UpdateStreetCredBar(mStreetCred);
        ScoreUI.UpdateScoreText(mScore);
    }

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
            GameState.Instance.GameOver();
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
            GameState.Instance.GameOver();
        }
        else
        {
            ScoreUI.UpdateIntoxicationBar(mStreetCred);
        }   
    }
}

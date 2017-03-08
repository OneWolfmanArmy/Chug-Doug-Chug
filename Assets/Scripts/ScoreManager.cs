using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public UIManager ScoreUI;

    public float IntoxicationRate;
    public float StreetCredRate;

    private int mScore;
    private float mIntoxication;
    private float mStreetCred;

    void Start()
    {
        mScore = 0;
        mIntoxication = 0.0f;        
        mStreetCred = .5f;
        UpdateAllUI();
    }

    private void UpdateAllUI()
    {
        ScoreUI.UpdateIntoxicationBar(mIntoxication);
        ScoreUI.UpdateStreetCredBar(mStreetCred);
        ScoreUI.UpdateScore(mScore);
    }

    public void IncrementScore(int Points)
    {
        mScore += Points;
    }

    public void IncrementIntoxication()
    {
        mIntoxication += IntoxicationRate * Time.deltaTime;
        if(mIntoxication >= 1)
        {
            GameState.Instance.GameOver();
        }

        ScoreUI.UpdateIntoxicationBar(mIntoxication);
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
        if(mStreetCred <= 0.0f)
        {
            GameState.Instance.GameOver();
        }
    }
}

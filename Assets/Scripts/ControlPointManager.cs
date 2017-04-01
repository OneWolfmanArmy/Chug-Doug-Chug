using System.Collections.Generic;
using UnityEngine;

public class ControlPointManager : MonoBehaviour
{
    [System.Serializable]
    public struct Difficulty
    {
        public int MaxDriftCount;
        public float MinDriftDelay;
        public float MaxDriftDelay;
    }
    public Difficulty[] Difficulties;

    public ControlPoint[] ControlPoints;

    private bool bActive;
    private int mDifficultyLevel;
    private Difficulty mDifficulty;

    private List<ControlPoint> mStableCP;
    private List<ControlPoint> mDriftingCP;

    public void SetActive(bool Active)
    {
        bActive = Active;
        for (int i = 0; i < ControlPoints.Length; i++)
        {
            ControlPoints[i].SetActive(Active);
        }
    }

    public void Init()
    {
        mStableCP = new List<ControlPoint>(ControlPoints);
        mDriftingCP = new List<ControlPoint>(ControlPoints.Length);
        IncreaseDifficulty();
    }

    public void OnFrame()
    {
        if (bActive && mDriftingCP.Count < mDifficulty.MaxDriftCount)
        {
            SelectNextDrifter();
        }
    }

    public void IncreaseDifficulty()
    {
        mDifficulty = Difficulties[mDifficultyLevel];
        mDifficultyLevel++;
    }

    private void SelectNextDrifter()
    {
        int RandomIndex = Random.Range(0, mStableCP.Count);
        ControlPoint ToDriftCP = mStableCP[RandomIndex];
        mStableCP.Remove(ToDriftCP);
        mDriftingCP.Add(ToDriftCP);
        float RandomDelay = Random.Range(mDifficulty.MinDriftDelay, mDifficulty.MaxDriftDelay);
        ToDriftCP.Destabilize(() => RemoveFromDriftingList(ToDriftCP), RandomDelay);
    }

    private void RemoveFromDriftingList(ControlPoint ToStableCP)
    {
        mDriftingCP.Remove(ToStableCP);
        mStableCP.Add(ToStableCP);
    }
}

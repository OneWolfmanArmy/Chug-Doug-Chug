using System.Collections.Generic;
using UnityEngine;

public class ControlPointManager : MonoBehaviour, IGameLoop
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
    
    private int mDifficultyLevel;
    private Difficulty mDifficulty;

    private List<ControlPoint> mStableCP;
    private List<ControlPoint> mDriftingCP;
    
    public void OnGameBegin()
    {
        mStableCP = new List<ControlPoint>(ControlPoints);
        mDriftingCP = new List<ControlPoint>(ControlPoints.Length);
        mDifficulty = Difficulties[0];
        mDifficultyLevel = 1;
        for(int i = 0; i < ControlPoints.Length; i++)
        {
            ControlPoints[i].Init();
        }
    }
    
    public void OnFrame()
    {
        if (mDriftingCP.Count < mDifficulty.MaxDriftCount)
        {
            SelectNextDrifter();
        }
    }

    public void IncreaseDifficulty()
    {
        mDifficulty = Difficulties[mDifficultyLevel];
        mDifficultyLevel++;
    }

    private void ResetControlNodes()
    {
        for (int i = 0; i < ControlPoints.Length; i++)
        {
            ControlPoints[i].ResetNode();
        }
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

using System.Collections.Generic;
using UnityEngine;

public class ControlPointManager : MonoBehaviour
{
    public ControlPoint[] ControlPoints;

    private int mDriftCount;
    private int mMaxDriftCount;

    private List<ControlPoint> mStableCP;
    private List<ControlPoint> mDriftingCP;

    void Start()
    {
        mStableCP = new List<ControlPoint>(ControlPoints);
        mDriftingCP = new List<ControlPoint>(ControlPoints.Length);
        
    }

    private void SelectNextDrift()
    {
        int RandomCP = Random.Range(0, mStableCP.Count);
        ControlPoint ToDriftCP = mStableCP[RandomCP];
        mStableCP.Remove(ToDriftCP);
        mDriftingCP.Add(ToDriftCP);

    }
}

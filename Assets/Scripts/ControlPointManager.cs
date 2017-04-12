using System.Collections.Generic;
using UnityEngine;

public class ControlPointManager : MonoBehaviour, IGameLoop
{
    public ControlPoint DebugCP;

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
            if (ControlPoints[i].gameObject != null)
            {
                ControlPoints[i].OnGameBegin();
            }
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
        if (DebugCP == null)
        {
            mDifficulty = Difficulties[mDifficultyLevel];
            mDifficultyLevel++;
        }
    }

    private void ResetControlNodes()
    {
        for (int i = 0; i < ControlPoints.Length; i++)
        {
            if (ControlPoints[i] != null)
            {
                ControlPoints[i].ResetNode();
            }
        }
    }


    private void SelectNextDrifter()
    {
        ControlPoint ToDriftCP;
        if (DebugCP == null)
        {
            int RandomIndex = Random.Range(0, mStableCP.Count);
            ToDriftCP = mStableCP[RandomIndex];            
        }
        else
        {
            ToDriftCP = DebugCP;
        }

        mStableCP.Remove(ToDriftCP);
        mDriftingCP.Add(ToDriftCP);
        float RandomDelay = Random.Range(mDifficulty.MinDriftDelay, mDifficulty.MaxDriftDelay);
        ToDriftCP.Destabilize(() => {
            RemoveFromDriftingList(ToDriftCP);
        }        
        , RandomDelay);

        for (int i = 0; i < mStableCP.Count; i++)
        {
            if (mStableCP[i].Influence < ToDriftCP.Influence)
            {
                mStableCP[i].SetRigidBodyType(RigidbodyType2D.Dynamic);
            }
        }
    }

    private void RemoveFromDriftingList(ControlPoint ToStableCP)
    {
        mDriftingCP.Remove(ToStableCP);
        mStableCP.Add(ToStableCP);
      //  for(int i = 0; i < ControlPoints.Length; i++)
       // {
          //  ControlPoints[i].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
          //  ControlPoints[i].GetComponent<Rigidbody2D>().angularVelocity = 0;
        //}
    }

}

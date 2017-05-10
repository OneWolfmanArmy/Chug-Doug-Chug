using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPointManager : MonoBehaviour, IGameLoop
{
    #region Editor

    public ControlPoint[] ControlPoints;
    public ControlPoint DebugCP;

    #endregion


    #region Properties

    private struct CPDifficulty
    {
        public int MaxDriftCount;
        public float MinDriftDelay;
        public float MaxDriftDelay;
        public float MaxDriftTime;
    }
    private CPDifficulty mDifficulty;

    private List<ControlPoint> mStableCP;
    private List<ControlPoint> mDriftingCP;
    private ControlPoint mPreviousCP;

    #endregion


    #region IGameLoop

    public void OnCreate()
    {
        mStableCP = new List<ControlPoint>(ControlPoints);
        mDriftingCP = new List<ControlPoint>(ControlPoints.Length);

        for (int i = 0; i < ControlPoints.Length; i++)
        {
            if (ControlPoints[i] != null)
            {
                ControlPoint CP = ControlPoints[i];
                CP.OnCreate();
                CP.SetCallbacks
                (() =>
                {
                    AddInfluence(CP);
                },
                () =>
                {
                    RemoveFromDriftingList(CP);
                });                
            }
        }

        ResetControlNodes();
    }

    public void OnGameBegin()
    {
        mStableCP = new List<ControlPoint>(ControlPoints);
        mDriftingCP = new List<ControlPoint>(ControlPoints.Length);
        mPreviousCP = null;

        ResetControlNodes();

        for (int i = 0; i < ControlPoints.Length; i++)
        {
            if (ControlPoints[i] != null)
            {
                ControlPoints[i].OnGameBegin();
                if (ControlPoints[i].Draggable)
                {
                    mStableCP.Remove(ControlPoints[i]);
                }
            }
        }
    }
    
    public void OnFrame()
    {
        if (mDriftingCP.Count < mDifficulty.MaxDriftCount)
        {
            SelectNextDrifter();
        }
        /*for (int i = 0; i < ControlPoints.Length; i++)
        {
            if (ControlPoints[i].gameObject != null)
            {
                ControlPoints[i].OnFrame();
            }
        }*/
    }

    #endregion


    #region Public Methods

    public void SetDifficulty(Doug.Difficulty Difficulty)
    {
        mDifficulty.MaxDriftCount = Difficulty.MaxDriftCount;
        mDifficulty.MinDriftDelay = Difficulty.MinDriftDelay;
        mDifficulty.MaxDriftDelay = Difficulty.MaxDriftDelay;
        mDifficulty.MaxDriftTime = Difficulty.MaxDriftTime;
    }

    #endregion


    #region Private Methods
    
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

    private void AddInfluence(ControlPoint CP)
    {
        CP.Mobilize();
        for(int i = 0; i < CP.Children.Length; i++)
        {
            ControlPoint Next = CP.Children[i];
            if (!Next.IsMobile())
            {
                AddInfluence(Next);
            }
        }
    }

    private void RemoveInfluence(ControlPoint CP)
    {
        CP.Immobilize();
        for (int i = 0; i < CP.Children.Length; i++)
        {
            ControlPoint Next = CP.Children[i];
            if (Next.IsMobile())
            {
                RemoveInfluence(Next);
            }
        }
    }

    private void RemoveFromDriftingList(ControlPoint ToStableCP)
    {
        if (mDriftingCP.Contains(ToStableCP))
        {
            mDriftingCP.Remove(ToStableCP);
            mPreviousCP = ToStableCP;
            mStableCP.Add(ToStableCP);
            ToStableCP.DragCollider.enabled = false;
        }
        RemoveInfluence(ToStableCP);
    }

    private void SelectNextDrifter()
    {
        if (mPreviousCP != null)
        {
            mStableCP.Remove(mPreviousCP);
        }

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

        if (mPreviousCP != null)
        {
            mStableCP.Add(mPreviousCP);
        }

        int index;
        for(index = 0; index < mDriftingCP.Count; index++)
        {
            if(ToDriftCP.Influence > mDriftingCP[index].Influence)
            {
                break;
            }
        }
        mDriftingCP.Insert(index, ToDriftCP);

        float RandomDelay = Random.Range(mDifficulty.MinDriftDelay, mDifficulty.MaxDriftDelay);
        ToDriftCP.Destabilize(RandomDelay, mDifficulty.MaxDriftTime);
    }

    #endregion
}

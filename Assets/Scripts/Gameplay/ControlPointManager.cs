using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPointManager : MonoBehaviour, IGameLoop
{
    #region Editor

    public bool ShowVisualDebug;
    public ControlPoint[] ControlPoints;
    public ControlPoint DebugCP;

    #endregion


    #region Properties
    
    public DifficultyLevel.ControlPoint Difficulty { set { mDifficulty = value; } }
    private DifficultyLevel.ControlPoint mDifficulty;

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
                if(ShowVisualDebug)
                {
                    CP.ShowVisualDebug = true;
                }
                CP.OnCreate();
                CP.SetCallbacks
                (() =>
                {
                    Debug.LogWarning("Move Callback... " + CP.gameObject.name);
                    AddInfluence(CP);
                },
                () =>
                {
                    Debug.LogWarning("Stop Callback... " + CP.gameObject.name);
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
                if (ControlPoints[i].AlwaysDraggable)
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

    public void SetDifficulty(DifficultyLevel.ControlPoint Difficulty)
    {
        mDifficulty = Difficulty;
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
                if (!Next.IsDrifting)
                {
                    RemoveInfluence(Next);
                }
                else if(CP.AlwaysDraggable)
                {
                    //AddInfluence(Next);
                    CP.Mobilize();
                }
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

       // AddInfluence(ToDriftCP);
    }

    #endregion
}

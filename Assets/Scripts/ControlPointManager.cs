using System.Collections;
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
        public float MaxDriftTime;
    }
    public Difficulty[] Difficulties;

    public ControlPoint[] ControlPoints;

    
    private int mDifficultyLevel;
    private Difficulty mDifficulty;

    private List<ControlPoint> mStableCP;
    private List<ControlPoint> mDriftingCP;
    private ControlPoint mPreviousCP;


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
        mDifficulty = Difficulties[0];
        mDifficultyLevel = 1;

        ResetControlNodes();

        for (int i = 0; i < ControlPoints.Length; i++)
        {
            if (ControlPoints[i] != null)
            {
                ControlPoints[i].OnGameBegin();
                if (ControlPoints[i].Draggable)
                {
                    //ControlPoints[i].SetRigidBodyType(RigidbodyType2D.Dynamic);
                    AddInfluence(ControlPoints[i]);
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

    public void AddInfluence(ControlPoint CP)
    {
        //CP.DragCollider.enabled = true;
        for (int i = 0; i < mStableCP.Count; i++)
        {
            if ( mStableCP[i].Influence <= CP.Influence)
            {
                mStableCP[i].SetRigidBodyType(RigidbodyType2D.Dynamic);
            }
        }
    }

    public void RemoveInfluence(ControlPoint CP)
    {
        //CP.DragCollider.enabled = false;
        CP.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        CP.GetComponent<Rigidbody2D>().angularVelocity = 0;
        CP.SetRigidBodyType(RigidbodyType2D.Kinematic);
        for (int i = 0; i < mStableCP.Count; i++)
        {
            if (mDriftingCP.Count == 0 || mStableCP[i].Influence >= mDriftingCP[0].Influence)
            {
                mStableCP[i].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                mStableCP[i].GetComponent<Rigidbody2D>().angularVelocity = 0;
                mStableCP[i].SetRigidBodyType(RigidbodyType2D.Kinematic);
            }
        }
    }

    void RemoveFromDriftingList(ControlPoint ToStableCP)
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

    

}

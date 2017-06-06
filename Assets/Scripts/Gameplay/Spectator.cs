using System.Collections.Generic;
using UnityEngine;

public abstract class Spectator : MonoBehaviour, IGameLoop
{
    #region Editor

    public int CredValue;

    [Range(0.0f, 1.0f)]
    public float SpawnChance;

    public enum SpectatorState { Default, Absent, Entering, Exiting, Watching, Distracted };
    public float UpdatePeriod;

    [Range(0.0f, 1.0f)]
    public float EnterChance;

    [Range(0.0f, 1.0f)]
    public float ExitChance;

    [Range(0.0f, 1.0f)]
    public float DistractChance;

    [Range(0.0f, 1.0f)]
    public float FocusChance;

    public float CylinderRadius;
    public float MovementSpeed;

    #endregion


    #region Properties

    private bool bCanUpdate;
    private float mTimeTillUpdate;

    private Renderer[] mRenderers;
    private SpectatorState mState;
    private int mDirection;
    private float mDestinationX;

    private List<System.Action> mStateChangeCallbacks;
    //private System.Action mEnterCallback;
   // private System.Action mExitCallback;

    #endregion


    #region IGameLoop

    public virtual void OnCreate()
    {
        mRenderers = GetComponentsInChildren<Renderer>();
    }

    public virtual void OnGameBegin()
    {
        if (IsVisible())
        {
            EnterState(SpectatorState.Watching);
        }
        else
        {
            EnterState(SpectatorState.Absent);
        }
    }

    public virtual void OnFrame()
    {   
        ProcessCurrentState();
    }

    #endregion


    #region Public Methods
    
    public void SetVisibility(bool IsVisible)
    {
        if(mRenderers == null)
        {
            return;
        }
        
        for (int i = 0; i < mRenderers.Length; i++)
        {
            mRenderers[i].enabled = IsVisible;
        }
    }

    public bool IsVisible()
    {
        if (mRenderers == null)
        {
            return false;
        }

        return mRenderers[0].enabled;
    }

    public bool CanAffectCred()
    {
        return mState == SpectatorState.Watching;
    }

    public void SetCallbacks(System.Action Absent, System.Action Entering, System.Action Exiting, System.Action Watching, System.Action Distracted)
    {
        mStateChangeCallbacks = new List<System.Action>();

        mStateChangeCallbacks.Add(Absent);
        mStateChangeCallbacks.Add(Entering);
        mStateChangeCallbacks.Add(Exiting);
        mStateChangeCallbacks.Add(Watching);
        mStateChangeCallbacks.Add(Distracted);
    }

    public virtual void EnterRoom(float DestinationX)
    {
        mDestinationX = DestinationX;
        EnterState(SpectatorState.Entering);
    }

    public virtual void ExitRoom(float DestinationX)
    {
        mDestinationX = DestinationX * Mathf.Sign(transform.position.x);
        EnterState(SpectatorState.Exiting);
    }

    #endregion


    #region Private Methods

    protected virtual void EnterState(SpectatorState EntryState)
    {
        ExitCurrentState();

        switch (EntryState)
        {
            case SpectatorState.Absent:
                break;
            case SpectatorState.Entering:
                SetVisibility(true);
                mDirection = (int)Mathf.Sign(-transform.position.x);
                break;
            case SpectatorState.Exiting:
                mDirection = (int)Mathf.Sign(transform.position.x);
                break;
            case SpectatorState.Watching:
                StateChangeCallback(SpectatorState.Watching);
                break;
            case SpectatorState.Distracted:
                StateChangeCallback(SpectatorState.Distracted);
                break;
        }

        mState = EntryState;
    }

    protected virtual void ExitCurrentState()
    {
        switch (mState)
        {
            case SpectatorState.Default:
                mTimeTillUpdate = UpdatePeriod;
                break;
            case SpectatorState.Absent:
                break;
            case SpectatorState.Entering:
                break;
            case SpectatorState.Exiting:
                SetVisibility(false);
                break;
            case SpectatorState.Watching:
                break;
            case SpectatorState.Distracted:
                break;
        }
    }

    protected virtual void ProcessCurrentState()
    {
        switch (mState)
        {
            case SpectatorState.Default:                
                break;
            case SpectatorState.Absent:
                if (TestChance(EnterChance))
                {
                    StateChangeCallback(SpectatorState.Entering);
                }
                break;
            case SpectatorState.Entering:
                if (MoveTowardDestination())
                {
                    EnterState(SpectatorState.Watching);
                }
                break;
            case SpectatorState.Exiting:
                if (MoveTowardDestination())
                {
                    EnterState(SpectatorState.Absent);
                }
                break;
            case SpectatorState.Watching:
                if (TestChance(DistractChance))
                {
                    EnterState(SpectatorState.Distracted);
                }
                if (TestChance(ExitChance))
                {
                    StateChangeCallback(SpectatorState.Exiting);
                }                
                break;
            case SpectatorState.Distracted:
                if (TestChance(FocusChance))
                {
                    EnterState(SpectatorState.Watching);
                }
                break;
        }
    }

    private void StateChangeCallback(SpectatorState NextState)
    {
        if(mStateChangeCallbacks.Count >= (int)NextState)
        {
            mStateChangeCallbacks[(int)NextState - 1]();
        }
    }

    private bool TestChance(float Probability)
    {
        mTimeTillUpdate -= Time.deltaTime;
        if (mTimeTillUpdate <= 0)
        {
            mTimeTillUpdate = 2.0f * UpdatePeriod - mTimeTillUpdate;
            return Random.Range(0, 1.0f) <= Probability;
        }
        return false;
    }

    private bool MoveTowardDestination()
    {
        transform.position += mDirection * MovementSpeed * Time.deltaTime * Vector3.right;
        return (transform.position.x - mDestinationX) * mDirection >= 0;
    }

    #endregion
}

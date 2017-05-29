using UnityEngine;

public class Spectator : MonoBehaviour, IGameLoop
{
    #region Editor

    [Range(0.0f, 1.0f)]
    public float SpawnRate;

    public enum SpectatorState { Entering, Exiting, Watching, Distracted };
    public float MovementSpeed;
    public float EnterChance;

    #endregion


    #region Properties

    private SpectatorState mState;
    private int mDirection;
    private float mDestinationX;

    #endregion


    #region IGameLoop

    public void OnCreate()
    {
        
    }

    public void OnGameBegin()
    {
        EnterState(SpectatorState.Watching);
    }

    public void OnFrame()
    {
        ProcessCurrentState();       
    }

    #endregion


    #region Public Methods

    public void ExitRoom(float DestinationX)
    {
        mDestinationX = DestinationX;
        EnterState(SpectatorState.Exiting);
    }

    public void EnterRoom(float DestinationX)
    {
        mDestinationX = DestinationX;
        EnterState(SpectatorState.Entering);
    }

    #endregion


    #region Private Methods

    private void EnterState(SpectatorState EntryState)
    {
        ExitCurrentState();

        switch (EntryState)
        {
            case SpectatorState.Entering:
                break;
            case SpectatorState.Exiting:
                mDirection = (int)Mathf.Sign(transform.position.x);
                break;
            case SpectatorState.Watching:
                break;
            case SpectatorState.Distracted:
                break;
        }

        mState = EntryState;
    }

    private void ExitCurrentState()
    {
        switch (mState)
        {
            case SpectatorState.Entering:
                break;
            case SpectatorState.Exiting:
                break;
            case SpectatorState.Watching:
                break;
            case SpectatorState.Distracted:
                break;
        }
    }

    private void ProcessCurrentState()
    {
        switch (mState)
        {
            case SpectatorState.Entering:
                break;
            case SpectatorState.Exiting:
                transform.position += mDirection * MovementSpeed * Time.deltaTime * Vector3.right;
                break;
            case SpectatorState.Watching:
                float Rand = Random.Range(0, 1000.0f);
                if (Rand > 990)
                {
                    //ExitRoom();
                }
                break;
            case SpectatorState.Distracted:
                break;
        }
    }

    #endregion
}

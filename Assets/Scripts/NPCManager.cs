using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour, IGameLoop
{
    #region Editor
    
    public int AbsoluteMaxSpectators;
    public Spectator[] SpectatorTypes;

    /********************
     * SpawnPointMin:
     * x: Max horizontal distance from center at MIN z-coordinate
     * y: Min height
     * z: Min depth
    *********************/
    public Vector3 SpawnPointMin;

    /********************
     * SpawnPointMax:
     * x: Max horizontal distance from center at MAX z-coordinate
     * y: Max height
     * z: Max depth
    *********************/
    public Vector3 SpawnPointMax;

    #endregion


    #region Properties
    
    private Difficulty.NPC mDifficulty;
    private float mSpawnRateSum;
    private List<Spectator> mSpectators;
    private static float mYZPerspectiveRatio;
    private static float mYXPerspectiveRatio;

    #endregion


    #region IGameLoop

    public void OnCreate()
    {
        ComputeRandomRange();
    }

    public void OnGameBegin()
    {
        InitialSpawn();
        for(int i = 0; i < mSpectators.Count; i++)
        {
            mSpectators[i].OnGameBegin();
        }
    }

    public void OnFrame()
    {
        for (int i = 0; i < mSpectators.Count; i++)
        {
            mSpectators[i].OnFrame();
        }
    }

    #endregion


    #region Public Methods

    public float GetZFromY(float y)
    {
        float z = SpawnPointMin.z + mYZPerspectiveRatio * y;
        return z;
    }

    public void SetDifficulty(Difficulty.NPC Difficulty)
    {
        mDifficulty.MaxSpectators = Difficulty.MaxSpectators;
    }

    #endregion



    #region Private Methods
    
    private void ComputeRandomRange()
    {
        mSpawnRateSum = 0;
        for (int i = 0; i < SpectatorTypes.Length; i++)
        {
            mSpawnRateSum += SpectatorTypes[i].SpawnRate;
        }
    }

    

    private float GetRandomXFromY(float y)
    {
        float XMax = SpawnPointMin.x + mYXPerspectiveRatio * y;
        float RandomX = Random.Range(0, XMax);
        float XComplement = SpawnPointMax.x - Mathf.Abs(RandomX);
        float ExpBase = XComplement / SpawnPointMax.x;
        float Distribution = Mathf.Pow(ExpBase, 2.0f);
        float RandomPower = Mathf.Pow(-1, (int)Random.Range(0, 2));
        RandomX = RandomPower* Distribution *SpawnPointMax.x;

        return RandomX;
    }

    private Spectator RandomType()
    {
        float RandomInterval = Random.Range(0, mSpawnRateSum);
        for (int j = 0; j < SpectatorTypes.Length; j++)
        {
            if (RandomInterval < SpectatorTypes[j].SpawnRate)
            {
                return SpectatorTypes[j];
            }
            else
            {
                RandomInterval -= mSpawnRateSum;
            }
        }
        return null;
    }

    private Vector3 RandomPosition()
    {
        float RandomY = Random.Range(SpawnPointMin.y, SpawnPointMax.y);
        float RandomX = GetRandomXFromY(RandomY);
        float ZDepth = GetZFromY(RandomY);
        return new Vector3(RandomX, RandomY, ZDepth);
    }

    private void SpawnAtPosition(Spectator Type, Vector3 Position)
    {
        Spectator Spawn = Instantiate(Type, transform);
        Spawn.transform.position = Position;
        mSpectators.Add(Spawn);
    }

    private void InitialSpawn()
    {
        float yDiff = SpawnPointMax.y - SpawnPointMin.y;
        if (AbsoluteMaxSpectators <= 0 || yDiff <= 0)
        {
            return;
        }

        mSpectators = new List<Spectator>(AbsoluteMaxSpectators);

        mYZPerspectiveRatio = (SpawnPointMax.z - SpawnPointMin.z) / yDiff;
        mYXPerspectiveRatio = (SpawnPointMax.x - SpawnPointMin.x) / yDiff;

        for (int i = 0; i < mDifficulty.MaxSpectators; i++)
        {
            SpawnAtPosition(RandomType(), RandomPosition());
        }

        for (int i = mDifficulty.MaxSpectators; i < AbsoluteMaxSpectators; i++)
        {
            SpawnAtPosition(RandomType(), transform.position);   
        }
    }

    #endregion
}

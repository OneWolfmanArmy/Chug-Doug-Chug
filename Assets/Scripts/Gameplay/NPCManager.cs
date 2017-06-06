using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour, IGameLoop
{
    #region Editor
    
    public int AbsoluteMaxSpectators;
    public Spectator[] SpectatorTypes;
    public Vector2 CellSize;

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
    
    private DifficultyLevel.NPC mDifficulty;
    private float mSpawnChanceSum;
    private List<Spectator> mSpectators;
    private Vector3 mFrustumGradient;
    private float mSpectatorCredSum;

    SpectatorGrid mSpectatorGrid;

    #endregion


    #region IGameLoop

    public void OnCreate()
    {
        mFrustumGradient = SpawnPointMax - SpawnPointMin;
        if(mFrustumGradient.z != 0)
        {
            mFrustumGradient /= mFrustumGradient.z;
        }

        mSpectatorGrid = SpectatorGrid.CreateGrid(new Trapezoid(SpawnPointMin.x, SpawnPointMax.x, SpawnPointMin.z, SpawnPointMax.z), CellSize);
        mSpectators = new List<Spectator>(AbsoluteMaxSpectators);
        ComputeSpawnChanceSum();
    }

    public void OnGameBegin()
    {
        Spectator[] OldSpectators = GetComponentsInChildren<Spectator>();
        for(int i = 0; i < OldSpectators.Length; i++)
        {
            Destroy(OldSpectators[i].gameObject);
        }

        InitialSpawn();

        for (int i = 0; i < mSpectators.Count; i++)
        {
            Spectator S = mSpectators[i];
            S.SetCallbacks
            (
                ()=> { },
                ()=> { OnSpectatorEnter(S); },
                ()=> { OnSpectatorExit(S); },
                ()=> { OnSpectatorBeginWatching(S); },
                ()=> { OnSpectatorDistracted(S); }
            );
        }

        for (int i = 0; i < mSpectators.Count; i++)
        {
            mSpectators[i].OnGameBegin();
        }
    }

    public void OnFrame()
    {
        for (int i = 0; i < mSpectators.Count; i++)
        {
            if (mSpectators[i] != null)
            {
                mSpectators[i].OnFrame();
            }
        }

        SpectatorGrid.DrawGrid(mSpectatorGrid, CellSize);
    }

    #endregion


    #region Public Methods
    
    public void SetDifficulty(DifficultyLevel.NPC Difficulty)
    {
        mDifficulty.MaxSpectators = Difficulty.MaxSpectators;
    }

    #endregion


    #region Private Methods
    
    private void ComputeSpawnChanceSum()
    {
        mSpawnChanceSum = 0;
        for (int i = 0; i < SpectatorTypes.Length; i++)
        {
            mSpawnChanceSum += SpectatorTypes[i].SpawnChance;
        }
    }        

    //private float GetRandomXFromY(float y)
    //{
    //    float XMax = SpawnPointMin.x + mXYPerspectiveRatio * y;
    //    float RandomX = Random.Range(0, XMax);
    //    float XComplement = SpawnPointMax.x - Mathf.Abs(RandomX);
    //    float ExpBase = XComplement / SpawnPointMax.x;
    //    float Distribution = Mathf.Pow(ExpBase, 2.0f);
    //    float RandomPower = Mathf.Pow(-1, (int)Random.Range(0, 2));
    //    RandomX = RandomPower * Distribution * SpawnPointMax.x;
    //    return RandomX;
    //}

    private Spectator RandomType()
    {
        float RandomInterval = Random.Range(0, mSpawnChanceSum);
        for (int j = 0; j < SpectatorTypes.Length; j++)
        {
            if (RandomInterval <= SpectatorTypes[j].SpawnChance)
            {
                return SpectatorTypes[j];
            }
            else
            {
                RandomInterval -= SpectatorTypes[j].SpawnChance;
            }
        }
        return null;
    }

    //private Vector3 RandomPosition()
    //{
    //    float RandomY = Random.Range(SpawnPointMin.y, SpawnPointMax.y);
    //    float RandomX = GetRandomXFromY(RandomY);
    //    float ZDepth = GetZFromY(RandomY);
    //    return new Vector3(RandomX, RandomY, ZDepth);
    //}

    //private bool CheckSpace(Vector3 Center, float Radius1)
    //{
    //    float RadiusSum = Radius1;

    //    for(int i = 0; i < mSpectators.Count - 1; i++)
    //    {
    //        Spectator Other = mSpectators[i];
    //        RadiusSum = Radius1 + Other.CylinderRadius;

    //        float XDist = Center.x - Other.transform.position.x;
    //        float ZDist = Center.z - Other.transform.position.z;
    //        float SqrDistance = XDist * XDist + ZDist * ZDist;
    //        if (SqrDistance != 0 && SqrDistance <= RadiusSum * RadiusSum)
    //        {                
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    private Vector3 ConvertGridPosition(Vector3 Position)
    {
        return new Vector3(Position.x, SpawnPointMin.y + (Position.z - SpawnPointMin.z) * mFrustumGradient.y, Position.z);
    }

    private void SpawnAtPosition(Spectator Type, Vector3 Position)
    {
        Spectator Spawn = Instantiate(Type, transform);
        Spawn.transform.position = Position;
        Spawn.OnCreate();
        mSpectators.Add(Spawn);
    }

    private void SpawnSpectator(Spectator Type)
    {
        Spectator Spawn = Instantiate(Type, transform);
        Spawn.OnCreate();
        mSpectators.Add(Spawn);
    }

    private void InitialSpawn()
    {
        if (AbsoluteMaxSpectators <= 0)
        {
            return;
        }

        mSpectatorCredSum = 0;

        Spectator LastSpectator;
        for (int i = 0; i < mDifficulty.MaxSpectators; i++)
        {
            SpawnSpectator(RandomType());
            LastSpectator = mSpectators[i];

            Vector3 Pos = mSpectatorGrid.GetRandomCellPosition();
            if (mSpectatorGrid.OccupyCellAtPosition(Pos))
            {
                LastSpectator.transform.position = ConvertGridPosition(Pos);
                LastSpectator.SetVisibility(true);
            }            
        }

        for (int i = mDifficulty.MaxSpectators; i < AbsoluteMaxSpectators; i++)
        {
            SpawnSpectator(RandomType());
            mSpectators[i].transform.position = new Vector3(SpawnPointMax.x * 2.0f, SpawnPointMax.y, SpawnPointMax.z);
            mSpectators[i].SetVisibility(false);
        }
    }
    #endregion


    #region Spectator Callbacks

    private void OnSpectatorEnter(Spectator S)
    {
        Vector3 Pos = mSpectatorGrid.GetRandomCellPosition();
        if (mSpectatorGrid.OccupyCellAtPosition(Pos))
        {
            S.transform.position = new Vector3(S.transform.position.x, ConvertGridPosition(Pos).y, Pos.z);
            S.EnterRoom(Pos.x);
        }
    }

    private void OnSpectatorExit(Spectator S)
    {
        if (mSpectatorGrid.FreeCellAtPosition(S.transform.position))
        {
            S.ExitRoom(SpawnPointMax.x * 1.5f);
        }
    }

    private void OnSpectatorBeginWatching(Spectator S)
    {
        mSpectatorCredSum += S.CredValue;
        UIManager.Instance.CredMultiplier.text = mSpectatorCredSum.ToString();
    }

    private void OnSpectatorDistracted(Spectator S)
    {
        mSpectatorCredSum -= S.CredValue;
        UIManager.Instance.CredMultiplier.text = mSpectatorCredSum.ToString();
    }

    #endregion
}

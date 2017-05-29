using UnityEngine;

[System.Serializable]
public struct Difficulty
{
    [System.Serializable]
    public struct Score
    {
        public int Target;
    }
    public Score ScoreDifficulty;

    [System.Serializable]
    public struct ControlPoint
    {
        public int MaxDriftCount;
        public float MinDriftDelay;
        public float MaxDriftDelay;
        public float MaxDriftTime;
    }
    public ControlPoint CPDifficulty;

    [System.Serializable]
    public struct NPC
    {
        public int MaxSpectators;
    }
    public NPC NPCDifficulty;
}

public class GameLoopManager : MonoBehaviour, IGameLoop
{
    #region Editor

    public ScoreManager ScoreController;
    public Doug DougController;
    public NPCManager NPCController;

    public float InitialDelay;

    public Difficulty[] Difficulties;

    #endregion


    #region Properties

    private int mDifficultyLevel;

    #endregion


    #region IGameLoop

    public void OnCreate()
    {
        ScoreController.OnCreate();
        DougController.OnCreate();
        NPCController.OnCreate();
    }

    public void OnGameBegin()
    {
        SetDifficulty(0);
        ScoreController.OnGameBegin();
        DougController.OnGameBegin();
        NPCController.OnGameBegin();

    }

    public void OnFrame()
    {
        if (mDifficultyLevel < (Difficulties.Length - 1) && CanIncreaseDifficulty())
        {
            IncreaseDifficulty();
        }

        if (DougController.DrinkingDistance())
        {
            ScoreController.IncrementIntoxication();
            ScoreController.IncrementCred();
            ScoreController.IncrementScore(ScoreController.DrinkScoreMultiplier);
        }
        else
        {
            ScoreController.DecrementIntoxication();
            ScoreController.DecrementCred();
        }
        ScoreController.OnFrame();

        DougController.OnFrame();

        NPCController.OnFrame();
    }

    #endregion


    #region Public Methods

    

    #endregion


    #region Private Methods

    private bool CanIncreaseDifficulty()
    {
        return ScoreController.CanIncreaseDifficulty();
    }
    
    private void SetDifficulty(int Level)
    {
        mDifficultyLevel = Level;
        Difficulty CurrentDifficulty = Difficulties[mDifficultyLevel];
        ScoreController.SetDifficulty(CurrentDifficulty.ScoreDifficulty);
        DougController.SetDifficulty(CurrentDifficulty.CPDifficulty);
        NPCController.SetDifficulty(CurrentDifficulty.NPCDifficulty);
    }

    private void IncreaseDifficulty()
    {
        SetDifficulty(mDifficultyLevel + 1);
    }

    #endregion
}

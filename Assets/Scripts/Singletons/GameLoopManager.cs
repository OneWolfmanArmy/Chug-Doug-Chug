using UnityEngine;


public class GameLoopManager : Singleton<GameLoopManager>, IGameLoop
{   
    #region Editor

    public ScoreManager ScoreController;
    public Doug DougController;
    public NPCManager NPCController;

    public float InitialDelay;

    public DifficultyLevel[] Difficulties;

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

        if (DougController.IsDrinking)
        {
            ScoreController.IncrementIntoxication();
            ScoreController.IncrementCred(NPCController.SpectatorCredSum);
            ScoreController.IncrementScore(ScoreController.DrinkScoreMultiplier);
        }
        else
        {
            ScoreController.DecrementIntoxication();
            ScoreController.DecrementCred(NPCController.SpectatorCredSum);
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
        DifficultyLevel CurrentDifficulty = Difficulties[mDifficultyLevel];
        ScoreController.Difficulty = CurrentDifficulty.ScoreDifficulty;
        DougController.SetControlPointDifficulty(CurrentDifficulty.CPDifficulty);
        NPCController.Difficulty = CurrentDifficulty.NPCDifficulty;
    }

    private void IncreaseDifficulty()
    {
        SetDifficulty(mDifficultyLevel + 1);
    }

    #endregion
}

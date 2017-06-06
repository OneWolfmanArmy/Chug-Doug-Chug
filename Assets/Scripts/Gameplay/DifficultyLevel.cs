using UnityEngine;

[System.Serializable]
public struct DifficultyLevel
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
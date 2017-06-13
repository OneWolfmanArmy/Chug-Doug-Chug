using UnityEngine;

public class Doug : MonoBehaviour, IGameLoop
{
    #region Editor

    public ControlPointManager CPManager;

    public Transform BeerSrc;
    public Transform BeerDst;

    public float MinDrinkingDistance;
    public bool IsDrinking { get { return bDrinking; } }
    private bool bDrinking;

    #endregion


    #region Properties

    #endregion


    #region IGameLoop

    public void OnCreate()
    {
        MinDrinkingDistance *= MinDrinkingDistance;
        
        CPManager.OnCreate();
    }

    public void OnGameBegin()
    {
        CPManager.OnGameBegin();
    }    

    public void OnFrame()
    {
        bool Drinking = DrinkingDistance();
        if(bDrinking ^ Drinking)
        {
            if (!bDrinking)
            {
                AudioManager.Instance.PlaySoundEffect(gameObject, "Gulp");
            }
            else
            {
                AudioManager.Instance.StopSFX(gameObject);
            }
        }

        bDrinking = Drinking;
          
        CPManager.OnFrame();
    }

    #endregion
    

    #region Public Methods

    public void SetControlPointDifficulty(DifficultyLevel.ControlPoint Difficulty)
    {
        CPManager.Difficulty = Difficulty;
    }

    #endregion


    #region Private Methods

    private bool DrinkingDistance()
    {
        return Vector2.SqrMagnitude(BeerDst.position - BeerSrc.position) <= MinDrinkingDistance;
    }

    #endregion
}

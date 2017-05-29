using UnityEngine;

public class Doug : MonoBehaviour, IGameLoop
{
    #region Editor
    
    public ControlPointManager CPManager;

    public Transform BeerSrc;
    public Transform BeerDst;

    public float MinDrinkingDistance;

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
        CPManager.OnFrame();
    }

    #endregion


    #region Public Methods

    public void SetDifficulty(Difficulty.ControlPoint Difficulty)
    {
        CPManager.SetDifficulty(Difficulty);
    }

    public bool DrinkingDistance()
    {
        return Vector2.SqrMagnitude(BeerDst.position - BeerSrc.position) <= MinDrinkingDistance;
    }

    #endregion


    #region Private Methods

    #endregion
}

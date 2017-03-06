using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    #region Editor
    
    public Text PlayOption;
    public Text HighScoresOption;

    #endregion


    #region UI

    public void Play()
    {
        GameState.Instance.EnterMainGame();        
    }

    #endregion
}

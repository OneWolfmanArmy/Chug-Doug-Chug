using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doug : MonoBehaviour
{
    public ScoreManager ScoreBoard;

    private bool bDrinking;

    void Start()
    {
        StartDrinking();
    }

    void Update()
    {
        if(bDrinking)
        {
            ScoreBoard.IncrementIntoxication();
        }
    }

    public void StartDrinking()
    {
        bDrinking = true;
    }

    public void StopDrinking()
    {
        bDrinking = false;
    }


}

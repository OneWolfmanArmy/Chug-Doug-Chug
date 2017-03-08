using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doug : MonoBehaviour
{
    public ScoreManager Score;

    private bool bDrinking;

    void Start()
    {
        StartDrinking();
    }

    void Update()
    {
        if(bDrinking)
        {
            Score.IncrementIntoxication();
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

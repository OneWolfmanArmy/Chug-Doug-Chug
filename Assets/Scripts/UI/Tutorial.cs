using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Tutorial : MonoBehaviour
{
    #region Editor

    public Doug DougScript;
    public RectTransform Pointer;

    [System.Serializable]
    public struct Step
    {
        public RectTransform TextBox;
        public Vector3 PointerPos;
        public float PointerRot;
        public UnityEvent Callback;
    }
    public Step[] Steps;
    
    #endregion


    #region Properties

    private int mCurrentStep;

    #endregion


    #region Public Methods

    public void BeginTutorial()
    {
        mCurrentStep = 0;
        DisplayNextStep();
    }

    public void NextStep()
    {
        Step S = Steps[mCurrentStep];
        HidePrevStep();
        mCurrentStep++;

        if (mCurrentStep < Steps.Length)
        {
            DisplayNextStep();
            if(S.Callback != null)
            {
                S.Callback.Invoke();
            }
        }
        else
        {
            ExitTutorial();
        }
    }

    public void ExitTutorial()
    {
        GameState.Instance.BeginPlay();
    }

    #endregion


    #region Callbacks

    public void PromptNozzleMovement(ControlPoint CP)
    {
       // DougScript.CPManager.AddInfluence(CP);
        StartCoroutine(WaitForStepCompletion(() => 
        {
            return DougScript.IsDrinking;
        }, 
        () =>
        {
            //DougScript.CPManager.RemoveInfluence(CP);
        }
        ));
    }

    #endregion
    

    #region Private Methods

    private void HidePrevStep()
    {
        if (Steps[mCurrentStep].TextBox != null)
        {
            Steps[mCurrentStep].TextBox.gameObject.SetActive(false);
        }
    }

    private void DisplayNextStep()
    {
        Step S = Steps[mCurrentStep];
        if (S.TextBox == null)
        {
            return;
        }

        S.TextBox.gameObject.SetActive(true);        
        Pointer.localPosition = Steps[mCurrentStep].PointerPos;
        Pointer.rotation = Quaternion.Euler(Vector3.forward * S.PointerRot);
        Pointer.localScale = new Vector3(-Mathf.Sign(Mathf.Sin(S.PointerRot)), 1, 1);
    }    

    private IEnumerator WaitForStepCompletion(Func<bool> Requisite, Action Callback)
    {
        if(!Requisite())
        {
            yield return null;
            StartCoroutine(WaitForStepCompletion(Requisite, Callback));
        }
        else
        {
            yield return null;
            Callback();
            NextStep();
        }        
    }

    #endregion
}

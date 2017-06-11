using DG.Tweening;
using UnityEngine;

public class TweenBehavior : MonoBehaviour
{
    public Vector3 PunchScale;
    public float Duration;
    [Range(0, 10)]
    public int Vibrato;
    [Range(0, 1)]
    public float Elasticity;

    private bool bInProgress;

    public void DOPunchScale()
    {
        if (!bInProgress)
        {
            bInProgress = true;
            Tweener T = transform.DOPunchScale(PunchScale, Duration, Vibrato, Elasticity);
            T.OnComplete(() => { bInProgress = false; });
        }
    }


}

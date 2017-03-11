using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Image mFrame;
    private Image mFilling;

    void Awake()
    {
        mFrame = transform.GetChild(0).GetComponent<Image>();
        mFilling = transform.GetChild(1).GetComponent<Image>();
    }

    public void ResizeFilling(float Ratio)
    {
        if(mFrame == null || mFilling == null)
        {
            return;
        }

        mFilling.rectTransform.sizeDelta = Vector2.Scale(mFrame.rectTransform.sizeDelta, new Vector2(Ratio, 1.0f));
    }
}

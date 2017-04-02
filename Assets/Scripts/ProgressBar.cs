using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image Frame;
    public Image Filling;

    public void ResizeFilling(float Ratio)
    {
        if(Frame == null || Filling == null)
        {
            return;
        }

        Filling.rectTransform.localScale = new Vector2(1.0f, Ratio);
    }
}

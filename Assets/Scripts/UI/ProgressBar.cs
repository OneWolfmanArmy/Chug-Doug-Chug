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
            Debug.Log("Error: Missing reference to Frame or Filling in ProgressBar " + gameObject.name);
            return;
        }

        Filling.rectTransform.localScale = new Vector3(1.0f, Ratio, 1.0f);
    }
}

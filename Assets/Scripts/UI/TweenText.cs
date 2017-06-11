using UnityEngine;
using UnityEngine.UI;

public class TweenText : TweenBehavior
{
    private Text mText;

    void Awake()
    {
        mText = GetComponent<Text>();
    }

    public void SetText(string S)
    {
        mText.text = S;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public void ExitTutorial()
    {
        GameState.Instance.BeginPlay();
        Destroy(gameObject);
    }
}

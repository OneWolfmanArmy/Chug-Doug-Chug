using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShotGlassButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image MouseOverFilling;

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseOverFilling.gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOverFilling.gameObject.SetActive(true);
    }
}

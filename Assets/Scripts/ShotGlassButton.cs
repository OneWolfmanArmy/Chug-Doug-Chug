using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShotGlassButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image MouseOverFilling;

    private void OnEnable()
    {
        MouseOverFilling.gameObject.SetActive(true);
    }
        
    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseOverFilling.gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOverFilling.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MouseOverFilling.gameObject.SetActive(true);
    }
}

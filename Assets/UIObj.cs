using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIObj : MonoBehaviour
 , IPointerClickHandler // 2
 , IDragHandler
 , IPointerEnterHandler
 , IPointerExitHandler {
    public void OnDrag(PointerEventData eventData) {
    }

    // Start is called before the first frame update
    public void OnPointerClick(PointerEventData eventData) {
    }

    public void OnPointerEnter(PointerEventData eventData) {
        MouseDrag.Instance.CanZoom(false);
    }

    public void OnPointerExit(PointerEventData eventData) {
        MouseDrag.Instance.CanZoom(true);
    }
}

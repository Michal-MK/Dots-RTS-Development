using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClearUpgradeSlot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public UpgradeSlot_UI slot;

	public void OnPointerClick(PointerEventData eventData) => slot.ClearSlot();

	public void OnPointerDown(PointerEventData eventData) {	}

	public void OnPointerEnter(PointerEventData eventData) => GetComponent<Image>().color = new Color(1, 1, 1, 1);

	public void OnPointerExit(PointerEventData eventData) => GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
}

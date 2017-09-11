using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClearUpgradeSlot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public delegate void ClickedOnGameObject(UpgradeSlot clicked);
	public static event ClickedOnGameObject OnSlotClear;

	public void OnPointerClick(PointerEventData eventData) {
		if (OnSlotClear != null) {
			OnSlotClear(transform.parent.GetComponent<UpgradeSlot>());
			print("Clearing slot " + transform.parent.name);
		}

	}
	public void OnPointerDown(PointerEventData eventData) {
		//Has to be implemented for some reason.
	}

	public void OnPointerEnter(PointerEventData eventData) {
		GetComponent<Image>().color = new Color(1, 1, 1, 1);
	}

	public void OnPointerExit(PointerEventData eventData) {
		GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
	}
}

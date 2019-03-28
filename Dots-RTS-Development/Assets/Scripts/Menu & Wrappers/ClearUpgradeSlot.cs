using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClearUpgradeSlot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public static event EventHandler<UpgradeSlot> OnSlotClear;

	public void OnPointerClick(PointerEventData eventData) {
		if (OnSlotClear != null) {
			OnSlotClear(this, transform.parent.GetComponent<UpgradeSlot>());
			print($"Clearing slot {transform.parent.name } of {transform.parent.parent.name}");
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

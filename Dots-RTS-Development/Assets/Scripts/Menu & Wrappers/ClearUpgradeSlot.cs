using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClearUpgradeSlot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public delegate void ClickedOnGameObject(UpgradeSlot clicked);
	public static event ClickedOnGameObject OnSlotClear;

	public UpgradeSlot parentSlot;

	public void OnPointerClick(PointerEventData eventData) {
		if (OnSlotClear != null) {
			OnSlotClear(transform.parent.GetComponent<UpgradeSlot>());
			print("Clearing slot " + transform.parent.name + " of " + transform.parent.parent.name);
		}

	}
	public void OnPointerDown(PointerEventData eventData) {
		//Has to be implemented for some reason.
	}

	public void OnPointerEnter(PointerEventData eventData) {
		GetComponent<Image>().color = new Color(1, 1, 1, 1);
	}

	public void OnPointerExit(PointerEventData eventData) {
		if (parentSlot != null) {
			if (parentSlot.type == Upgrade.Upgrades.NONE) {
				GetComponent<Image>().color = new Color(1, 1, 1, 0f);
			}
			else {
				GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
			}
		}
		else {
			if (transform.parent.GetComponent<UpgradeSlot>().type == Upgrade.Upgrades.NONE) {
				GetComponent<Image>().color = new Color(1, 1, 1, 0f);
			}
			else {
				GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlot : MonoBehaviour, IPointerClickHandler {
	public int slot;
	public delegate void OnUpgradeSlotClick(int slot);
	public static event OnUpgradeSlotClick OnSlotClicked;

	void Start() {
        slot = int.Parse(transform.name);
	}

	public void OnPointerClick(PointerEventData eventData) {
		OnSlotClicked?.Invoke(slot);
	}
}

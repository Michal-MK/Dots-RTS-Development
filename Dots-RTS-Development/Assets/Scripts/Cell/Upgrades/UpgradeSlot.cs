using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlot : MonoBehaviour, IPointerClickHandler {
	public int slot;

	// Use this for initialization
	void Start() {
		slot = int.Parse(transform.name);
	}

	public void OnPointerClick(PointerEventData eventData) {

	}
}
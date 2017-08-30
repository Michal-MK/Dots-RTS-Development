using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ControlSchemeHandle : MonoBehaviour, IEndDragHandler, IDragHandler {

	public RectTransform CSTransform;

	public void OnDrag(PointerEventData eventData) {
		float cursorPerCent = eventData.position.y / Screen.height;
		CSTransform.anchorMin = new Vector2(0, cursorPerCent - 0.125f);
		CSTransform.anchorMax = new Vector2(1, cursorPerCent + 0.125f);
	}

	public void OnEndDrag(PointerEventData eventData) {
		float cursorPerCent = eventData.position.y / Screen.height;
		
		if (cursorPerCent < 0.125f) {
			CSTransform.anchorMin = new Vector2(0, 0);
			CSTransform.anchorMax = new Vector2(1, 0.25f);
		}
		else if (cursorPerCent > 0.875f) {
			CSTransform.anchorMin = new Vector2(0, 0.75f);
			CSTransform.anchorMax = new Vector2(1, 1f);
		}

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ControlSchemeHandle : MonoBehaviour, IEndDragHandler, IDragHandler, IPointerClickHandler {

	public RectTransform CSTransform;
	bool hidden;

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
	IEnumerator ExposeORHide(float Rotation) {
		bool moving = true;
		while (moving) {
			yield return new WaitForEndOfFrame();
			CSTransform.Rotate(0, 0, Rotation * Time.deltaTime);
			if (CSTransform.rotation.eulerAngles.z > 90f && Rotation > 0) {
				CSTransform.rotation = Quaternion.Euler(0, 0, 90);
				hidden = true;
				moving = false;
			}
			if (CSTransform.rotation.eulerAngles.z > 90f && Rotation < 0) {
				CSTransform.rotation = Quaternion.Euler(0, 0, 0);
				hidden = false;
				moving = false;
			}
		}
		
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (hidden) {
			StartCoroutine(ExposeORHide(-100));
		}
		else if (eventData.clickCount == 2) {
			StartCoroutine(ExposeORHide(100));
		}
	}
}

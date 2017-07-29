using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamBox : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

	// Use this for initialization
	public TeamSetup myParrent;
	public Vector3 initialPos;
	public int team;
	RectTransform rect;
	public RectTransform panel;
	Vector2 oldMousePos = Vector2.zero;
	bool cease;
	public float r;

	public static GameObject dragged;

	// Update is called once per frame
	public void AllThingsSet() {
		//myParrent = transform.parent.GetComponent<TeamSetup>();
		rect = gameObject.GetComponent<RectTransform>();
		//panel = transform.parent.gameObject.GetComponent<RectTransform>();
		initialPos = transform.position;
	}

	public void OnDrag(PointerEventData eventData) {

		Vector2 mousePos = eventData.position;

		Vector2 mouseDelta = (mousePos - oldMousePos);
		Vector2 mouseOffset = ((Vector2)rect.position - mousePos);

		if (Vector2.Distance(panel.position, mousePos + mouseOffset + mouseDelta) < (r - rect.sizeDelta.x)) {
			rect.position = mousePos + mouseOffset + mouseDelta;
		}
		//print("I am here " + mousePos + " and the gameObjects center is " + rect.position + " so the offset is " + mouseOffset + " so the expected position is " + (mousePos + mouseOffset + mouseDelta));
		oldMousePos = mousePos;

		
		
		//if (rect.position.x + rect.sizeDelta.x> panel.position.x + panel.sizeDelta.x / 2) {
		//	rect.position = new Vector3(panel.position.x + panel.sizeDelta.x / 2 - rect.sizeDelta.x, rect.position.y);
		//	mouseOffset = ((Vector2)rect.position - mousePos);
		//}

		//if (rect.position.x - rect.sizeDelta.x < panel.position.x - panel.sizeDelta.x / 2) {
		//	rect.position = new Vector3(panel.position.x - panel.sizeDelta.x / 2 + rect.sizeDelta.x, rect.position.y);
		//	mouseOffset = ((Vector2)rect.position - mousePos);
		//}

		//if (rect.position.y + rect.sizeDelta.y > panel.position.y + panel.sizeDelta.y / 2) {
		//	rect.position = new Vector3(rect.position.x, panel.position.y + panel.sizeDelta.y / 2 - rect.sizeDelta.y);
		//	mouseOffset = ((Vector2)rect.position - mousePos);
		//}

		//if (rect.position.y - rect.sizeDelta.y < panel.position.y - panel.sizeDelta.y / 2) {
		//	rect.position = new Vector3(rect.position.x, panel.position.y - panel.sizeDelta.y / 2 + rect.sizeDelta.y);
		//	mouseOffset = ((Vector2)rect.position - mousePos);
		//}

	}

	public void OnPointerDown(PointerEventData eventData) {
		oldMousePos = eventData.position;
		dragged = gameObject;
	}

	//public void PointerExit() {
	//	//print("BREXIT");
	//	if (gameObject == dragged) {
	//		transform.position = initialPos;
	//		cease = true;
	//	}
	//}

	public void OnPointerUp(PointerEventData eventData) {
		TeamBox t = eventData.pointerPress.GetComponent<TeamBox>();
		dragged = null;
		myParrent.TeamBoxPosChange(eventData.pointerPress.transform.position, t);
	}
}

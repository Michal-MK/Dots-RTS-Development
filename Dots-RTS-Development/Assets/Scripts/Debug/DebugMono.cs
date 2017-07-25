using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugMono : MonoBehaviour, IPointerClickHandler
	, IDragHandler
	, IPointerEnterHandler
	, IPointerExitHandler {

	SpriteRenderer sprite;
	Image image;
	Color target = Color.red;
	RectTransform rect;
	RectTransform panel;
	LineRenderer line;

	//private void OnMouseOver() {
	//	if (Input.GetMouseButton(0)) {
	//		print("B");
	//		transform.Translate(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10));

	//		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
	//	}
	//}
	void Start() {
		try {
			line = GetComponent<LineRenderer>();
			print("Got");
			RenderLine();
		}
		catch {

		}


		try {
			sprite = GetComponent<SpriteRenderer>();
			image = GetComponent<Image>();
			rect = GetComponent<RectTransform>();
			panel = transform.parent.GetComponent<RectTransform>();
			Vector4 colour = image.color;
			print(colour);
		}
		catch {

		}

		//string[] strings = new string[10] { "b", "g", "d", "w", "k", "p", "c", "z", "u", "a" }; Success
		string[] strings = new string[10] { "b", "G", "d", "w", "k", "P", "C", "z", "u", "a" };


		foreach (string s in strings) {
			print(s);
		}

		for (int j = strings.Length - 1; j > 0; j--) {
			for (int i = 0; i < j; i++) {
				string precedingName = strings[i];
				string folowingName = strings[i + 1];
				int value = string.Compare(precedingName, folowingName, true);
				if (value > 0) {
					string temp = strings[i];

					strings[i] = strings[i + 1];
					strings[i + 1] = temp;
				}
			}
		}
		print("---------------------------");
		foreach (string s in strings) {
			print(s);
		}
	}

	Vector3 oldMousePos = Vector2.zero;
	bool assign = true;

	public void OnDrag(PointerEventData eventData) {
		if (assign) {
			oldMousePos = eventData.position;
			assign = false;
		}

		Vector3 mousePos = eventData.position;

		Vector3 mouseDelta = (mousePos - oldMousePos);
		Vector3 mouseOffset = (rect.position - mousePos);
		target = Color.magenta;

		if (transform.parent.name == "Panel") {
			rect.position = mousePos + mouseOffset + mouseDelta;    //print("I am here " + mousePos + " and the gameObjects center is " + rect.position + " so the offset is " + mouseOffset + " so the expected position is " + (mousePos + mouseOffset + mouseDelta));
		}
		oldMousePos = mousePos;

		if (rect.position.x + rect.sizeDelta.x / 2 > panel.position.x + panel.sizeDelta.x / 2) {
			rect.position = new Vector3(panel.position.x + panel.sizeDelta.x / 2 - rect.sizeDelta.x / 2, rect.position.y);
			mouseOffset = (rect.position - mousePos);
		}

		if (rect.position.x - rect.sizeDelta.x / 2 < panel.position.x - panel.sizeDelta.x / 2) {
			rect.position = new Vector3(panel.position.x - panel.sizeDelta.x / 2 + rect.sizeDelta.x / 2, rect.position.y);
			mouseOffset = (rect.position - mousePos);
		}

		if (rect.position.y + rect.sizeDelta.y / 2 > panel.position.y + panel.sizeDelta.y / 2) {
			rect.position = new Vector3(rect.position.x, panel.position.y + panel.sizeDelta.y / 2 - rect.sizeDelta.x / 2);
			mouseOffset = (rect.position - mousePos);
		}

		if (rect.position.y - rect.sizeDelta.y / 2 < panel.position.y - panel.sizeDelta.y / 2) {
			rect.position = new Vector3(rect.position.x, panel.position.y - panel.sizeDelta.y / 2 + rect.sizeDelta.y / 2);
			mouseOffset = (rect.position - mousePos);
		}

	}

	public void RenderLine() {
		try {
			RectTransform[] children = GameObject.Find("Panel").GetComponentsInChildren<RectTransform>() as RectTransform[];
			line.positionCount = children.Length;
			for (int i = 0; i < line.positionCount; i++) {
				if (children[i].name != "Panel") {
					line.SetPosition(i, Camera.main.ScreenToWorldPoint(children[i].position) + Vector3.forward * 10);
				}
			}
		}
		catch {

		}
	}


	void Update() {
		if (Input.GetMouseButtonUp(0)) {
			assign = true;
			RenderLine();
		}

		try {
			sprite.color = Vector4.MoveTowards(sprite.color, target, Time.deltaTime);
		}
		catch {

		}
		try {
			image.color = Vector4.MoveTowards(image.color, target, Time.deltaTime);
		}
		catch {

		}
	}

	public void OnPointerClick(PointerEventData eventData) // 3
	{
		print("I was clicked");
		target = Color.blue;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		target = Color.green;
	}

	public void OnPointerExit(PointerEventData eventData) {
		print("Exited");
		target = Color.red;
	}
}


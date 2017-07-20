using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DebugMono : MonoBehaviour, IPointerClickHandler
	, IDragHandler
	, IPointerEnterHandler
	, IPointerExitHandler {

	SpriteRenderer sprite;
	Color target = Color.red;

	private void OnMouseOver() {
		if (Input.GetMouseButton(0)) {
			//print("B");
			//transform.Translate(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,10));

			//transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
		}
	}

	public void OnDrag(PointerEventData eventData) {
		print("I'm being dragged!");
		target = Color.magenta;
		transform.position = Camera.main.ScreenToWorldPoint(eventData.position) + new Vector3(0,0,10);
	}




	void Awake() {
		try {
			sprite = GetComponent<SpriteRenderer>();
		}
		catch {

		}
	}

	void Update() {
		try {
			sprite.color = Vector4.MoveTowards(sprite.color, target, Time.deltaTime);
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
		target = Color.red;
	}
}


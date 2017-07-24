using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TeamBox : MonoBehaviour, IDragHandler, IDropHandler, IPointerDownHandler {

	// Use this for initialization
	public TeamSetup myParrent;
	public Vector3 initialPos;
	public int team;

	// Update is called once per frame
	public void AllThingsSet() {
		//myParrent = transform.parent.GetComponent<TeamSetup>();

		initialPos = transform.position;
	}

	public void OnPointerPress() {

	}

	public void OnDrag(PointerEventData eventData) {
		if (eventData.dragging) {
			transform.position = eventData.position;
		}
		
	}

	public void OnDrop(PointerEventData eventData) {
		myParrent.TeamBoxPosChange(eventData.pointerPress.transform.position, eventData.pointerPress.GetComponent<TeamBox>());
		//print(team);
		//transform.position = initialPos;
	}

	public void OnPointerDown(PointerEventData eventData) {
		//print(gameObject.name + "");
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveFromClanBox : MonoBehaviour, IDropHandler {

	// Use this for initialization
	public TeamSetup myParrent;

	public void OnDrop(PointerEventData eventData) {
		myParrent.TeamBoxPosChange(eventData.pointerPress.transform.position, eventData.pointerPress.GetComponent<TeamBox>());
		//print(team);
		//transform.position = initialPos;
	}

}

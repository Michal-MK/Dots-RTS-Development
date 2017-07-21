using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBox : MonoBehaviour {

	// Use this for initialization
	TeamSetup myParrent;
	// Update is called once per frame
	private void Start() {
		myParrent = transform.parent.GetComponent<TeamSetup>();
	}
	void OnMouseOver () {
		if (Input.GetMouseButton(0)) {
			print("xxx");
			transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
		if (Input.GetMouseButtonUp(0)) {
			myParrent.TeamBoxPosChange(transform.position);
		}
	}

}

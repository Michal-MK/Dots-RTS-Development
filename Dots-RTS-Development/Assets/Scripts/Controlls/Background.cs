using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

	private void OnMouseOver() {
		print("A");
		if (Input.GetMouseButtonUp(0)) {
			print("Hello");
			CellBehaviour.ClearSelection();
		}
	}
}

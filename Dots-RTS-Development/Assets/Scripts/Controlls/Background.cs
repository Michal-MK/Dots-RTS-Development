using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

	private void OnMouseOver() {
		if (Input.GetMouseButtonUp(0)) {
			CellBehaviour.ClearSelection();
			foreach (CellBehaviour cell in CellBehaviour.cellsInSelection) {
				cell.UpdateCellInfo();
			}
		}
	}
}

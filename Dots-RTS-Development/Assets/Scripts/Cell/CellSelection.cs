using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSelection : MonoBehaviour {

	public static List<CellScript> cellsInSelection = new List<CellScript>();

	//Checks the list whether the cell is already selected ? removes it : adds it
	public static void ModifySelection(CellScript cell) {
		for (int i = 0; i < cellsInSelection.Count; i++) {
			if (cell == cellsInSelection[i]) {
				cellsInSelection.RemoveAt(i);
				cell.SetSelected();
				print(cellsInSelection.Count);
				return;
			}
		}
		cellsInSelection.Add(cell);
		print(cellsInSelection.Count);
		cell.SetSelected();

	}
}

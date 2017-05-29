using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : CellScript {

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
	//Wrapper for cell atacking
	public static void AttackCell(CellScript target, enmTeam team) {
		if (cellsInSelection.Count != 0) {
			if (team == enmTeam.ENEMY) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].AttackCell(target);
				}
			}else if (team == enmTeam.ALLIED) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].EmpowerCell(target);
				}
			}
		}
		ClearSelection();
	}

	//Resets Cell colour and clears the selection list
	public static void ClearSelection() {
		for (int i = 0; i < cellsInSelection.Count; i++) {
			cellsInSelection[i].isSelected = false;
			cellsInSelection[i].textMesh.color = new Color(1, 1, 1);
		}
		cellsInSelection.Clear();
	}
}

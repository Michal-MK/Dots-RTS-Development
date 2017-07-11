using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControll : MonoBehaviour {
	public delegate void TeamChangeEventHandler(CellBehaviour sender, Cell.enmTeam previous, Cell.enmTeam current);
	
	public delegate void EnteredCellEditMode(EditCell sender);

	public delegate void PanelValueChanged();
	public delegate void EditModeChanged(LevelEditorCore.Mode mode);
	public delegate void NewSelectionForDownload(SaveFileInfo sender);

	public static List<CellBehaviour> cells = new List<CellBehaviour>();


	public static void GameOver() {
		print("You Lost");
	}

	public static void YouWon() {
		print("You Won");
	}
}


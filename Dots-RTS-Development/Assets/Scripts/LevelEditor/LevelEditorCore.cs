using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelEditorCore : MonoBehaviour {

	public static event GameControll.PanelValueChanged panelChange;
	//When EditMode Changes, passes the new mode
	public static event GameControll.EditModeChanged modeChange;

	// All of the input fields, there's only one input panel so this can be static
	public static InputField teamInput;
	public static InputField maxInput;
	public static InputField startInput;
	public static InputField regenInput;

	// Current parsed values, imparsable gets turned into default
	public static int team;
	public static int max;
	public static int start;
	public static float regen;

	// the set defaults
	public int defaultTeam = 0;
	public int defaultMax = 50;
	public int defaultStart = 10;
	public int defaultRegen = 1;

	// The current editor mode
	public enum Mode { WaitForModeChange, EditCells, DeleteCells, PlaceCells };

	public static Mode editorMode;

	public Texture2D[] cursors;

	private void Start() {
		teamInput = GameObject.Find("TeamInputField").GetComponent<InputField>();
		maxInput = GameObject.Find("MaxElementCountIF").GetComponent<InputField>();
		startInput = GameObject.Find("StartElementCountIF").GetComponent<InputField>();
		regenInput = GameObject.Find("RegenInputField").GetComponent<InputField>();
		GetPanelValues();
	}

	public void DestoyCellsButton() {
		editorMode = Mode.DeleteCells;
		modeChange(Mode.DeleteCells);
		Cursor.SetCursor(cursors[1], Vector2.zero, CursorMode.Auto);
	}
	public void PlaceCellButton() {
		editorMode = Mode.PlaceCells;
		modeChange(Mode.PlaceCells);
		Cursor.SetCursor(cursors[0], Vector2.zero, CursorMode.Auto);
	}
	public void EditCellsButton() {
		editorMode = Mode.EditCells;
		modeChange(Mode.EditCells);
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}

	public void GetPanelValues() {

		if (!int.TryParse(teamInput.text, out team)) {
			team = defaultTeam;
		}
		if (!int.TryParse(maxInput.text, out max)) {
			max = defaultMax;
		}
		if (!int.TryParse(startInput.text, out start)) {
			start = defaultStart;
		}
		if (!float.TryParse(regenInput.text, out regen)) {
			regen = defaultRegen;
		}

		editorMode = Mode.PlaceCells;

		if(modeChange != null) {
			modeChange(Mode.PlaceCells);
		}
		if (panelChange != null) {
			panelChange();
		}
	}
}

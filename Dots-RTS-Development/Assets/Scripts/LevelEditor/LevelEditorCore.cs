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

	// Current parsed values, imparsable gets turned into defalut
	public static int team;
	public static int max;
	public static int start;
	public static float regen;

	// the set defaluts
	public int defalutTeam;
	public int defalutMax;
	public int defalutStart;
	public int defalutRegen;

	// The current editor mode
	public enum Mode {
		WaitForModeChange,
		EditCells,
		DeleteCells,
		PlaceCells
	};
	public static Mode editorMode;

	public Texture2D[] cursors;

	private void Start() {
		//thisOneCamera = gameObject.GetComponent<Camera>();
		//print(start);
		teamInput = GameObject.Find("TeamInputField").GetComponent<InputField>();
		maxInput = GameObject.Find("MaxElementCountIF").GetComponent<InputField>();
		startInput = GameObject.Find("StartElementCountIF").GetComponent<InputField>();
		regenInput = GameObject.Find("RegenInputField").GetComponent<InputField>();
		//print(startInput.text);


		PanelChange();
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

	public void PanelChange() {
		//print("PanelChanged");
		
		if (!int.TryParse(teamInput.text, out team)) {
			team = defalutTeam;
		}
		if (!int.TryParse(maxInput.text, out max)) {
			max = defalutMax;
		}
		if (!int.TryParse(startInput.text, out start)) {
			start = defalutStart;
		}
		if (!float.TryParse(regenInput.text, out regen)) {
			regen = defalutRegen;
		}

		if (panelChange != null) {
			panelChange();
		}
	}
}

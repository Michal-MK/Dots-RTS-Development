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
	public static InputField difficultyInput;
	public static InputField sizeInput;

	// Current parsed values, imparsable gets turned into defalut
	public static int team;
	public static int max;
	public static int start;
	public static float regen;
	public static float difficulty;
	public static float gameSize;

	// the set defaluts
	public int defalutTeam;
	public int defalutMax;
	public int defalutStart;
	public int defalutRegen;
	public float defalutDifficulty;
	public float defalutGameSize;

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
		
		//Get All of the references to input fields even those that are later turned off;
		teamInput = GameObject.Find("TeamInputField").GetComponent<InputField>();
		maxInput = GameObject.Find("MaxElementCountIF").GetComponent<InputField>();
		startInput = GameObject.Find("StartElementCountIF").GetComponent<InputField>();
		regenInput = GameObject.Find("RegenInputField").GetComponent<InputField>();
		difficultyInput = GameObject.Find("AiReactionSpeed").GetComponent<InputField>();
		sizeInput = GameObject.Find("GameSize").GetComponent<InputField>();

		//Disable the panels;
		GameObject.Find("IOHugePanel").SetActive(false);
		GameObject.Find("GameSettingsPanel").SetActive(false);

		//Send an event
		PanelChange();
		//Subscribe to that event so even this script can use it
		panelChange += internalPanelChange; 

	}

	private void OnDestroy() {
		panelChange -= internalPanelChange;
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
		if (!float.TryParse(difficultyInput.text, out difficulty)) {
			difficulty = defalutDifficulty;
		}
		if (!float.TryParse(sizeInput.text, out gameSize)) {
			gameSize = defalutGameSize;
		}
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
			//Start the panel change event;
			panelChange();

		}
	}

	void internalPanelChange() {
		if (gameSize != Camera.main.orthographicSize) {
			Camera.main.orthographicSize = gameSize;
		}
	}
}

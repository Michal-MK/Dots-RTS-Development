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
	public static InputField aiDifficultyInput;
	public static InputField sizeInput;

	// Current parsed values, imparsable gets turned into default
	public static int team;
	public static int max;
	public static int start;
	public static float regen;
	public static float aiDificulty;
	public static float gameSize;


	// the set defaults
	public int defaultTeam = 0;
	public int defaultMax = 50;
	public int defaultStart = 10;
	public float defaultRegen = 1f;
	public float defaultDificulty = 2f;
	public float defalutGameSize = 250f;

	// The current editor mode
	public enum Mode { WaitForModeChange, EditCells, DeleteCells, PlaceCells };

	public static Mode editorMode;

	public Texture2D[] cursors;

	private void Start() {
		teamInput = GameObject.Find("TeamInputField").GetComponent<InputField>();
		maxInput = GameObject.Find("MaxElementCountIF").GetComponent<InputField>();
		startInput = GameObject.Find("StartElementCountIF").GetComponent<InputField>();
		regenInput = GameObject.Find("RegenInputField").GetComponent<InputField>();
		aiDifficultyInput = GameObject.Find("AiReactionSpeed").GetComponent<InputField>();
		sizeInput = GameObject.Find("GameSettingsPanel").GetComponent<InputField>();

		//Disable the panels;
		GameObject.Find("IOHugePanel").SetActive(false);
		GameObject.Find("GameSettingsPanel").SetActive(false);

		//Send an event
		GetPanelValues();

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
		if (!float.TryParse(aiDifficultyInput.text, out aiDificulty)) {
			aiDificulty = defaultDificulty;
		}

		editorMode = Mode.PlaceCells;

		if(modeChange != null) {
			modeChange(Mode.PlaceCells);
		}
		if (panelChange != null) {
			panelChange();
		}

	}

	void internalPanelChange() {
		if (gameSize != Camera.main.orthographicSize) {
			Camera.main.orthographicSize = gameSize;
		}
	}
}

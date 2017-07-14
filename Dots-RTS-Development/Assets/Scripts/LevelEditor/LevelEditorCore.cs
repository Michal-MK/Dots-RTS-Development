using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelEditorCore : MonoBehaviour {

	public static event Control.PanelValueChanged panelChange;
	//When EditMode Changes, passes the new mode
	public static event Control.EditModeChanged modeChange;

	/// <summary>
	/// All of the input fields, there's only one input panel so this can be static
	/// </summary>
	//PlaceCellsPanel
	public static TeamSelectScript teamInput;
	public static InputField maxInput;
	public static InputField startInput;
	public static InputField regenInput;
	//GameSetttingPanel
	public static InputField aiDifficultyInput;
	public static InputField sizeInput;
	//IOPanel
	public static InputField fileNameInput;
	public static InputField levelNameInput;
	public static InputField authorNameInput;
	

	/// <summary>
	/// Current parsed values, imparsable gets turned into default
	/// </summary>
	//PlaceCellsPanel
	public static int _team;
	public static int max;
	public static int start;
	public static float regen;
	//GameSettingsPanel
	public static float aiDificulty;
	public static float gameSize;
	//IOPanel
	public static string fileName;
	public static string levelName;
	public static string authorName;
	


	/// <summary>
	/// the set defaults
	/// </summary>
	//PlaceCellsPanel
	public int defaultTeam = 0;
	public int defaultMax = 50;
	public int defaultStart = 10;
	public float defaultRegen = 1f;
	//GameSettingsPanel
	public float defaultDificulty = 2f;
	public float defaultGameSize = 250f;
	//IOPanel
	public string defaultFileName = "UserMadeLevel";
	public string defaultLevelName = "CustomLevel";
	public string defaultAuthorName = "Anonymous";
	

	// The current editor mode
	public enum Mode { WaitForModeChange, EditCells, DeleteCells, PlaceCells };

	public static Mode editorMode;

	public Texture2D[] cursors;



	private void Start() {
		//Find all of the input fields;
		teamInput = gameObject.GetComponent<TeamSelectScript>();
		maxInput = GameObject.Find("MaxElementCountIF").GetComponent<InputField>();
		startInput = GameObject.Find("StartElementCountIF").GetComponent<InputField>();
		regenInput = GameObject.Find("RegenInputField").GetComponent<InputField>();

		aiDifficultyInput = GameObject.Find("AiReactionSpeed").GetComponent<InputField>();
		sizeInput = GameObject.Find("GameSize").GetComponent<InputField>();

		fileNameInput = GameObject.Find("FileNameIF").GetComponent<InputField>();
		levelNameInput = GameObject.Find("Level Name").GetComponent<InputField>();
		authorNameInput = GameObject.Find("Author's name").GetComponent<InputField>();

		//Disable the panels;
		GameObject.Find("IOHugePanel").SetActive(false);
		GameObject.Find("GameSettingsPanel").SetActive(false);

		//Set the defaluts by parsing all of the input fields
		GetPlaceCellPanelValues();
		GetGameSettingsPanelValues();
		GetIOPanelValues();

		// Set defalut mode to placeCells
		StartCoroutine(WaitOneFrame());
	}

	//Have to wait for all of the things to initialize before I can call a button press
	IEnumerator WaitOneFrame() {
		yield return new WaitForEndOfFrame();
		PlaceCellButton();
	}

	public static void TeamSET() {
		teamInput.UpdateButtonVisual();
	}

	private void OnDestroy() {
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

	public void GetPlaceCellPanelValues() {
		
		team = teamInput.team;

		if (!int.TryParse(maxInput.text, out max)) {
			max = defaultMax;
		}
		if (!int.TryParse(startInput.text, out start)) {
			start = defaultStart;
		}
		if (!float.TryParse(regenInput.text, out regen)) {
			regen = defaultRegen;
		}
		

		if (panelChange != null) {
			panelChange();
		}

	}
	public void GetGameSettingsPanelValues () {
		if (!float.TryParse(aiDifficultyInput.text, out aiDificulty)) {
			aiDificulty = defaultDificulty;
		}
		if (!float.TryParse(sizeInput.text, out gameSize)) {
			gameSize = defaultGameSize;
		}

		RefreshCameraSize();
	}
	public void GetIOPanelValues() {
		if (string.IsNullOrEmpty(fileNameInput.text) || fileNameInput.text == " ") {
			fileName = defaultFileName;
		}
		else {
			fileName = fileNameInput.text;
		}

		if (string.IsNullOrEmpty(levelNameInput.text) || levelNameInput.text == " ") {
			levelName = defaultLevelName;			
		}
		else {
			levelName = levelNameInput.text;
		}

		if (string.IsNullOrEmpty(authorNameInput.text) || authorNameInput.text == " ") {
			authorName = defaultAuthorName;
		}
		else {
			authorName = authorNameInput.text;
		}
	}

	//This is called with the panelChange event;
	public void RefreshCameraSize() {
		if (gameSize != Camera.main.orthographicSize) {
			if (gameSize < 250) {
				sizeInput.text = "250";
				gameSize = 250;
			}

			Camera.main.orthographicSize = gameSize;
		}
	}

	public static int team {
		get { return _team; }
		set { _team = value; TeamSET(); }
	}
}

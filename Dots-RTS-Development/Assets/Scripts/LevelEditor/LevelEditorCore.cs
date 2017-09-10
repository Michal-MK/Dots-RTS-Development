using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Globalization;

public class LevelEditorCore : MonoBehaviour {
	/// <summary>
	/// Triggered when value from an input field is parsed
	/// </summary>
	public static event Control.PanelValueChanged panelValueParsed;
	/// <summary>
	/// Triggered when user changes mouse behaviour
	/// </summary>
	public static event Control.EditModeChanged modeChange;


	public GameObject CellPrefab;
	public Transform teamButton;
	public GameObject upgradePanel;

	public List<EditCell> cellList = new List<EditCell>();
	public List<Cell.enmTeam> teamList = new List<Cell.enmTeam>();
	public List<EditCell> selectedCellList = new List<EditCell>();
	public Texture2D[] cursors;


	#region InputFields
	//CellEdittingPanel
	public InputField maxInput;
	public InputField startInput;
	public InputField regenInput;

	//GameSetttingPanel
	public InputField aiDifficultyAllInput;
	public InputField aiDifficultySingleInput;
	public InputField sizeInput;

	//ExportPanel
	public InputField levelNameInput;
	public InputField authorNameInput;
	#endregion

	#region Parsed value for each input field
	//Cell Editting Panel
	private Cell.enmTeam _team;
	private int _max;
	private int _start;
	private float _regen;

	//Game Settings Panel
	private float aiDificultyAll;
	private float _gameSize;
	public Dictionary<Cell.enmTeam, float> aiDifficultyDict = new Dictionary<Cell.enmTeam, float>();

	//Export Panel
	public string levelName;
	public string authorName;
	//view
	private static bool _outlineOn = false;
	private bool _areCellsFitToScreen = false;
	#endregion

	#region Default value for each input field ==> this gets used if input field value can not be parsed correctly
	//Cell Editting Panel
	private Cell.enmTeam defaultTeam = Cell.enmTeam.NEUTRAL;
	private int defaultMax = 50;
	private int defaultStart = 10;
	private float defaultRegen = 1f;

	//Game Settings Panel
	public float defaultDificulty = 2f;
	public float defaultGameSize = 250f;

	//Export Panel
	public string defaultLevelName = "CustomLevel";
	public string defaultAuthorName = "Anonymous";
	#endregion

	#region Enums
	// The current editor mode
	public enum Mode {
		/// <summary>
		/// In this mode user can use the mouse only for placing new cells to the scene 
		/// </summary>
		PlaceCells,
		/// <summary>
		/// In this mode user can use the mouse only for moving the cell of altering its attributes
		/// </summary>
		EditCells,
		/// <summary>
		/// In this mode user can use the mouse only for removeing the cell by clicking on it
		/// </summary>
		DeleteCells
	};
	// Input field to prase from
	public enum PCPanelAttribute {
		/// <summary>
		/// Starting element count will be parsed from the input field
		/// </summary>
		Start,
		/// <summary>
		/// Maximum element count will be parsed from the input field
		/// </summary>
		Max,
		/// <summary>
		/// Regeneration period will be parsed from the input field
		/// </summary>
		Regen,
		/// <summary>
		/// Team will be parsed from the next click on the appropriate button.
		/// </summary>
		Team
	};
	#endregion

	private Mode _editorMode;
	private TeamSetup teamSetup;
	private bool _isUpdateSentByCell;
	private Cell.enmTeam singleDifficultyInputFieldSpecificTeam;

	private void Start() {
		teamSetup = UI_ReferenceHolder.LE_gameSettingsPanel.GetComponent<TeamSetup>();

		EditCell.OnCellSelected += EditCell_OnCellSelected;
		EditCell.OnCellDeselected += EditCell_OnCellDeselected;

		//Set the defaluts by parsing all of the input fields
		PlaceCellPanelParseFromField(PCPanelAttribute.Start);
		PlaceCellPanelParseFromField(PCPanelAttribute.Max);
		PlaceCellPanelParseFromField(PCPanelAttribute.Regen);
		team = defaultTeam;
		ParseGameSize_GameSettingsPanel();
		ParseSaveInfo_SavePanel();
		AiDiffHandler();

		//Set defalut mode to placeCells
		editorMode = Mode.PlaceCells;
	}

	private void OnDestroy() {
		EditCell.OnCellSelected -= EditCell_OnCellSelected;
		EditCell.OnCellDeselected -= EditCell_OnCellDeselected;
	}

	#region Functions to add or remove a cell from the static list
	public void AddCell(EditCell c) {

		cellList.Add(c);
		if (getOutilneState) {
			c.ToggleCellOutline(true);
		}
		if (c.cellTeam != Cell.enmTeam.ALLIED && c.cellTeam != Cell.enmTeam.NEUTRAL) {
			if (!teamList.Contains(c.cellTeam)) {
				teamList.Add(c.cellTeam);
				aiDifficultyDict.Add(c.cellTeam, defaultDificulty);
				AllAiDifficultyWriter.RedoText(aiDifficultyDict);
			}
		}

	}
	public void RemoveCell(EditCell c) {
		cellList.Remove(c);
		//If a cell is found with the same team then don't remove the clan.
		foreach (Cell cell in cellList) {
			if (cell.cellTeam == c.cellTeam) {
				return;
			}
		}
		teamSetup.RemoveFromClan(c.cellTeam);
		teamList.Remove(c.cellTeam);
		aiDifficultyDict.Remove(c.cellTeam);
		AllAiDifficultyWriter.RedoText(aiDifficultyDict);
	}
	#endregion

	#region Event subscribers
	private void EditCell_OnCellSelected(EditCell sender) {
		if (!selectedCellList.Contains(sender)) {
			selectedCellList.Add(sender);
		}
		if (areCellsFitToScreen) {
			FitCellsOnScreen(selectedCellList);
		}
	}

	private void EditCell_OnCellDeselected(EditCell sender) {
		selectedCellList.Remove(sender);
		if (areCellsFitToScreen) {
			FitCellsOnScreen(selectedCellList);
		}
	}
	#endregion

	#region Wrappers

	public void ModeButtonWrapper(int mode) {
		ModeButton((Mode)mode);
	}

	public void PlaceCellPanelParseFromFieldWrapper(int inputField) {
		PlaceCellPanelParseFromField((PCPanelAttribute)inputField);
	}

	public void TeamSelectedButtonWrapper(int thisTeam) {
		TeamSelectedButton((Cell.enmTeam)thisTeam);
	}

	public void AiDiffHandlerWrapper() {
		AiDiffHandler(singleDifficultyInputFieldSpecificTeam);
	}

	#endregion

	//Cell placing logic
	private void Update() {
#if (UNITY_EDITOR || UNITY_STANDALONE)
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && editorMode == Mode.PlaceCells) {
			Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
			EditCell c = newCell.GetComponent<EditCell>();
			c.cellTeam = team;
			c.maxElements = maxElementCount;
			c.regenPeriod = regenarationPeriod;
			c.elementCount = startingElementCount;
			c.core = this;
			AddCell(c);
			c.FastResize();

		}
#endif
#if (UNITY_ANDROID || UNITY_IOS)
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && LevelEditorCore.editorMode == LevelEditorCore.Mode.PlaceCells) {
			Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(CellPrefab, pos, Quaternion.identity);
			EditCell c = newCell.GetComponent<EditCell>();
			c.cellTeam = team;
			c.maxElements = max;
			c.regenPeriod = regen;
			c.elementCount = start;
			c.core = this;
			c.FastResize();
			AddCell(c);
		c.FastResize();
		}
#endif
	}

	//Switches the cusror mode 
	public void ModeButton(Mode mode) {
		_editorMode = mode;
		modeChange?.Invoke(mode);
		switch (mode) {
			case Mode.PlaceCells: {
				Cursor.SetCursor(cursors[0], Vector2.zero, CursorMode.Auto);

				return;
			}
			case Mode.EditCells: {
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
				return;
			}
			case Mode.DeleteCells: {
				Cursor.SetCursor(cursors[1], Vector2.zero, CursorMode.Auto);
				return;
			}
			default: {
				Debug.LogError("Unknown mode passed");
				return;
			}
		}
	}

	//Parses a value from selected input field
	public void PlaceCellPanelParseFromField(PCPanelAttribute inputField) {
		switch (inputField) {
			case PCPanelAttribute.Start: {
				if (!int.TryParse(startInput.text, out _start)) {
					print(startInput.text);
					_start = defaultStart;
				}
				panelValueParsed?.Invoke(PCPanelAttribute.Start);
				return;
			}
			case PCPanelAttribute.Max: {
				if (!int.TryParse(maxInput.text, out _max)) {
					print(maxInput.text);
					_max = defaultMax;
				}
				panelValueParsed?.Invoke(PCPanelAttribute.Max);
				return;
			}
			case PCPanelAttribute.Regen: {
				if (!float.TryParse(regenInput.text, NumberStyles.Float, CultureInfo.InvariantCulture, out _regen)) {
					print(regenInput.text);
					_regen = defaultRegen;
				}
				panelValueParsed?.Invoke(PCPanelAttribute.Regen);
				return;
			}
			case PCPanelAttribute.Team: {
				UI_ReferenceHolder.LE_cellInputFields.SetActive(false);
				UI_ReferenceHolder.LE_teamPickerPanel.SetActive(true);
				return;
			}

		}
	}

	//Prses values for saving a level
	public void ParseSaveInfo_SavePanel() {

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

	//Parses a value for game size
	public void ParseGameSize_GameSettingsPanel() {
		float f = 250;
		if (float.TryParse(sizeInput.text, NumberStyles.Float, CultureInfo.InvariantCulture, out f) && f > 250) {
			print(f);
			gameSize = f;

		}
		else {
			gameSize = defaultGameSize;
		}
	}

	public void TeamSelectedButton(Cell.enmTeam correspondingTeam) {
		team = correspondingTeam;
		UI_ReferenceHolder.LE_teamPickerPanel.SetActive(false);
		UI_ReferenceHolder.LE_cellInputFields.SetActive(true);
	}

	private void UpdateTeamButtonVisual(Cell.enmTeam thisTeam) {
		Text description = teamButton.Find("TeamDescription").GetComponent<Text>();
		description.color = ColourTheTeamButtons.GetContrastColorBasedOnTeam(thisTeam);
		description.text = ColourTheTeamButtons.GetDescriptionBasedOnTeam(thisTeam);
		teamButton.GetComponent<Image>().color = ColourTheTeamButtons.GetColorBasedOnTeam(thisTeam);
	}

	public void CellOutlineToggle(Toggle toggle) {
		_outlineOn = toggle.isOn;
		foreach (EditCell cell in cellList) {
			cell.ToggleCellOutline(toggle.isOn);
		}
	}

	/// <summary>
	/// Refreshes the Ai difficulty
	/// </summary>
	/// <param name="team">the team this applies to, 0 = all</param>
	public void AiDiffHandler(Cell.enmTeam team = Cell.enmTeam.NEUTRAL) {
		if (float.TryParse(aiDifficultyAllInput.text, NumberStyles.Float, CultureInfo.InvariantCulture, out aiDificultyAll) && team == 0) {
			foreach (Cell.enmTeam key in teamList) {
				if (aiDifficultyDict.ContainsKey(key)) {
					aiDifficultyDict.Remove(key);
				}
				aiDifficultyDict.Add(key, aiDificultyAll);
			}

		}
		else if (!float.TryParse(aiDifficultyAllInput.text, NumberStyles.Float, CultureInfo.InvariantCulture, out aiDificultyAll) && team == 0) {
			foreach (Cell.enmTeam key in teamList) {
				if (aiDifficultyDict.ContainsKey(key)) {
					aiDifficultyDict.Remove(key);
				}
				aiDifficultyDict.Add(key, defaultDificulty);
			}

		}
		else {
			float singleDiff;
			if (float.TryParse(aiDifficultySingleInput.text, NumberStyles.Float, CultureInfo.InvariantCulture, out singleDiff)) {
				aiDifficultyDict.Remove(team);
				aiDifficultyDict.Add(team, singleDiff);


			}
			else {
				aiDifficultyDict.Remove(team);
				aiDifficultyDict.Add(team, defaultDificulty);

			}
		}
		aiDifficultySingleInput.gameObject.SetActive(false);
		AllAiDifficultyWriter.RedoText(aiDifficultyDict);
	}

	public void EnableSingleDiffInputField(TeamBox t) {

		aiDifficultySingleInput.gameObject.SetActive(true);
		aiDifficultySingleInput.transform.position = (Vector2)t.transform.position + new Vector2(0, t.myRectTransform.sizeDelta.y / 2);
		float value;
		if (aiDifficultyDict.TryGetValue(t.team, out value)) {
			aiDifficultySingleInput.contentType = InputField.ContentType.Standard;
			aiDifficultySingleInput.text = value.ToString();
			aiDifficultySingleInput.contentType = InputField.ContentType.DecimalNumber;
		}
		else {
			aiDifficultySingleInput.text = "";
		}
		singleDifficultyInputFieldSpecificTeam = t.team;
	}

	//public static void BringUpUpgradePanel() {
	//	RectTransform rc = upgradePanel.GetComponent<RectTransform>();
	//	//rc.sizeDelta = new Vector2(rc.sizeDelta.x, 0);
	//}

	//This is called with the panelChange event;
	public void RefreshCameraSize(float val) {
		if (val != Camera.main.orthographicSize) {
			if (val < 250) {
				sizeInput.text = "250";
				gameSize = 250;
			}
			Camera.main.transform.position = Vector3.zero + Vector3.back * 10;
			Camera.main.orthographicSize = val;
			GameObject.Find("Borders").GetComponent<PlayFieldSetup>().ResizeBackground(Camera.main.aspect);
		}
	}

	public void FitCellsOnScreen(List<EditCell> cells) {
		//find borders
		if (cells.Count == 0) {
			RefreshCameraSize(gameSize);
			return;
		}
		float lowestX = Mathf.Infinity;
		float highestX = -Mathf.Infinity;
		float lowestY = Mathf.Infinity;
		float highestY = -Mathf.Infinity;
		//print("CellCount " + cells.Count);
		float adjustBy = 0;

		foreach (EditCell cell in cellList) {
			if (cell.cellRadius * 0.33f > adjustBy) {
				adjustBy = cell.cellRadius * 0.33f;
			}
		}

		if (cells.Count == 1) {
			Vector3 pos = cells[0].transform.position;
			lowestX = pos.x - cells[0].cellRadius;
			highestX = pos.x + cells[0].cellRadius;
			lowestY = pos.y - cells[0].cellRadius;
			highestY = pos.y + cells[0].cellRadius;
		}
		else {
			foreach (Cell cell in cells) {
				Vector3 cellPos = cell.transform.position;
				//print("radius " + cell.cellRadius);
				if (cellPos.x - cell.cellRadius < lowestX) {
					lowestX = cellPos.x - cell.cellRadius;

				}
				if (cellPos.x + cell.cellRadius > highestX) {
					highestX = cellPos.x + cell.cellRadius;
				}
				if (cellPos.y - cell.cellRadius < lowestY) {
					lowestY = cellPos.y - cell.cellRadius;
				}
				if (cellPos.y + cell.cellRadius > highestY) {
					highestY = cellPos.y + cell.cellRadius;
				}
			}
		}
		lowestX -= adjustBy;
		highestX += adjustBy;
		lowestY -= adjustBy;
		highestY += adjustBy;

		Camera.main.transform.position = new Vector3(Mathf.Lerp(lowestX, highestX, 0.5f), Mathf.Lerp(lowestY, highestY, 0.5f), -10);
		float aspect = Camera.main.aspect;
		if ((highestX - lowestX) / aspect >= highestY - lowestY) {
			float delta = highestX - lowestX;
			float heightFromWidth = delta / aspect;
			Camera.main.orthographicSize = heightFromWidth * 0.5f;
		}
		else {
			float delta = highestY - lowestY;
			Camera.main.orthographicSize = delta * 0.5f;
		}
	}


	#region Getters and Setters
	public bool areCellsFitToScreen {
		get { return _areCellsFitToScreen; }
		set {
			_areCellsFitToScreen = value;
			if (value == true) {
				FitCellsOnScreen(selectedCellList);
			}
			else {
				RefreshCameraSize(gameSize);
			}
		}

	}

	public Mode editorMode {
		get { return _editorMode; }
		set {
			ModeButton(value);
		}
	}

	public bool isUpdateSentByCell {
		get { return _isUpdateSentByCell; }
		set { _isUpdateSentByCell = value; }
	}

	public Cell.enmTeam team {
		get { return _team; }
		set {
			_team = value;
			UpdateTeamButtonVisual(value);
			panelValueParsed?.Invoke(PCPanelAttribute.Team);
		}
	}

	public float gameSize {
		get { return _gameSize; }
		set {
			_gameSize = value;
			RefreshCameraSize(value);
		}
	}

	public int startingElementCount {
		get { return _start; }
		set {
			_start = value;
			startInput.text = value.ToString();
		}
	}

	public float regenarationPeriod {
		get { return _regen; }
		set {
			_regen = value;
			regenInput.text = value.ToString();
		}
	}

	public int maxElementCount {
		get { return _max; }
		set {
			_max = value;
			maxInput.text = value.ToString();
		}
	}
	public static bool getOutilneState {
		get { return _outlineOn; }
	}
	#endregion
}

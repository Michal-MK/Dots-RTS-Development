using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorCore : MonoBehaviour {

	public static event Control.PanelValueChanged panelChange;
	//When EditMode Changes, passes the new mode
	public static event Control.EditModeChanged modeChange;

	static Vector3 defaultCameraPosition;
	static TeamSetup teamSetup;


	public static List<Cell> cellList = new List<Cell>();
	public static List<Cell.enmTeam> teamList = new List<Cell.enmTeam>();
	public static List<Cell> selectedCellList = new List<Cell>();
	/// <summary>
	/// All of the input fields, there's only one input panel so this can be static --- epic reasoning
	/// </summary>
	//PlaceCellsPanel
	public static TeamSelectScript teamButton;
	public static InputField maxInput;
	public static InputField startInput;
	public static InputField regenInput;
	//GameSetttingPanel
	public static InputField aiDifficultyAllInput;
	public static InputField aiDifficultySingleInput;
	public static InputField sizeInput;
	//IOPanel
	//public static InputField fileNameInput;
	public static InputField levelNameInput;
	public static InputField authorNameInput;

	public static GameObject upgradePanel;

	/// <summary>
	/// Current parsed values, imparsable gets turned into default
	/// </summary>
	//PlaceCellsPanel
	public static int _team;
	public static int max;
	public static int start;
	public static float regen;
	//GameSettingsPanel
	public static float aiDificultyAll;
	public static float _gameSize;
	public static Dictionary<int, float> aiDifficultyDict = new Dictionary<int, float>();
	//IOPanel
	//public static string fileName;
	public static string levelName;
	public static string authorName;
	//view
	static bool OutlineOn = false;



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
	//public string defaultFileName = "UserMadeLevel";
	public string defaultLevelName = "CustomLevel";
	public string defaultAuthorName = "Anonymous";


	// The current editor mode
	public enum Mode { WaitForModeChange, EditCells, DeleteCells, PlaceCells };
	public enum PCPanelAttribute { Team, Max, Start, Regen };

	public static Mode editorMode;

	public Texture2D[] cursors;

	public static bool dontUpdate;

	private IEnumerator Start() {
		//Find all of the input fields;
		teamButton = gameObject.GetComponent<TeamSelectScript>();
		maxInput = GameObject.Find("MAX_Elements_IF").GetComponent<InputField>();
		startInput = GameObject.Find("START_Elements_IF").GetComponent<InputField>();
		regenInput = GameObject.Find("REGEN_Elements_IF").GetComponent<InputField>();

		aiDifficultyAllInput = GameObject.Find("Canvas").transform.Find("GameSettingsPanel/AI Difficulty/AI_Diff_IF").GetComponent<InputField>();
		aiDifficultySingleInput = GameObject.Find("Canvas").transform.Find("GameSettingsPanel/Single_Ai_Diff_IF").GetComponent<InputField>();
		sizeInput = GameObject.Find("Canvas").transform.Find("GameSettingsPanel/Game Size/CAM_Size_IF").GetComponent<InputField>();

		levelNameInput = GameObject.Find("Canvas").transform.Find("SavePanel/LevelName/Level Name IF").GetComponent<InputField>();
		authorNameInput = GameObject.Find("Canvas").transform.Find("SavePanel/Author's name/Author's name IF").GetComponent<InputField>();

		//UpgradePanel
		upgradePanel = GameObject.Find("Upgrade_Panel");

		//Disable the panels;
		//aiDifficultySingleInput.gameObject.SetActive(false);
		UI_ReferenceHolder.LE_saveInfoPanel.SetActive(false);
		UI_ReferenceHolder.LE_gameSettingsPanel.SetActive(false);
		teamSetup = UI_ReferenceHolder.LE_gameSettingsPanel.GetComponent<TeamSetup>();


		//Set the defaluts by parsing all of the input fields
		defaultCameraPosition = Camera.main.transform.position;
		ParseCellTeam_PlaceCellPanel();
		ParseRegenPeriod_PlaceCellPanel();
		ParseMaxElementCount_PlaceCellPanel();
		ParseStartingElementCount_PlaceCellPanel();
		ParseGameSize_GameSettingsPanel();
		ParseSaveInfo_SavePanel();
		AiDiffHandler();

		// Set defalut mode to placeCells
		//Have to wait for all of the things to initialize before I can call a button press
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		PlaceCellButton();
	}

	#region Functions to add or remove a cell from the static list
	public static void AddCell(Cell c) {

		cellList.Add(c);
		if (OutlineOn) {
			c.gameObject.SendMessage("ToggleCellOutline", true);
		}
		if (c.cellTeam != Cell.enmTeam.ALLIED && c.cellTeam != Cell.enmTeam.NEUTRAL) {
			if (!teamList.Contains(c.cellTeam)) {
				teamList.Add(c.cellTeam);
			}
		}

	}
	public static void RemoveCell(Cell c) {
		cellList.Remove(c);
		foreach (Cell cell in cellList) {
			if (cell.cellTeam == c.cellTeam) {
				return;
			}
		}
		teamSetup.RemoveFromClan((int)c.cellTeam);
		teamList.Remove(c.cellTeam);

	}
	#endregion

	


	public void CellOutlineToggle(Toggle toggle) {
		OutlineOn = toggle.isOn;
		if (toggle.isOn) {
			foreach (Cell cell in cellList) {
				cell.gameObject.SendMessage("ToggleCellOutline", true);
			}
		}
		else {
			foreach (Cell cell in cellList) {
				cell.gameObject.SendMessage("ToggleCellOutline", false);
			}
		}
	}


	private void OnDestroy() {
		LevelEditorCore.cellList.Clear();
	}

	public void DestoyCellsButton() {
		editorMode = Mode.DeleteCells;
		modeChange?.Invoke(Mode.DeleteCells);
		Cursor.SetCursor(cursors[1], Vector2.zero, CursorMode.Auto);
	}
	public void PlaceCellButton() {
		editorMode = Mode.PlaceCells;
		modeChange?.Invoke(Mode.PlaceCells);
		Cursor.SetCursor(cursors[0], Vector2.zero, CursorMode.Auto);
	}
	public void EditCellsButton() {
		editorMode = Mode.EditCells;
		modeChange?.Invoke(Mode.EditCells);
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}
	//public void UpgradesButton() {
	//	editorMode = Mode.AssignUpgrades;
	//	modeChange?.Invoke(Mode.AssignUpgrades);
	//	Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	//}

	public void ParseCellTeam_PlaceCellPanel() {
		team = teamButton.team;

		if (panelChange != null && dontUpdate == false) {
			panelChange(PCPanelAttribute.Team);
		}

	}
	public void ParseMaxElementCount_PlaceCellPanel() {


		if (!int.TryParse(maxInput.text, out max)) {
			max = defaultMax;
		}

		if (panelChange != null && dontUpdate == false) {
			panelChange(PCPanelAttribute.Max);
		}

	}
	public void ParseStartingElementCount_PlaceCellPanel() {

		if (!int.TryParse(startInput.text, out start)) {
			start = defaultStart;
		}
		if (panelChange != null && dontUpdate == false) {
			panelChange(PCPanelAttribute.Start);
		}

	}
	public void ParseRegenPeriod_PlaceCellPanel() {

		if (!float.TryParse(regenInput.text, out regen)) {
			regen = defaultRegen;
		}

		if (panelChange != null && dontUpdate == false) {
			panelChange(PCPanelAttribute.Regen);
		}

	}

	/// <summary>
	/// Refreshes the Ai difficulty
	/// </summary>
	/// <param name="team">the team this applies to, 0 = all</param>
	public void AiDiffHandler(int team = 0) {
		if (float.TryParse(aiDifficultyAllInput.text, out aiDificultyAll) && team == 0) {
			foreach (int key in teamList) {
				if (aiDifficultyDict.ContainsKey(key)) {
					aiDifficultyDict.Remove(key);
				}
				aiDifficultyDict.Add(key, aiDificultyAll);
			}
			aiDifficultySingleInput.gameObject.SetActive(false);
		}
		else if (!float.TryParse(aiDifficultyAllInput.text, out aiDificultyAll) && team == 0) {
			foreach (int key in teamList) {
				if (aiDifficultyDict.ContainsKey(key)) {
					aiDifficultyDict.Remove(key);
				}
				aiDifficultyDict.Add(key, defaultDificulty);
			}
			aiDifficultySingleInput.gameObject.SetActive(false);
		}
		else {
			float singleDiff;
			if (float.TryParse(aiDifficultySingleInput.text, out singleDiff)) {
				aiDifficultyDict.Remove(team);
				aiDifficultyDict.Add(team, singleDiff);

			}
			else {
				aiDifficultyDict.Remove(team);
				aiDifficultyDict.Add(team, defaultDificulty);

			}
		}
		AllAiDifficultyWriter.RedoText();
	}

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

	public void ParseGameSize_GameSettingsPanel() {
		float f = 250;
		if (float.TryParse(sizeInput.text, out f) && f > 250) {
			gameSize = f;

		}
		else {
			//print("setToDefault");
			gameSize = defaultGameSize;
		}
	}

	//public static void BringUpUpgradePanel() {
	//	RectTransform rc = upgradePanel.GetComponent<RectTransform>();
	//	//rc.sizeDelta = new Vector2(rc.sizeDelta.x, 0);
	//}

	//This is called with the panelChange event;
	public static void RefreshCameraSize(float val) {
		if (val != Camera.main.orthographicSize) {
			if (val < 250) {
				sizeInput.text = "250";
				gameSize = 250;
			}
			Camera.main.transform.position = defaultCameraPosition;
			Camera.main.orthographicSize = val;
			GameObject.Find("Borders").GetComponent<PositionColiders>().ResizeBackground(Camera.main.aspect);
		}
	}

	public static void FitCellsOnScreen(List<Cell> cells) {
		//find boarders
		if (cells.Count == 0) {
			return;
		}
		float lowestX = Mathf.Infinity;
		float highestX = 0;
		float lowestY = Mathf.Infinity;
		float highestY = 0;
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
				if (cellPos.x < lowestX) {
					lowestX = cellPos.x - cell.cellRadius;
					print(cell.cellRadius);
				}
				if (cellPos.x > highestX) {
					highestX = cellPos.x + cell.cellRadius;
				}
				if (cellPos.y < lowestY) {
					lowestY = cellPos.y - cell.cellRadius;
				}
				if (cellPos.y > highestY) {
					highestY = cellPos.y + cell.cellRadius;
				}
			}
		}
		Camera.main.transform.position = new Vector3(Mathf.Lerp(lowestX, highestX, 0.5f), Mathf.Lerp(lowestY, highestY, 0.5f), -10);
		float aspect = Camera.main.aspect;
		if (highestX - lowestX > highestY - lowestY) {
			float delta = highestX - lowestX;
			float adjustedWidth = delta / aspect;
			Camera.main.orthographicSize = adjustedWidth * 0.5f;
		}
		else  {
			float delta = highestY - lowestY;
			Camera.main.orthographicSize = delta * 0.5f;
		}



	}

	public static int team {
		get { return _team; }
		set {
			_team = value;
			teamButton.UpdateButtonVisual();
		}
	}
	public static float gameSize {
		get { return _gameSize; }
		set {
			_gameSize = value;
			RefreshCameraSize(value);
		}
	}
}

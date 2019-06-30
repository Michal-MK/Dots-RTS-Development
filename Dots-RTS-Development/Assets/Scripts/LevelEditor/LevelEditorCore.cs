using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Globalization;
using System;

public class LevelEditorCore : MonoBehaviour {
	public GameObject cellPrefab;

	public SaveAndLoadEditor saveAndLoad;

	public List<EditCell> cellList = new List<EditCell>();
	public List<EditCell> selectedCellList = new List<EditCell>();
	public List<Team> teamList = new List<Team>();

	public LevelEditorUI UI;
	public TeamSetup teamSetup;

	public EditorMode EditorMode;
	private Team singleDifficultyInputFieldSpecificTeam;

	public UpgradeSlotCollection UIUpgradeSlots;

	[HideInInspector]
	public Texture2D[] cursors;

	#region Parsed value for each input field
	//Cell Editting Panel
	public Team Team;
	public int MaxElements;
	public int StartElements;
	public float Regeneration;

	//Game Settings Panel
	public float GlobalAIDifficulty;
	public float GameSize;
	public Dictionary<Team, float> aiDifficultyDict = new Dictionary<Team, float>();

	//Export Panel
	public string LevelName;
	public string AuthorName;

	//view
	public bool ShowCellOutline = false;
	public bool CellsFitToScreen = false;

	#endregion

	#region Default value for each input field

	private const Team DEFAULT_TEAM = Team.NEUTRAL;
	private const int DEFAULT_MAX_ELEMENTS = 50;
	private const int DEFAULT_INITIAL_ELEMENTS = 10;
	private const float DEFAULT_REGEN = 2f;

	private const float DEFAULT_DIFFICULTY = 2f;
	private const float DEFAULT_GAME_SIZE = 250;

	public const string DEFAULT_LEVEL_NAME = "CustomLevel";
	public const string DEFAULT_AUTHOR_NAME = "Anonymous";

	#endregion


	private void Start() {
		UI.startingElementCount.onValueChanged.AddListener((e) => { if (int.TryParse(e, out int value)) { StartElements = value; InputChanged(); } });
		UI.maxElementCount.onValueChanged.AddListener((e) => { if (int.TryParse(e, out int value)) { MaxElements = value; InputChanged(); } });
		UI.regenerationSpeed.onValueChanged.AddListener((e) => { if (int.TryParse(e, out int value)) { Regeneration = value; InputChanged(); } });

		Team = DEFAULT_TEAM;
		StartElements = DEFAULT_INITIAL_ELEMENTS;
		MaxElements = DEFAULT_MAX_ELEMENTS;
		Regeneration = DEFAULT_REGEN;

		ParseGameSize_GameSettingsPanel();
		ParseSaveInfo_SavePanel();
		AiDiffHandler();
		UIUpgradeSlots = new UpgradeSlotCollection(UI.uiUpgradeSlots);

		ModeButton(EditorMode.PlaceCells);
	}

	private void InputChanged() {
		foreach (EditCell cell in selectedCellList) {
			cell.UpdateStats();
		}
	}


	#region Functions to add or remove a cell from the static list
	public void AddCell(EditCell c, bool loadedFromFile = false) {

		cellList.Add(c);
		c.OnCellSelected += EditCell_OnCellSelected;
		c.OnCellDeselected += EditCell_OnCellDeselected;

		if (ShowCellOutline) {
			c.ToggleCellOutline(true);
		}

		if (c.Cell.Team != Team.NEUTRAL) {

			if (!teamList.Contains(c.Cell.Team)) {
				teamList.Add(c.Cell.Team);
				if (!loadedFromFile) {
					if (c.Cell.Team != Team.ALLIED) {
						aiDifficultyDict.Add(c.Cell.Team, DEFAULT_DIFFICULTY);
						AllAiDifficultyWriter.RedoText(aiDifficultyDict);
					}
				}
			}

		}
		if (loadedFromFile) {
			for (int i = 0; i < c.upgrade_manager.upgradeSlots.Length; i++) {
				if (c.upgrade_manager.upgrades[i] != Upgrades.NONE) {
					c.upgrade_manager.upgradeSlots[i].Type = c.upgrade_manager.upgrades[i];
					c.upgrade_manager.upgradeSlots[i].ChangeUpgradeImage(Upgrade.UPGRADE_GRAPHICS[c.upgrade_manager.upgrades[i]]);
				}
			}
			c.core = this;
		}
	}

	public void ResetScene() {
		foreach (EditCell c in cellList) {
			Destroy(c.gameObject);
		}
		cellList.Clear();
		teamSetup.clanDict.Clear();
		aiDifficultyDict.Clear();
	}

	public void RemoveCell(EditCell c) {
		cellList.Remove(c);
		c.OnCellSelected -= EditCell_OnCellSelected;
		c.OnCellDeselected -= EditCell_OnCellDeselected;

		//If a cell is found with the same team then don't remove the clan.
		foreach (CellBehaviour cell in cellList) {
			if (cell.Cell.Team == c.Cell.Team) {
				return;
			}
		}
		teamSetup.RemoveFromClan(c.Cell.Team);
		teamList.Remove(c.Cell.Team);
		if (c.Cell.Team != Team.ALLIED) {
			aiDifficultyDict.Remove(c.Cell.Team);
			AllAiDifficultyWriter.RedoText(aiDifficultyDict);
		}
	}
	#endregion

	#region Event subscribers
	private void EditCell_OnCellSelected(object sender, EditCell cell) {
		if (!selectedCellList.Contains(cell)) {
			selectedCellList.Add(cell);
		}
		if (CellsFitToScreen) {
			FitCellsOnScreen(selectedCellList);
		}
	}

	private void EditCell_OnCellDeselected(object sender, EditCell cell) {
		selectedCellList.Remove(cell);
		if (CellsFitToScreen) {
			FitCellsOnScreen(selectedCellList);
		}
	}
	#endregion

	#region Wrappers

	public void ModeButtonWrapper(int mode) {
		ModeButton((EditorMode)mode + 1);
	}

	public void TeamSelectedButtonWrapper(int thisTeam) {
		TeamSelectedButton((Team)thisTeam);
	}

	public void AiDiffHandlerWrapper() {
		AiDiffHandler(singleDifficultyInputFieldSpecificTeam);
	}

	#endregion

	//Cell placing logic
	private void Update() {
#if (UNITY_EDITOR || UNITY_STANDALONE)
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && EditorMode == EditorMode.PlaceCells) {
			Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			GameObject newCell = Instantiate(cellPrefab, pos, Quaternion.identity);

			newCell.name = "Cell " + Team;
			EditCell c = newCell.GetComponent<EditCell>();
			c.GetComponent<UM_Editor>().SetupUpgrades(UI);
			c.Cell.Team = Team;
			c.Cell.MaxElements = MaxElements;
			c.Cell.RegenPeriod = Regeneration;
			c.Cell.ElementCount = StartElements;
			c.upgrade_manager.upgrades = UIUpgradeSlots.Upgrades;
			for (int i = 0; i < c.upgrade_manager.upgrades.Length; i++) {
				c.upgrade_manager.upgradeSlots[i].Type = c.upgrade_manager.upgrades[i];
				c.upgrade_manager.upgradeSlots[i].ChangeUpgradeImage(Upgrade.UPGRADE_GRAPHICS[c.upgrade_manager.upgrades[i]]);
			}
			c.core = this;
			AddCell(c);
			c.FastResize();
			c.UpdateVisual();
		}
#endif
		//#if (UNITY_ANDROID || UNITY_IOS)
		//		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && editorMode == Mode.PlaceCells) {
		//			Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//			GameObject newCell = Instantiate(cellPrefab, pos, Quaternion.identity);
		//			EditCell c = newCell.GetComponent<EditCell>();
		//			c.cellTeam = team;
		//			c.maxElements = maxElementCount;
		//			c.regenPeriod = regenarationPeriod;
		//			c.elementCount = startingElementCount;
		//			c.upgrade_manager.upgrades = UpgradeSlot.getAssignedUpgrades;
		//			for (int i = 0; i < c.upgrade_manager.upgrades.Length; i++) {
		//				c.upgrade_manager.upgrade_Slots[i].type = c.upgrade_manager.upgrades[i];
		//				c.upgrade_manager.upgrade_Slots[i].ChangeUpgradeImage(Upgrade.UPGRADE_GRAPHICS[c.upgrade_manager.upgrades[i]]);
		//			}
		//			c.core = this;
		//			c.FastResize();
		//			AddCell(c);
		//		c.FastResize();
		//		}
		//#endif
	}

	//Switches the cusror mode 
	public void ModeButton(EditorMode mode) {
		EditorMode = mode;
		switch (mode) {
			case EditorMode.PlaceCells: {
				Cursor.SetCursor(cursors[0], Vector2.zero, CursorMode.Auto);
				while (selectedCellList.Count != 0)
					selectedCellList[0].isCellSelected = false;
				return;
			}
			case EditorMode.EditCells: {
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
				return;
			}
			case EditorMode.DeleteCells: {
				Cursor.SetCursor(cursors[1], Vector2.zero, CursorMode.Auto);
				while (selectedCellList.Count != 0)
					selectedCellList[0].isCellSelected = false;
				return;
			}
		}
	}


	public void ApplyAllToSelection() {
		foreach (EditCell cell in selectedCellList) {
			cell.upgrade_manager.PreinstallUpgrades(UIUpgradeSlots.Upgrades);
			cell.UpdateStats();
			cell.UpdateVisual();
		}
	}

	//Parses values for saving a level
	public void ParseSaveInfo_SavePanel() {

		if (string.IsNullOrEmpty(UI.levelNameInput.text) || UI.levelNameInput.text == " ") {
			LevelName = DEFAULT_LEVEL_NAME;
		}
		else {
			LevelName = UI.levelNameInput.text;
		}

		if (string.IsNullOrEmpty(UI.authorNameInput.text) || UI.authorNameInput.text == " ") {
			AuthorName = DEFAULT_AUTHOR_NAME;
		}
		else {
			AuthorName = UI.authorNameInput.text;
		}
	}

	//Parses a value for game size
	public void ParseGameSize_GameSettingsPanel() {
		if (float.TryParse(UI.sizeInput.text, NumberStyles.Float, CultureInfo.InvariantCulture, out float f) && f > DEFAULT_GAME_SIZE) {
			GameSize = f;
		}
		else {
			GameSize = DEFAULT_GAME_SIZE;
		}
		RefreshCameraSize(GameSize);
	}

	public void TeamSelectedButton(Team correspondingTeam) {
		Team = correspondingTeam;
		Text buttonText = UI.activeTeamButton.GetComponentInChildren<Text>();
		buttonText.text = correspondingTeam.ToString();
		buttonText.color = CellColours.GetContrastColor(correspondingTeam);
		UI.activeTeamButton.GetComponent<Image>().color = CellColours.GetColor(correspondingTeam);
	}

	//Updates team button colour depending on the team for a visual feedback and better representation
	private void UpdateTeamButtonVisual(Team thisTeam) {
		Text description = UI.cellTeam.transform.Find("TeamDescription").GetComponent<Text>();
		description.color = CellColours.GetContrastColor(thisTeam);
		description.text = ColourTheTeamButtons.GetDescriptionBasedOnTeam(thisTeam);
		UI.cellTeam.GetComponent<Image>().color = CellColours.GetColor(thisTeam);
	}

	//Editor setting whethrt cell should show thier maximum size
	public void CellOutlineToggle(Toggle toggle) {
		ShowCellOutline = toggle.isOn;
		foreach (EditCell cell in cellList) {
			cell.ToggleCellOutline(toggle.isOn);
		}
	}

	/// <summary>
	/// Refreshes the Ai difficulty
	/// </summary>
	/// <param name="team">the team this applies to, 0 = all</param>
	public void AiDiffHandler(Team team = Team.NEUTRAL) {
		if (float.TryParse(UI.aiDifficultyAllInput.text, NumberStyles.Float, CultureInfo.InvariantCulture, out GlobalAIDifficulty) && team == 0) {
			foreach (Team key in teamList) {
				if (aiDifficultyDict.ContainsKey(key)) {
					aiDifficultyDict.Remove(key);
					aiDifficultyDict.Add(key, GlobalAIDifficulty);
				}

			}

		}
		else if (!float.TryParse(UI.aiDifficultyAllInput.text, NumberStyles.Float, CultureInfo.InvariantCulture, out GlobalAIDifficulty) && team == 0) {
			foreach (Team key in teamList) {
				if (aiDifficultyDict.ContainsKey(key)) {
					aiDifficultyDict.Remove(key);
					aiDifficultyDict.Add(key, DEFAULT_DIFFICULTY);
				}

			}

		}
		else {
			float singleDiff;
			if (float.TryParse(UI.aiDifficultySingleInput.text, NumberStyles.Float, CultureInfo.InvariantCulture, out singleDiff)) {
				aiDifficultyDict.Remove(team);
				aiDifficultyDict.Add(team, singleDiff);


			}
			else {
				aiDifficultyDict.Remove(team);
				aiDifficultyDict.Add(team, DEFAULT_DIFFICULTY);

			}
		}
		UI.aiDifficultySingleInput.gameObject.SetActive(false);
		AllAiDifficultyWriter.RedoText(aiDifficultyDict);
	}

	//After doubleclicking a team on the round table an input field pops up where you can adjust the speed
	public void EnableSingleDiffInputField(TeamBox t) {

		UI.aiDifficultySingleInput.gameObject.SetActive(true);
		UI.aiDifficultySingleInput.transform.position = (Vector2)t.transform.position + new Vector2(0, t.myRectTransform.sizeDelta.y / 2);
		if (aiDifficultyDict.TryGetValue(t.team, out float value)) {
			UI.aiDifficultySingleInput.contentType = InputField.ContentType.Standard;
			UI.aiDifficultySingleInput.text = value.ToString();
			UI.aiDifficultySingleInput.contentType = InputField.ContentType.DecimalNumber;
		}
		else {
			UI.aiDifficultySingleInput.text = "";
		}
		singleDifficultyInputFieldSpecificTeam = t.team;
	}

	public void RefreshCameraSize(float val) {
		if (val != Camera.main.orthographicSize) {
			if (val < DEFAULT_GAME_SIZE) {
				UI.sizeInput.text = DEFAULT_GAME_SIZE.ToString();
				GameSize = DEFAULT_GAME_SIZE;
			}
			Camera.main.transform.position = Vector3.zero + Vector3.back * 10;
			Camera.main.orthographicSize = val;
			GameObject.Find("Borders").GetComponent<PlayFieldSetup>().ResizeBackground(Camera.main.aspect);
		}
	}


	//Calculation of camera size and position for zooming onto a cell
	public void FitCellsOnScreen(List<EditCell> cells) {
		if (cells.Count == 0) {
			RefreshCameraSize(GameSize);
			return;
		}
		float lowestX = Mathf.Infinity;
		float highestX = -Mathf.Infinity;
		float lowestY = Mathf.Infinity;
		float highestY = -Mathf.Infinity;
		float adjustBy = 0;

		foreach (EditCell cell in cellList) {
			if (cell.Cell.CellRadius * 0.33f > adjustBy) {
				adjustBy = cell.Cell.CellRadius * 0.33f;
			}
		}

		if (cells.Count == 1) {
			Vector3 pos = cells[0].transform.position;
			lowestX = pos.x - cells[0].Cell.CellRadius;
			highestX = pos.x + cells[0].Cell.CellRadius;
			lowestY = pos.y - cells[0].Cell.CellRadius;
			highestY = pos.y + cells[0].Cell.CellRadius;
		}
		else {
			foreach (CellBehaviour cell in cells) {
				Vector3 cellPos = cell.transform.position;
				if (cellPos.x - cell.Cell.CellRadius < lowestX) {
					lowestX = cellPos.x - cell.Cell.CellRadius;

				}
				if (cellPos.x + cell.Cell.CellRadius > highestX) {
					highestX = cellPos.x + cell.Cell.CellRadius;
				}
				if (cellPos.y - cell.Cell.CellRadius < lowestY) {
					lowestY = cellPos.y - cell.Cell.CellRadius;
				}
				if (cellPos.y + cell.Cell.CellRadius > highestY) {
					highestY = cellPos.y + cell.Cell.CellRadius;
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
}

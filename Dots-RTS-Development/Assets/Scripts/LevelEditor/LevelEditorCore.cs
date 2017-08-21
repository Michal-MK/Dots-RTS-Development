using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class LevelEditorCore : MonoBehaviour {

    public static event Control.PanelValueChanged panelChange;
    //When EditMode Changes, passes the new mode
    public static event Control.EditModeChanged modeChange;

    static Vector3 defaultCameraPosition;

    public static List<Cell> cellList = new List<Cell>();
    public static List<Cell.enmTeam> teamList = new List<Cell.enmTeam>();
    /// <summary>
    /// All of the input fields, there's only one input panel so this can be static
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
    public enum Mode { WaitForModeChange, EditCells, DeleteCells, PlaceCells, AssignUpgrades };
    public enum PCPanelAttribute { Team, Max, Start, Regen  };

    public static Mode editorMode;

    public Texture2D[] cursors;

    public static bool dontUpdate;

    #region Functions to add or remove a cell from the static list
    public static void AddCell(Cell c) {

        cellList.Add(c);
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
        teamList.Remove(c.cellTeam);
    }
    #endregion

    private IEnumerator Start() {
        //Find all of the input fields;
        teamButton = gameObject.GetComponent<TeamSelectScript>();
        maxInput = GameObject.Find("MAX_Elements_IF").GetComponent<InputField>();
        startInput = GameObject.Find("START_Elements_IF").GetComponent<InputField>();
        regenInput = GameObject.Find("REGEN_Elements_IF").GetComponent<InputField>();

        aiDifficultyAllInput = GameObject.Find("AI_Diff_IF").GetComponent<InputField>();
        aiDifficultySingleInput = GameObject.Find("Single_Ai_Diff_IF").GetComponent<InputField>();
        sizeInput = GameObject.Find("CAM_Size_IF").GetComponent<InputField>();

        //fileNameInput = GameObject.Find("FileNameIF").GetComponent<InputField>();
        levelNameInput = GameObject.Find("Level Name IF").GetComponent<InputField>();
        authorNameInput = GameObject.Find("Author's name IF").GetComponent<InputField>();

        //UpgradePanel
        upgradePanel = GameObject.Find("Upgrade_Panel");

        //Disable the panels;
        aiDifficultySingleInput.gameObject.SetActive(false);
        GameObject.Find("SavePanel").SetActive(false);
        GameObject.Find("GameSettingsPanel").SetActive(false);

        //Set the defaluts by parsing all of the input fields
        defaultCameraPosition = Camera.main.transform.position;
        GetPlaceCellPanelTeam();
        GetPlaceCellPanelRegen();
        GetPlaceCellPanelMax();
        GetPlaceCellPanelStart();
        GetGameSizeIf();
        GetIOPanelValues();
        AiDiffHandler();

        // Set defalut mode to placeCells
        //Have to wait for all of the things to initialize before I can call a button press
        yield return new WaitForEndOfFrame();
        PlaceCellButton();
    }

    public static void TeamSET() {
        teamButton.UpdateButtonVisual();
    }

    private void OnDestroy() {
        LevelEditorCore.cellList.Clear();
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
    public void UpgradesButton() {
        editorMode = Mode.AssignUpgrades;
        modeChange(Mode.AssignUpgrades);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void GetPlaceCellPanelTeam() {

        team = teamButton.team;

        if (panelChange != null && dontUpdate == false) {
            panelChange(PCPanelAttribute.Team);
        }

    }
    public void GetPlaceCellPanelMax() {

        if (!int.TryParse(maxInput.text, out max)) {
            max = defaultMax;
        }

        if (panelChange != null && dontUpdate == false) {
            panelChange(PCPanelAttribute.Max);
        }

    }
    public void GetPlaceCellPanelStart() {

        if (!int.TryParse(startInput.text, out start)) {
            start = defaultStart;
        }
        if (panelChange != null && dontUpdate == false) {
            panelChange(PCPanelAttribute.Start);
        }

    }
    public void GetPlaceCellPanelRegen() {

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

    public void GetIOPanelValues() {
        //if (string.IsNullOrEmpty(fileNameInput.text) || fileNameInput.text == " ") {
        //	fileName = defaultFileName;
        //}
        //else {
        //	fileName = fileNameInput.text;
        //}

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
    public void GetGameSizeIf() {
        float f = 250;
        if (float.TryParse(sizeInput.text, out f) && f > 250) {
            gameSize = f;

        }
        else {
            //print("setToDefault");
            gameSize = defaultGameSize;
        }
    }

    public static void BringUpUpgradePanel() {
        RectTransform rc = upgradePanel.GetComponent<RectTransform>();
        //rc.sizeDelta = new Vector2(rc.sizeDelta.x, 0);
    }
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

    public static int team {
        get { return _team; }
        set { _team = value; TeamSET(); }
    }
    public static float gameSize {
        get { return _gameSize; }
        set { _gameSize = value; RefreshCameraSize(value); }
    }
}

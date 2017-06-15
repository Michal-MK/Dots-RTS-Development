using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCell : MonoBehaviour {

	//public LevelEditorCore core;

	public Upgrade_Manager um;

	public static event GameControll.EnteredCellEditMode changedASelectionOfCell;

	private Camera _cam;
	private Vector3 _oldCamPos;

	public bool thereIsACellSelected;

	//instead of "thisCell" use keyword "this" or nothing since the class is not static
	Cell thisCell;

	private void Awake() {
		thisCell = gameObject.GetComponent<Cell>();
		_cam = Camera.main;
		//core = GameObject.Find("Main Camera").GetComponent<LevelEditorCore>();
		LevelEditorCore.modeChange += EditorModeUpdate;
		LevelEditorCore.panelChange += RefreshCellFromPanel;

	}
	private void OnDisable() {
		LevelEditorCore.modeChange -= EditorModeUpdate;
	}


	private void OnMouseOver() {
		if (Input.GetMouseButtonDown(0)) {
			if (LevelEditorCore.editorMode == LevelEditorCore.Mode.EditCells && thereIsACellSelected == false) {
				thereIsACellSelected = true;
				_oldCamPos = _cam.transform.position;
				_cam.transform.position = gameObject.transform.position + new Vector3(0, 0, -10);
				_cam.orthographicSize = _cam.orthographicSize * 0.25f;
				

				LevelEditorCore.teamInput.text = ((int)thisCell._team).ToString();
				LevelEditorCore.startInput.text = thisCell.elementCount.ToString();
				LevelEditorCore.regenInput.text = thisCell.regenPeriod.ToString();
				LevelEditorCore.maxInput.text = thisCell.maxElements.ToString();
			}
			if (LevelEditorCore.editorMode == LevelEditorCore.Mode.DeleteCells) {
				Destroy(gameObject);
			}
		}
	}


	private void Update() {
		if (thereIsACellSelected) {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				_cam.orthographicSize = _cam.orthographicSize * 4;
				_cam.transform.position = _oldCamPos;
				thereIsACellSelected = false;
			}
		}
	}

	private void RefreshCellFromPanel() {
		if (thereIsACellSelected) {
			thisCell.elementCount = LevelEditorCore.start;
			thisCell._team = (Cell.enmTeam)LevelEditorCore.team;
			thisCell.maxElements = LevelEditorCore.max;
			thisCell.regenPeriod = LevelEditorCore.regen;
		}
	}

	private void EditorModeUpdate(LevelEditorCore.Mode mode) {
		if (thereIsACellSelected) {
			_cam.orthographicSize = _cam.orthographicSize * 4;
			_cam.transform.position = _oldCamPos;
			thereIsACellSelected = false;
		}
	}

}

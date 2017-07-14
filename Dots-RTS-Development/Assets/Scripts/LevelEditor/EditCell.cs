using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCell : MonoBehaviour {

	//public LevelEditorCore core;

	public Upgrade_Manager um;

	public static event Control.EnteredCellEditMode changedASelectionOfCell;

	private Camera _cam;
	private Vector3 _oldCamPos;

	public bool isCellSelected = false;

	private float _defaultSize;
	private float _zoomedSize;

	Cell thisCell;

	private void Awake() {
		thisCell = gameObject.GetComponent<Cell>();
		_cam = Camera.main;

		LevelEditorCore.modeChange += EditorModeUpdate;
		LevelEditorCore.panelChange += RefreshCellFromPanel;

	}
	private void OnDisable() {
		LevelEditorCore.modeChange -= EditorModeUpdate;
		LevelEditorCore.panelChange -= RefreshCellFromPanel;

	}

	private void Start() {
		_defaultSize = _cam.orthographicSize;
		_zoomedSize = _defaultSize * 0.25f;
	}

	private void OnMouseOver() {
		if (Input.GetMouseButtonDown(0)) {
			if (LevelEditorCore.editorMode == LevelEditorCore.Mode.EditCells && isCellSelected == false) {
				isCellSelected = true;
				_oldCamPos = _cam.transform.position;
				_cam.transform.position = gameObject.transform.position + new Vector3(0, 0, -10);
				if (LevelEditorCore.start >= 10 && LevelEditorCore.start <= LevelEditorCore.max) {
					_cam.orthographicSize = Map.MapFloat(LevelEditorCore.start, 10, LevelEditorCore.max, _zoomedSize, 120);
				}

				LevelEditorCore.teamInput.team = ((int)thisCell.cellTeam);
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
		if (isCellSelected) {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				_cam.orthographicSize = _defaultSize;
				_cam.transform.position = _oldCamPos;
				isCellSelected = false;
			}
		}
	}

	private void RefreshCellFromPanel() {
		if (isCellSelected) {
			if (LevelEditorCore.start >= 10 && LevelEditorCore.start <= LevelEditorCore.max) {
				_cam.orthographicSize = Map.MapFloat(LevelEditorCore.start, 10, LevelEditorCore.max, _zoomedSize, 105.8f);
			}
			thisCell.elementCount = LevelEditorCore.start;
			thisCell.cellTeam = (Cell.enmTeam)LevelEditorCore.team;
			thisCell.maxElements = LevelEditorCore.max;
			thisCell.regenPeriod = LevelEditorCore.regen;
		}
	}

	private void EditorModeUpdate(LevelEditorCore.Mode mode) {
		if (isCellSelected) {
			_cam.orthographicSize = _defaultSize;
			_cam.transform.position = _oldCamPos;
			isCellSelected = false;
		}
	}

}

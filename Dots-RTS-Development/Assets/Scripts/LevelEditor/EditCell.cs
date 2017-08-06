using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditCell : Cell, IPointerDownHandler, IPointerUpHandler {

    //public LevelEditorCore core;


    public static event Control.EnteredCellEditMode changedASelectionOfCell;

    private Camera _cam;
    private Vector3 _oldCamPos;

    public bool isCellSelected = false;

    private float _defaultSize;
    private float _zoomedSize;

    float pointerDownAtTime;
    bool longPress;
    float longPressTreshold = 0.8f;
    bool lookForLongPress;


    private void Awake() {
        pointerDownAtTime = Mathf.Infinity;
        LevelEditorCore.modeChange += EditorModeUpdate;
        LevelEditorCore.panelChange += RefreshCellFromPanel;


    }
    private void OnDisable() {
        LevelEditorCore.modeChange -= EditorModeUpdate;
        LevelEditorCore.panelChange -= RefreshCellFromPanel;

    }

    //private void Start() {
    //	_defaultSize = _cam.orthographicSize;
    //	_zoomedSize = _defaultSize * 0.25f;
    //}

    //private void OnMouseOver() {
    //	if (Input.GetMouseButtonDown(0)) {
    //		if (LevelEditorCore.editorMode == LevelEditorCore.Mode.EditCells && isCellSelected == false) {
    //			isCellSelected = true;
    //			_oldCamPos = _cam.transform.position;
    //			_cam.transform.position = gameObject.transform.position + new Vector3(0, 0, -10);
    //			if (LevelEditorCore.start >= 10 && LevelEditorCore.start <= LevelEditorCore.max) {
    //				_cam.orthographicSize = Map.MapFloat(LevelEditorCore.start, 10, LevelEditorCore.max, _zoomedSize, 120);
    //			}

    //			LevelEditorCore.teamButton.team = ((int)thisCell.cellTeam);
    //			LevelEditorCore.startInput.text = thisCell.elementCount.ToString();
    //			LevelEditorCore.regenInput.text = thisCell.regenPeriod.ToString();
    //			LevelEditorCore.maxInput.text = thisCell.maxElements.ToString();
    //		}
    //		if (LevelEditorCore.editorMode == LevelEditorCore.Mode.DeleteCells) {
    //			LevelEditorCore.RemoveCell(gameObject.GetComponent<Cell>());
    //			Destroy(gameObject);

    //		}
    //	}
    //}


    //private void Update() {
    //	if (isCellSelected) {
    //		if (Input.GetKeyDown(KeyCode.Escape)) {
    //			_cam.orthographicSize = _defaultSize;
    //			_cam.transform.position = _oldCamPos;
    //			isCellSelected = false;
    //		}
    //	}
    //}

    private void RefreshCellFromPanel() {
        if (isCellSelected) {
            elementCount = LevelEditorCore.start;
            cellTeam = (Cell.enmTeam)LevelEditorCore.team;
            maxElements = LevelEditorCore.max;
            regenPeriod = LevelEditorCore.regen;
        }
    }

    private void EditorModeUpdate(LevelEditorCore.Mode mode) {

        circle.enabled = false;
        circle.positionCount = 0;
        isCellSelected = false;

    }

    public void OnPointerDown(PointerEventData eventData) {
        if (LevelEditorCore.editorMode == LevelEditorCore.Mode.EditCells) {
            pointerDownAtTime = Time.time;
            lookForLongPress = true;
            isCellSelected = !isCellSelected;
            if (isCellSelected) {
                LevelEditorCore.teamButton.team = ((int)cellTeam);
                LevelEditorCore.startInput.text = elementCount.ToString();
                LevelEditorCore.regenInput.text = regenPeriod.ToString();
                LevelEditorCore.maxInput.text = maxElements.ToString();

                CreateCircle(transform.position, cellRadius, 30);
                circle.enabled = true;
                circle.startColor = Color.magenta;
                circle.endColor = Color.magenta;
            }
            else {
                circle.enabled = false;

            }
            
        }


        if (LevelEditorCore.editorMode == LevelEditorCore.Mode.DeleteCells) {
            LevelEditorCore.RemoveCell(gameObject.GetComponent<Cell>());
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (lookForLongPress) {
            if (Time.time - pointerDownAtTime > longPressTreshold) {
                longPress = true;

                circle.startColor = Color.green;
                circle.endColor = Color.green;
            }
        }
        if (longPress) {
            gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
        }
    }


    public void OnPointerUp(PointerEventData eventData) {
        if (isCellSelected) {
            CreateCircle(transform.position, cellRadius, 30);
            circle.startColor = Color.magenta;
            circle.endColor = Color.magenta;
            circle.enabled = true;
        }
        lookForLongPress = false;
        longPress = false;
    }
}

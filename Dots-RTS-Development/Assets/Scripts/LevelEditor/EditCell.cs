using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditCell : Cell ,IPointerDownHandler, IPointerUpHandler {


	public static event Control.CellSelected OnCellSelected;
	public static event Control.CellSelected OnCellDeselected;


	public LevelEditorCore core;

	public GameObject myMaxSizeOutline;

	private bool _isCellSelected = false;

	private float pointerDownAtTime;
	private bool longPress;
	private float longPressTreshold = 0.8f;
	private bool lookForLongPress;

	public override void Awake() {
		base.Awake();
		pointerDownAtTime = Mathf.Infinity;
		LevelEditorCore.modeChange += EditorModeUpdate;
		LevelEditorCore.panelValueParsed += RefreshCellFromPanel;
	}

	private void OnDisable() {
		LevelEditorCore.modeChange -= EditorModeUpdate;
		LevelEditorCore.panelValueParsed -= RefreshCellFromPanel;
	}

	public void FastResize() {
		float mappedValue;
		if (elementCount < 10) {
			mappedValue = 1;
		}
		else if (elementCount >= 10 && elementCount <= maxElements) {
			mappedValue = Map.MapFloat(elementCount, 10, maxElements, 1f, 2f);
		}
		else {
			if (elementCount < 1000) {
				mappedValue = Map.MapFloat(elementCount, maxElements, 999f, 2f, 4f);
			}
			else {
				mappedValue = 4;
			}
		}
		transform.localScale = new Vector3(mappedValue, mappedValue);
		cellRadius = col.radius * transform.localScale.x;
	}


	private void RefreshCellFromPanel(LevelEditorCore.PCPanelAttribute attribute) {
		//print(isCellSelected);
		if (isCellSelected && core.isUpdateSentByCell == false) {
			if (attribute == LevelEditorCore.PCPanelAttribute.Start) {
			
				elementCount = core.start;

				FastResize();
			}
			if (attribute == LevelEditorCore.PCPanelAttribute.Team) {
				cellTeam = core.team;
			}
			if (attribute == LevelEditorCore.PCPanelAttribute.Max) {
				maxElements = core.max;

				FastResize();
			}
			if (attribute == LevelEditorCore.PCPanelAttribute.Regen) {
				regenPeriod = core.regen;
			}
		}
	}

	private void EditorModeUpdate(LevelEditorCore.Mode mode) {
		isCellSelected = false;
	}

	public void OnPointerDown(PointerEventData eventData) {
		if (core.editorMode == LevelEditorCore.Mode.EditCells) {
			pointerDownAtTime = Time.time;
			lookForLongPress = true;
		}
		if (core.editorMode == LevelEditorCore.Mode.DeleteCells) {
			core.RemoveCell(gameObject.GetComponent<EditCell>());
			Destroy(gameObject);
		}
	}

	public void ToggleCellOutline(bool on) {
		if (on) {
			myMaxSizeOutline.SetActive(true);
		}
		else {
			myMaxSizeOutline.SetActive(false);
		}
	}

	private void Update() {
		if (lookForLongPress) {
			if (Time.time - pointerDownAtTime > longPressTreshold) {
				longPress = true;
				EnableCircle(Color.green);
				lookForLongPress = false;
			}
		}
		if (longPress ) {
			gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
		}
	}

	public void OnPointerUp(PointerEventData eventData) {
		

		
		if (lookForLongPress == false) {
			isCellSelected = true;
		}
		else {
			isCellSelected = !isCellSelected;
		}
		if (isCellSelected) {
			core.isUpdateSentByCell = true;
			core.team = cellTeam;
			core.start = elementCount;
			core.regen = regenPeriod;
			core.max = maxElements;
			core.isUpdateSentByCell = false;
			EnableCircle(Color.magenta);
		}

		longPress = false;
		lookForLongPress = false;
	}

	public bool isCellSelected {
		get { return _isCellSelected; }
		set {
			if (value == true) {
				OnCellSelected(this);
				cellSelectedRenderer.enabled = true;
			}
			else {
				OnCellDeselected(this);
				cellSelectedRenderer.enabled = false;
			}
			_isCellSelected = value;
		}
	}
}

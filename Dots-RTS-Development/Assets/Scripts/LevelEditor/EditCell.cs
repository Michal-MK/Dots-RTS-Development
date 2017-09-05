using UnityEngine;
using UnityEngine.EventSystems;

public class EditCell : Cell ,IPointerDownHandler, IPointerUpHandler {

	public LevelEditorCore core;

	public GameObject myMaxSizeOutline;

	public bool isCellSelected = false;

	float pointerDownAtTime;
	bool longPress;
	float longPressTreshold = 0.8f;
	bool lookForLongPress;




	public override void Awake() {
		base.Awake();
		pointerDownAtTime = Mathf.Infinity;
		LevelEditorCore.modeChange += EditorModeUpdate;
		LevelEditorCore.panelChange += RefreshCellFromPanel;
	}

	private void OnDisable() {
		LevelEditorCore.modeChange -= EditorModeUpdate;
		LevelEditorCore.panelChange -= RefreshCellFromPanel;
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

	#region Comment
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
	#endregion

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

		core.selectedCellList.Remove(gameObject.GetComponent<EditCell>());
		isCellSelected = false;
		cellSelected.enabled = false;
		//if (LevelEditorCore.areCellsFitToScreen) {
		//	LevelEditorCore.FitCellsOnScreen(LevelEditorCore.selectedCellList);
		//}

	}

	public void OnPointerDown(PointerEventData eventData) {
		if (core.editorMode == LevelEditorCore.Mode.EditCells) {
			pointerDownAtTime = Time.time;
			lookForLongPress = true;
			isCellSelected = !isCellSelected;
			if (isCellSelected) {
				core.selectedCellList.Add(gameObject.GetComponent<EditCell>());

				core.isUpdateSentByCell = true;
				core.team = cellTeam;
				core.start = elementCount;
				core.regen = regenPeriod;
				core.max = maxElements;
				core.isUpdateSentByCell = false;

				EnableCircle(Color.magenta);
				//if (LevelEditorCore.areCellsFitToScreen) {
				//	LevelEditorCore.FitCellsOnScreen(LevelEditorCore.selectedCellList);
				//}
			}
			else {

				core.selectedCellList.Remove(gameObject.GetComponent<EditCell>());
				cellSelected.enabled = false;
				//if (LevelEditorCore.areCellsFitToScreen) {
				//	LevelEditorCore.FitCellsOnScreen(LevelEditorCore.selectedCellList);
				//}
			}

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
				isCellSelected = true;
				EnableCircle(Color.green);
			}
		}
		if (longPress) {
			gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
		}
	}


	public void OnPointerUp(PointerEventData eventData) {
		if (isCellSelected) {
			EnableCircle(Color.magenta);
			if (!core.selectedCellList.Contains(this)) {
				core.selectedCellList.Add(this);
				//if (LevelEditorCore.areCellsFitToScreen) {
				//	LevelEditorCore.FitCellsOnScreen(LevelEditorCore.selectedCellList);
				//}
			}

		}
		lookForLongPress = false;
		longPress = false;
	}
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditCell : Cell, IPointerDownHandler, IPointerUpHandler {


	public static event Control.CellSelected OnCellSelected;
	public static event Control.CellSelected OnCellDeselected;

	public LevelEditorCore core;

	#region PrefabReferences
	public SpriteRenderer maxCellRadius;
	public UM_Editor upgrade_manager;
	#endregion


	private bool _isCellSelected = false;

	#region Press legth detection
	private float pointerDownAtTime;
	private bool longPress;
	private float longPressTreshold = 0.8f;
	private bool lookForLongPress;
	#endregion

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
		if (LevelEditorCore.getOutilneState) {
			ToggleCellOutline(true);
		}
	}



	private void RefreshCellFromPanel(LevelEditorCore.PCPanelAttribute attribute) {
		if (isCellSelected && core.isUpdateSentByCell == false) {
			if (attribute == LevelEditorCore.PCPanelAttribute.Start) {

				elementCount = core.startingElementCount;

				FastResize();
			}
			if (attribute == LevelEditorCore.PCPanelAttribute.Team) {
				cellTeam = core.team;
				if (cellTeam != enmTeam.ALLIED && cellTeam != enmTeam.NEUTRAL) {
					core.AddCell(this);
				}
			}
			if (attribute == LevelEditorCore.PCPanelAttribute.Max) {
				maxElements = core.maxElementCount;

				FastResize();
			}
			if (attribute == LevelEditorCore.PCPanelAttribute.Regen) {
				regenPeriod = core.regenarationPeriod;
			}
			if (attribute == LevelEditorCore.PCPanelAttribute.Upgrades) {
				Array.Copy(UpgradeSlot.instances, upgrade_manager.upgrades, UpgradeSlot.instances.Length);
				UpdateUpgradeVisual();
			}
		}
	}

	private void UpdateUpgradeVisual() {
		for (int i = 0; i < upgrade_manager.upgrades.Length; i++) {
			upgrade_manager.upgrade_Slots[i].type = upgrade_manager.upgrades[i];
			upgrade_manager.upgrade_Slots[i].ChangeUpgradeImage(Upgrade.UPGRADE_GRAPHICS[upgrade_manager.upgrades[i]]);
		}
	}

	private void EditorModeUpdate(LevelEditorCore.Mode mode) {
		if (isCellSelected == true) {
			//print(gameObject.name + " is deselecting");
			isCellSelected = false;
		}
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
			maxCellRadius.enabled = true;
			maxCellRadius.transform.localScale = new Vector2((2 / maxCellRadius.transform.parent.localScale.x), (2 / maxCellRadius.transform.parent.localScale.x));
			//Debug.LogWarning("I smell hardcoded BS!");
		}
		else {
			maxCellRadius.enabled = false;
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
		if (longPress) {
			gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
		}
	}

	public void OnPointerUp(PointerEventData eventData) {


		if (core.editorMode == LevelEditorCore.Mode.EditCells) {
			if (lookForLongPress == false) {
				isCellSelected = true;
			}
			else {
				isCellSelected = !isCellSelected;
			}
			if (isCellSelected) {
				core.isUpdateSentByCell = true;
				core.team = cellTeam;
				core.startingElementCount = elementCount;
				core.regenarationPeriod = regenPeriod;
				core.maxElementCount = maxElements;
				core.isUpdateSentByCell = false;
				EnableCircle(Color.magenta);
			}



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
				upgrade_manager.ToggleUpgradeInteraction(true);
			}
			else {
				OnCellDeselected(this);
				cellSelectedRenderer.enabled = false;
				upgrade_manager.ToggleUpgradeInteraction(false);
			}
			_isCellSelected = value;
		}
	}
}

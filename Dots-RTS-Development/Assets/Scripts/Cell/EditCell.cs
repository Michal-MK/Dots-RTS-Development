using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditCell : CellBehaviour, IPointerDownHandler, IPointerUpHandler {


	public static event Control.CellSelected OnCellSelected;
	public static event Control.CellSelected OnCellDeselected;

	public LevelEditorCore core;

	#region Prefab References
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

	public void Awake() {
		cellSprite = GetComponent<SpriteRenderer>();
		col = GetComponent<CircleCollider2D>();
		rg = GetComponent<Rigidbody2D>();
		uManager = GetComponent<Upgrade_Manager>();

		GameObject count = transform.Find("Count").gameObject;
		elementCountDisplay = count.GetComponent<TextMeshPro>();
		elementCountDisplayRenderer = count.GetComponent<MeshRenderer>();

		cellSelectedRenderer = transform.Find("Selected").GetComponent<SpriteRenderer>();

		Cell.CellRadius = col.radius * transform.localScale.x;
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
		if (Cell.ElementCount < 10) {
			mappedValue = 1;
		}
		else if (Cell.ElementCount >= 10 && Cell.ElementCount <= Cell.MaxElements) {
			mappedValue = Map.MapFloat(Cell.ElementCount, 10, Cell.MaxElements, 1f, 2f);
		}
		else {
			if (Cell.ElementCount < 1000) {
				mappedValue = Map.MapFloat(Cell.ElementCount, Cell.MaxElements, 999f, 2f, 4f);
			}
			else {
				mappedValue = 4;
			}
		}
		transform.localScale = new Vector3(mappedValue, mappedValue);
		Cell.CellRadius = col.radius * transform.localScale.x;
		if (LevelEditorCore.getOutilneState) {
			ToggleCellOutline(true);
		}
	}



	private void RefreshCellFromPanel(LevelEditorCore.PCPanelAttribute attribute) {
		if (isCellSelected && core.isUpdateSentByCell == false) {
			if (attribute == LevelEditorCore.PCPanelAttribute.Start) {

				Cell.ElementCount = core.startingElementCount;

				FastResize();
			}
			if (attribute == LevelEditorCore.PCPanelAttribute.Team) {
				Cell.Team = core.team;
				if (Cell.Team != Team.ALLIED && Cell.Team != Team.NEUTRAL) {
					core.AddCell(this);
				}
			}
			if (attribute == LevelEditorCore.PCPanelAttribute.Max) {
				Cell.MaxElements = core.maxElementCount;

				FastResize();
			}
			if (attribute == LevelEditorCore.PCPanelAttribute.Regen) {
				Cell.RegenPeriod = core.regenarationPeriod;
			}
			if (attribute == LevelEditorCore.PCPanelAttribute.Upgrades) {
				//Array.Copy(UpgradeSlot.UpgradeInstances, upgrade_manager.upgrades, UpgradeSlot.UpgradeInstances.Length); //TODO
				UpdateUpgradeVisual();
			}
		}
	}

	private void UpdateUpgradeVisual() {
		for (int i = 0; i < upgrade_manager.upgrades.Length; i++) {
			upgrade_manager.upgradeSlots[i].Type = upgrade_manager.upgrades[i];
			upgrade_manager.upgradeSlots[i].ChangeUpgradeImage(Upgrade.UPGRADE_GRAPHICS[upgrade_manager.upgrades[i]]);
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
			maxCellRadius.transform.localScale = new Vector2(2 / maxCellRadius.transform.parent.localScale.x, (2 / maxCellRadius.transform.parent.localScale.x));
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
				core.team = Cell.Team;
				core.startingElementCount = Cell.ElementCount;
				core.regenarationPeriod = Cell.RegenPeriod;
				core.maxElementCount = Cell.MaxElements;
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

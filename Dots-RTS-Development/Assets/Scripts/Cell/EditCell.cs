using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditCell : CellBehaviour, IPointerDownHandler, IPointerUpHandler {


	public event EventHandler<EditCell> OnCellSelected;
	public event EventHandler<EditCell> OnCellDeselected;

	public LevelEditorCore core;

	#region Prefab References
	public SpriteRenderer maxCellRadius;
	public EditorUpgradeManager upgrade_manager;
	#endregion


	private bool isCellSelected;

	#region Press length detection
	private float pointerDownAtTime;
	private bool longPress;
	private const float LONG_PRESS_THRESHOLD = 0.8f;
	private bool lookForLongPress;
	#endregion

	public void Awake() {
		cellSprite = GetComponent<SpriteRenderer>();
		col = GetComponent<CircleCollider2D>();
		rg = GetComponent<Rigidbody2D>();
		uManager = GetComponent<UpgradeManager>();

		GameObject count = transform.Find("Count").gameObject;
		elementCountDisplay = count.GetComponent<TextMeshPro>();
		elementCountDisplayRenderer = count.GetComponent<MeshRenderer>();

		cellSelectedRenderer = transform.Find("Selected").GetComponent<SpriteRenderer>();

		Cell.cellRadius = col.radius * transform.localScale.x;
		pointerDownAtTime = Mathf.Infinity;
	}

	public void FastResize() {
		float mappedValue;
		if (Cell.elementCount < 10) {
			mappedValue = 1;
		}
		else if (Cell.elementCount >= 10 && Cell.elementCount <= Cell.maxElements) {
			mappedValue = Map.MapFloat(Cell.elementCount, 10, Cell.maxElements, 1f, 2f);
		}
		else {
			if (Cell.elementCount < 1000) {
				mappedValue = Map.MapFloat(Cell.elementCount, Cell.maxElements, 999f, 2f, 4f);
			}
			else {
				mappedValue = 4;
			}
		}
		transform.localScale = new Vector3(mappedValue, mappedValue);
		Cell.cellRadius = col.radius * transform.localScale.x;
	}

	private void UpdateUpgradeVisual() {
		for (int i = 0; i < upgrade_manager.upgrades.Length; i++) {
			upgrade_manager.upgradeSlots[i].Type = upgrade_manager.upgrades[i];
			upgrade_manager.upgradeSlots[i].ChangeUpgradeImage(Upgrade.UpgradeGraphics[upgrade_manager.upgrades[i]]);
		}
	}

	public void UpdateStats() {
		Cell.elementCount = core.StartElements;
		Cell.maxElements = core.MaxElements;
		Cell.regenPeriod = core.Regeneration;
		FastResize();
		UpdateVisual();
	}

	public void OnPointerDown(PointerEventData eventData) {
		if (core.EditorMode == EditorMode.EditCells) {
			pointerDownAtTime = Time.time;
			lookForLongPress = true;
		}
		else if (core.EditorMode == EditorMode.DeleteCells) {
			core.RemoveCell(gameObject.GetComponent<EditCell>());
			Destroy(gameObject);
		}
	}

	public void ToggleCellOutline(bool on) {
		if (on) {
			maxCellRadius.enabled = true;
			maxCellRadius.transform.localScale = new Vector2(2 / maxCellRadius.transform.parent.localScale.x, (2 / maxCellRadius.transform.parent.localScale.x));
		}
		else {
			maxCellRadius.enabled = false;
		}
	}

	private void Update() {
		if (lookForLongPress) {
			if (Time.time - pointerDownAtTime > LONG_PRESS_THRESHOLD) {
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
		if (core.EditorMode == EditorMode.EditCells) {
			if (lookForLongPress == false) {
				IsCellSelected = true;
			}
			else {
				IsCellSelected = !IsCellSelected;
			}
			if (IsCellSelected) {
				core.Team = Cell.team;
				core.StartElements = Cell.elementCount;
				core.Regeneration = Cell.regenPeriod;
				core.MaxElements = Cell.maxElements;
				EnableCircle(Color.magenta);
			}
		}
		longPress = false;
		lookForLongPress = false;
	}

	public bool IsCellSelected {
		get => isCellSelected;
		set {
			if (value) {
				OnCellSelected?.Invoke(this,this);
				cellSelectedRenderer.enabled = true;
				upgrade_manager.ToggleUpgradeInteraction(true);
			}
			else {
				OnCellDeselected?.Invoke(this, this);
				cellSelectedRenderer.enabled = false;
				upgrade_manager.ToggleUpgradeInteraction(false);
			}
			isCellSelected = value;
		}
	}
}

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
	public UM_Editor upgrade_manager;
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
		uManager = GetComponent<Upgrade_Manager>();

		GameObject count = transform.Find("Count").gameObject;
		elementCountDisplay = count.GetComponent<TextMeshPro>();
		elementCountDisplayRenderer = count.GetComponent<MeshRenderer>();

		cellSelectedRenderer = transform.Find("Selected").GetComponent<SpriteRenderer>();

		Cell.CellRadius = col.radius * transform.localScale.x;
		pointerDownAtTime = Mathf.Infinity;
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
	}

	private void UpdateUpgradeVisual() {
		for (int i = 0; i < upgrade_manager.upgrades.Length; i++) {
			upgrade_manager.upgradeSlots[i].Type = upgrade_manager.upgrades[i];
			upgrade_manager.upgradeSlots[i].ChangeUpgradeImage(Upgrade.UpgradeGraphics[upgrade_manager.upgrades[i]]);
		}
	}

	public void UpdateStats() {
		Cell.ElementCount = core.StartElements;
		Cell.MaxElements = core.MaxElements;
		Cell.RegenPeriod = core.Regeneration;
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
				core.Team = Cell.Team;
				core.StartElements = Cell.ElementCount;
				core.Regeneration = Cell.RegenPeriod;
				core.MaxElements = Cell.MaxElements;
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

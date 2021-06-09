using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UM_InGame : Upgrade_Manager, IPointerClickHandler {

	private static UM_InGame currentCell;

	public event EventHandler<UM_InGame> OnUpgradeBegin;
	public event EventHandler<UM_InGame> OnUpgradeQuit;

	public GameCell cell;
	public SpriteRenderer slotRender;
	public Transform slotHolder;

	public readonly GameObject[] slotHolders = new GameObject[8];

	[HideInInspector]
	public readonly BoxCollider2D[] slots = new BoxCollider2D[8];

	private void Start() {
		for (int i = 0; i < slots.Length; i++) {
			slots[i] = slotHolder.GetChild(i).GetComponent<BoxCollider2D>();
		}
	}

	protected override void UpgradePreinstallSprites() {
		for (int i = 0; i < slotHolders.Length; i++) {
			if (upgrades[i] != Upgrades.NONE) {
				slotHolders[i].GetComponent<SpriteRenderer>().sprite = Upgrade.UpgradeGraphics[upgrades[i]];
				slotHolders[i].GetComponent<SpriteRenderer>().size = Vector2.one * 25f;
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.clickCount != 2 || currentCell == null) return;

		OnUpgradeQuit?.Invoke(this, currentCell);
		isUpgrading = true;
		currentCell = this;
		OnUpgradeBegin?.Invoke(this, currentCell);
	}

	private void Update() {
		if (!Input.GetKeyDown(KeyCode.Escape) || !isUpgrading || currentCell == null) return;
		OnUpgradeQuit?.Invoke(this, currentCell);
		isUpgrading = false;
		currentCell = null;
	}
}

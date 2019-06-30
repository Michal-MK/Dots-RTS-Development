﻿using UnityEngine;

public class UM_Editor : Upgrade_Manager {

	#region Unity UI

	private readonly SpriteRenderer[] ui_upgradeSlotsSprite = new SpriteRenderer[8];
	private readonly BoxCollider2D[] ui_upgradeSlotColider = new BoxCollider2D[8];
	public SpriteRenderer upgradeSlotFields;

	#endregion

	private EditCell currentCell;
	public UpgradeSlot_Cell[] upgradeSlots = new UpgradeSlot_Cell[8];

	public void SetupUpgrades(LevelEditorUI UI) {
		GameObject uiUpgrades = UI.upgradePickerButtons;

		int i = 0;
		foreach (UpgradeSlot slot in uiUpgrades.GetComponentsInChildren<UpgradeSlot>()) {
			ui_upgradeSlotsSprite[i] = slot.gameObject.GetComponent<SpriteRenderer>();
			ui_upgradeSlotColider[i] = slot.gameObject.GetComponent<BoxCollider2D>();
			i++;
			slot.OnSlotClicked += Slot_OnSlotClicked;
		}
		currentCell = GetComponent<EditCell>();
	}

	private void Slot_OnSlotClicked(object sender, UpgradeSlot e) {
		print($"I belong to {e.transform.parent.parent.name} you clicked slot {e.SlotID}");
	}

	private void OnDestroy() {
		foreach (UpgradeSlot slot in upgradeSlots) {
			slot.OnSlotClicked -= Slot_OnSlotClicked;
		}
	}

	protected override void UpgradePreinstallSprites() {
		for (int i = 0; i < upgradeSlots.Length; i++) {
			if (upgrades[i] != Upgrades.NONE) {
				upgradeSlots[i].selfSprite.sprite = Upgrade.UPGRADE_GRAPHICS[upgrades[i]];
				upgradeSlots[i].selfSprite.size = Vector2.one * 25f;
			}
		}
	}


	public void ToggleUpgradeInteraction(bool isOn) {
		for (int i = 0; i < upgradeSlots.Length; i++) {
			upgradeSlots[i].GetComponent<BoxCollider2D>().enabled = isOn;
		}
		if (isOn) {
			upgradeSlotFields.color = new Color(1, 1, 1, 0.4f);
		}
		else {
			upgradeSlotFields.color = new Color(1, 1, 1, 0f);
		}
	}
}

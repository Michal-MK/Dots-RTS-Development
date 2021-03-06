﻿using UnityEngine;

public class UM_Editor : Upgrade_Manager {

	private EditCell currentCell;

	private SpriteRenderer[] ui_upgradeSlotsSprite = new SpriteRenderer[8];
	private BoxCollider2D[] ui_upgradeSlotColider = new BoxCollider2D[8];

	public SpriteRenderer upgradeSlotFields;
	public UpgradeSlot[] upgrade_Slots = new UpgradeSlot[8];


	private static bool isSubscribed = false;
	private void Start() {
		GameObject uiUpgrades = UI_ReferenceHolder.LE_cellPanel.transform.parent.Find("UI_Upgrades").gameObject;

		int i = 0;
		foreach (UpgradeSlot slot in uiUpgrades.GetComponentsInChildren<UpgradeSlot>()) {
			ui_upgradeSlotsSprite[i] = slot.gameObject.GetComponent<SpriteRenderer>();
			ui_upgradeSlotColider[i] = slot.gameObject.GetComponent<BoxCollider2D>();
			i++;
		}

		currentCell = GetComponent<EditCell>();

		if (!isSubscribed) {
			UpgradeSlot.OnSlotClicked += UpgradeSlot_OnSlotClicked;
			isSubscribed = true;
		}
	}

	private void OnDestroy() {
		UpgradeSlot.OnSlotClicked -= UpgradeSlot_OnSlotClicked;
	}

	private void UpgradeSlot_OnSlotClicked(UpgradeSlot sender, int slot) {
		print("I belong to " + sender.transform.parent.parent.name + " you clicked slot " + slot);
	}

	public void ToggleUpgradeInteraction(bool isOn) {
		for (int i = 0; i < upgrade_Slots.Length; i++) {
			upgrade_Slots[i].GetComponent<BoxCollider2D>().enabled = isOn;
		}
		if (isOn) {
			upgradeSlotFields.color = new Color(1, 1, 1, 0.4f);
		}
		else {
			upgradeSlotFields.color = new Color(1, 1, 1, 0f);
		}
	}
}

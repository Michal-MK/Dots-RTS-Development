using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSlotState : Upgrade_Manager {
	public Upgrade_Manager cellUpgrades;
	public bool isUpgraded = false;

	public Upgrade current = Upgrade.NONE;

	//Returns this UpgradeSlot's installed upgrade
	public Upgrade GetInstalledUpgrade() {
		return current;
	}
	//Show upgrade slots on enter
	private void OnMouseEnter() {
		if (cellScript.team == CellScript.enmTeam.ALLIED) {
			upgradeSlotsRenderer.color = new Color32(255, 255, 255, 255);
		}
	}
	//Hide upgrade slots on leave
	private void OnMouseOver() {
		if (Input.GetMouseButtonDown(1)) {
			print("Slot ID: " + SlotID(gameObject) + " Installed upgrade: " + GetInstalledUpgrade());
		}
	}

	//Returns slots index in Upgrade Manager array as an enum
	public Slot SlotID(GameObject slotHolder) {
		Slot id = 0;

		for(int i = 0; i < cellUpgrades.slots.Length; i++) {
			if (slotHolder == cellUpgrades.slots[i]) {
				return id = (Slot)i;
			}
		}
		return id;
	}
}

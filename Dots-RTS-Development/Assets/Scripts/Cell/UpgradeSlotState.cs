using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSlotState : MonoBehaviour {
	public Upgrade_Manager cellUpgrades;
	public bool isUpgraded = false;


	public Upgrade_Manager.Upgrade current = Upgrade_Manager.Upgrade.NoUpgrade;

	//Returns this UpgradeSlot's installed upgrade
	public Upgrade_Manager.Upgrade GetInstalledUpgrade() {
		return current;
	}

	private void OnMouseOver() {
		if (Input.GetMouseButtonDown(1)) {
			print("Slot ID: " + SlotID(gameObject) + " Installed upgrade: " + GetInstalledUpgrade());
		}
	}

	//Returns slots index in Upgrade Manager array as an enum
	public Upgrade_Manager.Slot SlotID(GameObject slotHolder) {
		Upgrade_Manager.Slot id = 0;

		for(int i = 0; i < cellUpgrades.slots.Length; i++) {
			if (slotHolder == cellUpgrades.slots[i]) {
				return id = (Upgrade_Manager.Slot)i;
			}
		}
		return id;
	}
}

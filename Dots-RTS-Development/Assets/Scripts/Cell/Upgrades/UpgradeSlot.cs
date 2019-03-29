using System;
using UnityEngine;

/*
 * This script is used for data storage, it is included in upgrade prefabs in editor,
 * as well as in play scene in the upgrade panel, pretty much every "TeamBox" upgrade has a script that derives from this.
 */

public abstract class UpgradeSlot : MonoBehaviour {

	public int SlotID { get; set; }
	public Upgrades Type { get; set; } = Upgrades.NONE;

	public Upgrades[] UpgradeInstances { get; set; } = new Upgrades[8] {
		Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE,Upgrades.NONE
	};

	public event EventHandler<OnUpgradeSlotClickedEventArgs> OnSlotClicked;

	public bool IsSubscribed => OnSlotClicked != null;

	protected virtual void Start() {
		SlotID = int.Parse(transform.name);
		if (!IsSubscribed) {
			ClearUpgradeSlot.OnSlotClear += ClearUpgradeSlot_OnSlotClear;
		}
	}

	public abstract void ChangeUpgradeImage(Sprite newSprite);

	protected virtual void OnDestroy() {
		ClearUpgradeSlot.OnSlotClear -= ClearUpgradeSlot_OnSlotClear;
	}

	protected virtual void ClearUpgradeSlot_OnSlotClear(object sender, UpgradeSlot e) {
		print("Base Class set statics to NONE on slot " + e.SlotID);
	}
}

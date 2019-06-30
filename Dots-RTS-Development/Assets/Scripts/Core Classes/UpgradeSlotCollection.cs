using System.Linq;
using UnityEngine;

public class UpgradeSlotCollection {

	public UpgradeSlot[] UpgradeSlots { get; set; }

	public UpgradeSlotCollection(Transform holder) {
		UpgradeSlots = holder.GetComponentsInChildren<UpgradeSlot>();
	}

	public Upgrades[] Upgrades => UpgradeSlots.Select(s => s.Type).ToArray();
}

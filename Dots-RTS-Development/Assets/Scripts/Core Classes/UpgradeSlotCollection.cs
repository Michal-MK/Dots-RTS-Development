using System.Linq;
using UnityEngine;

public class UpgradeSlotCollection {
	private readonly UpgradeSlot[] upgradeSlots;

	public UpgradeSlotCollection(Transform holder) {
		upgradeSlots = holder.GetComponentsInChildren<UpgradeSlot>();
	}

	public Upgrades[] Upgrades => upgradeSlots.Select(s => s.Type).ToArray();
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * This script is used for data storage, it is included in upgrade prefabs in editor,
 * as well as in play scene in the upgrade panel, pretty much every "TeamBox" upgrade has a script that derives from this.
 */

public class UpgradeSlot : MonoBehaviour {

	public int _slot;
	public Upgrade.Upgrades _type = Upgrade.Upgrades.NONE;


	public delegate void OnUpgradeSlotClick(UpgradeSlot sender, int slot);
	public static event OnUpgradeSlotClick OnSlotClicked;

	protected static UpgradeSlot highlightedSlot;

	private static bool isSubscribed = false;

	protected virtual void Start() {
		_slot = int.Parse(transform.name);

		if (!isSubscribed) {
			ClearUpgradeSlot.OnSlotClear += ClearUpgradeSlot_OnSlotClear;
		}
	}

	public virtual void ChangeUpgradeImage(Sprite newSprite) {

	}

	protected virtual void OnDestroy() {
		ClearUpgradeSlot.OnSlotClear -= ClearUpgradeSlot_OnSlotClear;
	}

	protected virtual void ClearUpgradeSlot_OnSlotClear(UpgradeSlot clicked) {
		print("Base Class set statics to NONE on slot " + getSlotID);
	}


	public static UpgradeSlot getHighlightedSlot {
		get { return highlightedSlot; }
	}

	public int getSlotID {
		get { return _slot; }
	}

	public Upgrade.Upgrades type {
		get { return _type; }
		set { _type = value; }
	}
}
